namespace prj_import_biznes.DTOs
{
    public class ImportResult
    {
        public Guid Lote { get; set; }
        public int Filas { get; set; }
        public int IdCartera { get; set; }
        public string Archivo { get; set; } = "";
    }
}
