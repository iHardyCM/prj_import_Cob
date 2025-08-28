using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using prj_import_biznes.DTOs;
using System.Data;
using System.Globalization;
using System.Text;

namespace prj_import_biznes.Services.Import
{
    public class EmailImportService : IChannelImportService
    {
        private readonly IConfiguration _cfg;
        public EmailImportService(IConfiguration cfg) => _cfg = cfg;

        public ChannelType channel => ChannelType.Email;

        public async Task<ImportResult> ImportAsync(int idCartera, IFormFile file, string? usuario, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0) throw new InvalidOperationException("Archivo vacío.");
            if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".xlsx")
                throw new InvalidOperationException("Solo se acepta .xlsx.");

            var tvp = new DataTable();
            tvp.Columns.Add("DNI", typeof(string));
            tvp.Columns.Add("Correo", typeof(string));

            using (var stream = file.OpenReadStream())
            using (var wb = new XLWorkbook(stream))
            {
                var ws = wb.Worksheets.FirstOrDefault() ?? throw new InvalidOperationException("El archivo no tiene hojas.");
                var firstRow = ws.FirstRowUsed() ?? throw new InvalidOperationException("La hoja está vacía.");
                var headerRow = firstRow.RowNumber();

                var headers = ws.Row(headerRow).CellsUsed().ToDictionary(
                    c => Normalize(c.GetString()), c => c.Address.ColumnNumber);

                int cDni = GetHeader(headers, new[] { "dni", "dato1" })
                           ?? throw new InvalidOperationException("Falta cabecera DNI (o Dato1).");
                int cMail = GetHeader(headers, new[] { "correo", "email", "e-mail", "correoelectronico" })
                           ?? throw new InvalidOperationException("Falta cabecera CORREO (correo/email).");

                foreach (var row in ws.RowsUsed().Skip(1))
                {
                    var dni = row.Cell(cDni).GetString().Trim();
                    var mail = row.Cell(cMail).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(dni) || string.IsNullOrWhiteSpace(mail)) continue;

                    tvp.Rows.Add(dni, mail);
                }
            }

            if (tvp.Rows.Count == 0) throw new InvalidOperationException("No se encontraron filas válidas (DNI y CORREO).");

            var cs = _cfg.GetConnectionString("Sql")
                     ?? throw new InvalidOperationException("ConnectionStrings:Sql no configurada.");

            using var conn = new SqlConnection(cs);
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand("dbo.Importar_CampaignEMAIL", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add("@IdCartera", SqlDbType.Int).Value = idCartera;
            cmd.Parameters.Add("@NombreArchivo", SqlDbType.NVarChar, 255).Value = file.FileName;

            var p = cmd.Parameters.AddWithValue("@Rows", tvp);
            p.SqlDbType = SqlDbType.Structured;
            p.TypeName = "dbo.ESTRUCTURA_EMAIL";

            using var rdr = await cmd.ExecuteReaderAsync(ct);
            Guid lote = Guid.Empty; int filas = 0;
            if (await rdr.ReadAsync(ct)) { lote = rdr.GetGuid(0); filas = rdr.GetInt32(1); }

            return new ImportResult { Lote = lote, Filas = filas, IdCartera = idCartera, Archivo = file.FileName };
        }

        // helpers
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
        private static int? GetHeader(Dictionary<string, int> headers, string[] aliases)
        {
            foreach (var a in aliases)
                if (headers.TryGetValue(a, out var col)) return col;
            return null;
        }
    }
}