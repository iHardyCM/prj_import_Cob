using prj_import_biznes.DTOs;
using Microsoft.AspNetCore.Http;

namespace prj_import_biznes.Services.Import
{
    public interface IChannelImportService
    {
        ChannelType channel { get; }

        Task<ImportResult> ImportAsync(int IdCartera, IFormFile file, string? usuario, CancellationToken ct = default);

    }
}
