using Microsoft.AspNetCore.Http;

namespace prj_import_biznes.DTOs
{
    public class ImportRequest
    {
        public int IdCartera { get; set; }
        public IFormFile File { get; set; } = default!;
        public string? Usuario { get; set; }
    }
}
