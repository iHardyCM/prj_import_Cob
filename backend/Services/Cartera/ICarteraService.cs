using prj_import_biznes.Dtos;

namespace prj_import_biznes.Services.Cartera;
public interface ICarteraService
{
    Task<IReadOnlyList<CarteraDto>> ListAsync(CancellationToken ct = default);
}
