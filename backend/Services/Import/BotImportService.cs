using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using prj_import_biznes.DTOs;
using System.Data;
using System.Globalization;
using System.Text;

namespace prj_import_biznes.Services.Import
{
    public class BotImportService : IChannelImportService
    {
        private readonly IConfiguration _cfg;
        public BotImportService(IConfiguration cfg) => _cfg = cfg;
        public ChannelType channel => ChannelType.Bot;

        public async Task<ImportResult> ImportAsync(int idCartera, IFormFile file, string? usuario, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0) throw new InvalidOperationException("Archivo vacío.");
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".xlsx") throw new InvalidOperationException("Solo .xlsx.");

            var tvp = new DataTable();
            tvp.Columns.Add("DNI", typeof(string));
            tvp.Columns.Add("Telefono", typeof(string));
            tvp.Columns.Add("Nombre", typeof(string));

            using (var stream = file.OpenReadStream())
            using (var wb = new XLWorkbook(stream))
            {
                var ws = wb.Worksheets.FirstOrDefault() ?? throw new InvalidOperationException("El archivo no tiene hojas.");
                var firstRow = ws.FirstRowUsed() ?? throw new InvalidOperationException("La hoja está vacía.");
                var headerRow = firstRow.RowNumber();

                var headers = ws.Row(headerRow).CellsUsed().ToDictionary(
                  c => Normalize(c.GetString()), c => c.Address.ColumnNumber);

                if (!headers.TryGetValue("dni", out int cDni) ||
                    !headers.TryGetValue("telefono", out int cTel) ||
                    !headers.TryGetValue("nombre", out int cNom))
                    throw new InvalidOperationException("Cabeceras: DNI, TELEFONO, NOMBRE.");

                foreach (var row in ws.RowsUsed().Skip(1))
                {
                    var dni = row.Cell(cDni).GetString().Trim();
                    var tel = row.Cell(cTel).GetString().Trim();
                    var nom = row.Cell(cNom).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(dni) &&
                        string.IsNullOrWhiteSpace(tel) &&
                        string.IsNullOrWhiteSpace(nom)) continue;
                    tvp.Rows.Add(dni, tel, nom);
                }
            }

            if (tvp.Rows.Count == 0) throw new InvalidOperationException("Sin datos válidos.");

            var cs = _cfg.GetConnectionString("Sql") ?? throw new InvalidOperationException("ConnectionStrings:Sql no configurada.");
            using var conn = new SqlConnection(cs);
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand("dbo.Importar_CampaignBOT", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add("@IdCartera", SqlDbType.Int).Value = idCartera;
            cmd.Parameters.Add("@NombreArchivo", SqlDbType.NVarChar, 255).Value = file.FileName;
            cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value = (object?)usuario ?? DBNull.Value;

            var p = cmd.Parameters.AddWithValue("@Rows", tvp);
            p.SqlDbType = SqlDbType.Structured;
            p.TypeName = "dbo.ESTRUCTURA_BOT";

            using var rdr = await cmd.ExecuteReaderAsync(ct);
            Guid lote = Guid.Empty; int filas = 0;
            if (await rdr.ReadAsync(ct)) { lote = rdr.GetGuid(0); filas = rdr.GetInt32(1); }

            return new ImportResult { Lote = lote, Filas = filas, IdCartera = idCartera, Archivo = file.FileName };
        }

        private static string Normalize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var t = s.Trim().ToLowerInvariant().Replace(" ", "").Replace("_", "");
            return RemoveDiacritics(t);
        }

        private static string RemoveDiacritics(string text)
        {
            var norm = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(norm.Length);
            foreach (var ch in norm)
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }


    }
}
