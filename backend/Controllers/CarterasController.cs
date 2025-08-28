using Microsoft.AspNetCore.Mvc;
using prj_import_biznes.Dtos;
using prj_import_biznes.Services.Cartera;

namespace prj_import_biznes.Controllers
{
    [ApiController]
    [Route("api/carteras")]
    public class CarterasController : ControllerBase
    {
        private readonly ICarteraService _svc;
        public CarterasController(ICarteraService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarteraDto>>> List(CancellationToken ct = default)
            => Ok(await _svc.ListAsync(ct));
    
    }
}
