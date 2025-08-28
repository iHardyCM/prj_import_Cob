using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using prj_import_biznes.DTOs;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace prj_import_biznes.Services.Import
{
    public class SmsImportService : IChannelImportService
    {
        private readonly IConfiguration _cfg;
        public SmsImportService(IConfiguration cfg) => _cfg = cfg;
        public ChannelType channel => ChannelType.Sms;

        private static string Normalize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            // quita espacios/guiones bajos y acentos, y pasa a minúsculas
            var t = s.Trim().ToLowerInvariant().Replace(" ", "").Replace("_", "");
            return RemoveDiacritics(t);
        }

        private static string RemoveDiacritics(string text)
        {
            var norm = text.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder(norm.Length);
            foreach (var ch in norm)
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) !=
                    System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        public async Task<ImportResult> ImportAsync(int IdCartera, IFormFile file, string? usuario, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0) throw new InvalidOperationException("Archivo vacío.");
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".xlsx") throw new InvalidOperationException("Solo se acepta .xlsx.");

            // TVP: DNI, Telefono, Mensaje
            var tvp = new DataTable();
            tvp.Columns.Add("DNI", typeof(string));
            tvp.Columns.Add("Telefono", typeof(string));
            tvp.Columns.Add("Mensaje", typeof(string));

            using (var stream = file.OpenReadStream())
            using (var wb = new XLWorkbook(stream))
            {
                var ws = wb.Worksheets.FirstOrDefault();
                if (ws is null)
                    throw new InvalidOperationException("El archivo no tiene hojas.");

                var firstRow = ws.FirstRowUsed();
                if (firstRow is null)
                    throw new InvalidOperationException("La hoja está vacía.");

                var headerRow = firstRow.RowNumber();

                var headers = ws.Row(headerRow).Cells().ToDictionary(
                    c => Normalize(c.GetString()), c => c.Address.ColumnNumber);

                if (!headers.TryGetValue("dni", out int cDni) ||
                    !headers.TryGetValue("telefono", out int cTel) ||
                    !headers.TryGetValue("mensaje", out int cMsg))
                {
                    throw new InvalidOperationException("Cabeceras requeridas: DNI, TELEFONO, MENSAJE.");
                }

                foreach (var row in ws.RowsUsed().Skip(1)) // desde fila 2
                {
                    var dni = row.Cell(cDni).GetString().Trim();
                    var tel = row.Cell(cTel).GetString().Trim();
                    var msg = row.Cell(cMsg).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(dni) &&
                        string.IsNullOrWhiteSpace(tel) &&
                        string.IsNullOrWhiteSpace(msg))
                        continue;

                    tvp.Rows.Add(dni, tel, msg);
                }
            }

            if (tvp.Rows.Count == 0) throw new InvalidOperationException("El archivo no tiene filas válidas.");

            var cs = _cfg.GetConnectionString("Sql")!;
            using var conn = new SqlConnection(cs);
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand("dbo.Importar_CampaignSMS", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add("@IdCartera", SqlDbType.Int).Value = IdCartera;
            cmd.Parameters.Add("@NombreArchivo", SqlDbType.NVarChar, 255).Value = file.FileName;

            var p = cmd.Parameters.AddWithValue("@Rows", tvp);
            p.SqlDbType = SqlDbType.Structured;
            p.TypeName = "dbo.ESTRUCTURA_SMS";

            using var rdr = await cmd.ExecuteReaderAsync(ct);
            Guid lote = Guid.Empty; int filas = 0;
            if (await rdr.ReadAsync(ct))
            {
                lote = rdr.GetGuid(0);
                filas = rdr.GetInt32(1);
            }

            return new ImportResult { Lote = lote, Filas = filas, IdCartera = IdCartera, Archivo = file.FileName };
        }
    }
}
