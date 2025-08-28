using Microsoft.AspNetCore.Mvc;
using prj_import_biznes.DTOs;
using prj_import_biznes.Services.Import;

namespace prj_import_biznes.Controllers
{
    [ApiController]
    [Route("api/import")]
    public class ImportController : ControllerBase
    {
        private readonly IImportServiceFactory _factory;
        public ImportController(IImportServiceFactory factory) => _factory = factory;

        [HttpPost("{channel}")]
        public async Task<ActionResult<ImportResult>> Upload(
        [FromRoute] ChannelType channel,
        [FromForm] ImportRequest req,
        CancellationToken ct)
        {
            var svc = _factory.Resolve(channel);
            var result = await svc.ImportAsync(req.IdCartera, req.File, req.Usuario, ct);
            return Ok(result);
        }
    }
}
