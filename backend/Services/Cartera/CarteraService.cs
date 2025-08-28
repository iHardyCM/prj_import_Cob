using Microsoft.Data.SqlClient;
using prj_import_biznes.Dtos;

namespace prj_import_biznes.Services.Cartera
{
    public class CarteraService : ICarteraService
    {
        private readonly IConfiguration _cfg;
        public CarteraService(IConfiguration cfg) => _cfg = cfg;
     
        public async Task<IReadOnlyList<CarteraDto>> ListAsync(CancellationToken ct = default)
        {
            var list = new List<CarteraDto>();
            var cs = _cfg.GetConnectionString("Sql") ?? throw new InvalidOperationException("Conn Sql no configurada.");
            using var cn = new SqlConnection(cs);
            await cn.OpenAsync(ct);

            // Ajusta el SELECT a tu fuente real (CARTERA_REF o la vista contra siscob)
            using var cmd = new SqlCommand(@"
            SELECT IdCartera, Nombre
            FROM dbo.CARTERA_REF
            WHERE Activa=1
            ORDER BY Nombre;", cn);

            using var rd = await cmd.ExecuteReaderAsync(ct);
            while (await rd.ReadAsync(ct))
                list.Add(new CarteraDto(rd.GetInt32(0), rd.GetString(1)));
            return list;
        }
    }
}
