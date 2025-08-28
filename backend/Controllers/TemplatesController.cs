using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;

namespace prj_import_biznes.Controllers
{
    [ApiController]
    [Route("api/templates")]
    public class TemplatesController : ControllerBase
    {
        [HttpGet("import")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult ImportTemplate([FromQuery] string channel = "sms")
        {
            string[] headers = channel.ToLower() switch
            {
                "sms" => new[] { "DNI", "TELEFONO", "MENSAJE"},
                "ivr" => new[] { "DNI", "TELEFONO", "NOMBRE" },
                "email" => new[] { "DNI", "CORREO" },
                "wapi" => new[] { "DNI", "TELEFONO", "MENSAJE" },
                "bot" => new[] { "DNI", "TELEFONO", "NOMBRE" },
                _ => new[] { "DNI", "TELEFONO", "MENSAJE" },
            };

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Plantilla");
            for (int i = 0; i < headers.Length; i++)
                ws.Cell(1, i + 1).Value = headers[i];

            ws.Range(1, 1, 1, headers.Length).Style
            .Font.SetBold()
            .Fill.SetBackgroundColor(XLColor.LightGray);
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            var bytes = ms.ToArray();
            return File(bytes,
              "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
              $"plantilla_{channel}.xlsx");
        }
    }
}
