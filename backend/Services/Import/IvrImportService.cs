using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http; // para IFormFile
using prj_import_biznes.DTOs;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace prj_import_biznes.Services.Import
{
    public class IvrImportService : IChannelImportService
    {
        private readonly IConfiguration _cfg;
        public IvrImportService(IConfiguration cfg) => _cfg = cfg;

        public ChannelType channel => ChannelType.Ivr;

        public async Task<ImportResult> ImportAsync(int idCartera, IFormFile file, string? usuario, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0) throw new InvalidOperationException("Archivo vacío.");
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".xlsx") throw new InvalidOperationException("Solo se acepta .xlsx.");

            // TVP: DNI, Telefono, Nombre
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
                    throw new InvalidOperationException("Cabeceras requeridas: DNI, TELEFONO, NOMBRE.");

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

            if (tvp.Rows.Count == 0) throw new InvalidOperationException("No se encontraron datos válidos en el archivo.");

            var cs = _cfg.GetConnectionString("Sql")
                 ?? throw new InvalidOperationException("ConnectionStrings:Sql no configurada.");

            using var conn = new SqlConnection(cs);
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand("dbo.Importar_CampaignIVR", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add("@IdCartera", SqlDbType.Int).Value = idCartera;
            cmd.Parameters.Add("@NombreArchivo", SqlDbType.NVarChar, 255).Value = file.FileName;

            var p = cmd.Parameters.AddWithValue("@Rows", tvp);
            p.SqlDbType = SqlDbType.Structured;
            p.TypeName = "dbo.ESTRUCTURA_IVR";

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
