using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace prj_import_biznes.Controllers
{
    [ApiController]
    [Route("api/loads")]
    public class LoadsController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        public LoadsController(IConfiguration cfg) => _cfg = cfg;

        [HttpGet("ping")] public IActionResult Ping() => Ok(new { ok = true, at = DateTime.UtcNow });

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? channel, [FromQuery] int? cartera,
                                             [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            try
            {
                var ch = channel?.ToLowerInvariant();
                string sql = ch switch
                {
                    "sms" => @"
                    SELECT 'sms' Canal, s.IdCartera, cr.Nombre Cartera, s.Lote, s.NombreArchivo,
                           COUNT(*) Filas, MIN(s.FechaCarga) MinFecha
                    FROM dbo.CAMPAIGN_SMS s
                    LEFT JOIN dbo.CARTERA_REF cr ON cr.IdCartera = s.IdCartera
                    WHERE (@cartera IS NULL OR s.IdCartera = @cartera)
                      AND (@from IS NULL OR s.FechaCarga >= @from)
                      AND (@to   IS NULL OR s.FechaCarga <= @to)
                    GROUP BY s.IdCartera, cr.Nombre, s.Lote, s.NombreArchivo
                    ORDER BY MinFecha DESC;",
                    "ivr" => @"
                    SELECT 'ivr' Canal, i.IdCartera, cr.Nombre Cartera, i.Lote, i.NombreArchivo,
                           COUNT(*) Filas, MIN(i.FechaCarga) MinFecha
                    FROM dbo.CAMPAIGN_IVR i
                    LEFT JOIN dbo.CARTERA_REF cr ON cr.IdCartera = i.IdCartera
                    WHERE (@cartera IS NULL OR i.IdCartera = @cartera)
                      AND (@from IS NULL OR i.FechaCarga >= @from)
                      AND (@to   IS NULL OR i.FechaCarga <= @to)
                    GROUP BY i.IdCartera, cr.Nombre, i.Lote, i.NombreArchivo
                    ORDER BY MinFecha DESC;",
                    "email" => @"
                    SELECT 'email' Canal, e.IdCartera, cr.Nombre Cartera, e.Lote, e.NombreArchivo,
                           COUNT(*) Filas, MIN(e.FechaCarga) MinFecha
                    FROM dbo.CAMPAIGN_EMAIL e
                    LEFT JOIN dbo.CARTERA_REF cr ON cr.IdCartera = e.IdCartera
                    WHERE (@cartera IS NULL OR e.IdCartera = @cartera)
                      AND (@from IS NULL OR e.FechaCarga >= @from)
                      AND (@to   IS NULL OR e.FechaCarga <= @to)
                    GROUP BY e.IdCartera, cr.Nombre, e.Lote, e.NombreArchivo
                    ORDER BY MinFecha DESC;",
                    "bot" => @"
                    SELECT 'bot' Canal, e.IdCartera, cr.Nombre Cartera, e.Lote, e.NombreArchivo,
                           COUNT(*) Filas, MIN(e.FechaCarga) MinFecha
                    FROM dbo.CAMPAIGN_BOT e
                    LEFT JOIN dbo.CARTERA_REF cr ON cr.IdCartera = e.IdCartera
                    WHERE (@cartera IS NULL OR e.IdCartera = @cartera)
                      AND (@from IS NULL OR e.FechaCarga >= @from)
                      AND (@to   IS NULL OR e.FechaCarga <= @to)
                    GROUP BY e.IdCartera, cr.Nombre, e.Lote, e.NombreArchivo
                    ORDER BY MinFecha DESC;",
                    "wapi" => @"
                    SELECT 'wapi' Canal, e.IdCartera, cr.Nombre Cartera, e.Lote, e.NombreArchivo,
                           COUNT(*) Filas, MIN(e.FechaCarga) MinFecha
                    FROM dbo.CAMPAIGN_WAPI e
                    LEFT JOIN dbo.CARTERA_REF cr ON cr.IdCartera = e.IdCartera
                    WHERE (@cartera IS NULL OR e.IdCartera = @cartera)
                      AND (@from IS NULL OR e.FechaCarga >= @from)
                      AND (@to   IS NULL OR e.FechaCarga <= @to)
                    GROUP BY e.IdCartera, cr.Nombre, e.Lote, e.NombreArchivo
                    ORDER BY MinFecha DESC;",
                    _ => @"
                    SELECT * FROM (
                      SELECT 'sms' Canal, s.IdCartera, cr.Nombre Cartera, s.Lote, s.NombreArchivo,
                             COUNT(*) Filas, MIN(s.FechaCarga) MinFecha
                      FROM dbo.CAMPAIGN_SMS s
                      LEFT JOIN dbo.CARTERA_REF cr ON cr.IdCartera = s.IdCartera
                      WHERE (@cartera IS NULL OR s.IdCartera = @cartera)
                        AND (@from IS NULL OR s.FechaCarga >= @from)
                        AND (@to   IS NULL OR s.FechaCarga <= @to)
                      GROUP BY s.IdCartera, cr.Nombre, s.Lote, s.NombreArchivo

                      UNION ALL
                      SELECT 'ivr', i.IdCartera, cr.Nombre, i.Lote, i.NombreArchivo,
                             COUNT(*), MIN(i.FechaCarga)
                      FROM dbo.CAMPAIGN_IVR i
                      LEFT JOIN dbo.CARTERA_REF cr ON cr.IdCartera = i.IdCartera
                      WHERE (@cartera IS NULL OR i.IdCartera = @cartera)
                        AND (@from IS NULL OR i.FechaCarga >= @from)
                        AND (@to   IS NULL OR i.FechaCarga <= @to)
                      GROUP BY i.IdCartera, cr.Nombre, i.Lote, i.NombreArchivo

                      UNION ALL
                      SELECT 'email', e.IdCartera, cr.Nombre, e.Lote, e.NombreArchivo,
                             COUNT(*), MIN(e.FechaCarga)
                      FROM dbo.CAMPAIGN_EMAIL e
                      LEFT JOIN dbo.CARTERA_REF cr ON cr.IdCartera = e.IdCartera
                      WHERE (@cartera IS NULL OR e.IdCartera = @cartera)
                        AND (@from IS NULL OR e.FechaCarga >= @from)
                        AND (@to   IS NULL OR e.FechaCarga <= @to)
                      GROUP BY e.IdCartera, cr.Nombre, e.Lote, e.NombreArchivo

                        UNION ALL
                      SELECT 'bot', e.IdCartera, cr.Nombre, e.Lote, e.NombreArchivo,
                             COUNT(*), MIN(e.FechaCarga)
                      FROM dbo.CAMPAIGN_BOT e
                      LEFT JOIN dbo.CARTERA_REF cr ON cr.IdCartera = e.IdCartera
                      WHERE (@cartera IS NULL OR e.IdCartera = @cartera)
                        AND (@from IS NULL OR e.FechaCarga >= @from)
                        AND (@to   IS NULL OR e.FechaCarga <= @to)
                      GROUP BY e.IdCartera, cr.Nombre, e.Lote, e.NombreArchivo

                        UNION ALL
                        SELECT 'wapi', e.IdCartera, cr.Nombre, e.Lote, e.NombreArchivo,
                             COUNT(*), MIN(e.FechaCarga)
                        FROM dbo.CAMPAIGN_WAPI e
                        LEFT JOIN dbo.CARTERA_REF cr ON cr.IdCartera = e.IdCartera
                        WHERE (@cartera IS NULL OR e.IdCartera = @cartera)
                        AND (@from IS NULL OR e.FechaCarga >= @from)
                        AND (@to   IS NULL OR e.FechaCarga <= @to)
                        GROUP BY e.IdCartera, cr.Nombre, e.Lote, e.NombreArchivo
                    ) X
                    ORDER BY MinFecha DESC;"
                };

                var cs = _cfg.GetConnectionString("Sql")!;
                using var cn = new Microsoft.Data.SqlClient.SqlConnection(cs);
                await cn.OpenAsync();

                using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@cartera", (object?)cartera ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@from", (object?)from ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@to", (object?)to ?? DBNull.Value);

                using var rd = await cmd.ExecuteReaderAsync();
                var list = new List<object>();
                while (await rd.ReadAsync())
                {
                    list.Add(new
                    {
                        Canal = rd.GetString(0),
                        IdCartera = rd.GetInt32(1),
                        Cartera = rd.IsDBNull(2) ? null : rd.GetString(2),
                        Lote = rd.GetGuid(3),
                        NombreArchivo = rd.IsDBNull(4) ? null : rd.GetString(4),
                        Filas = rd.GetInt32(5),
                        MinFecha = rd.GetDateTime(6)
                    });
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Problem($"Error en /api/loads: {ex.Message}");
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export([FromQuery] string channel, [FromQuery] Guid lote, [FromQuery] string? format = "csv")
        {
            var table = channel.ToLower() switch
            {
                "sms" => "CAMPAIGN_SMS",
                "ivr" => "CAMPAIGN_IVR",
                "email" => "CAMPAIGN_EMAIL",
                "bot" => "CAMPAIGN_BOT",
                "wapi" => "CAMPAIGN_WAPI",
                _ => throw new ArgumentException("Canal inválido")
            };

            var sql = $"SELECT * FROM dbo.{table} WHERE Lote = @lote ORDER BY 1";
            var cs = _cfg.GetConnectionString("Sql")!;
            using var cn = new SqlConnection(cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@lote", lote);

            using var rd = await cmd.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(rd);

            if (string.Equals(format, "xlsx", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(format, "excel", StringComparison.OrdinalIgnoreCase))
            {
                using var wb = new ClosedXML.Excel.XLWorkbook();
                var ws = wb.Worksheets.Add("Datos");
                ws.Cell(1, 1).InsertTable(dt);   // pega con headers
                using var ms = new MemoryStream();
                wb.SaveAs(ms);
                return File(ms.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"{channel}_lote_{lote}.xlsx");
            }
            else
            {
                // CSV fallback
                var sb = new StringBuilder();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append('"').Append(dt.Columns[i].ColumnName.Replace("\"", "\"\"")).Append('"');
                }
                sb.AppendLine();
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (i > 0) sb.Append(',');
                        var val = row[i]?.ToString()?.Replace("\"", "\"\"") ?? "";
                        sb.Append('"').Append(val).Append('"');
                    }
                    sb.AppendLine();
                }
                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                return File(bytes, "text/csv", $"{channel}_lote_{lote}.csv");
            }
        }

    }

}
