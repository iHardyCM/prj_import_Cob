using Microsoft.Extensions.DependencyInjection;


namespace prj_import_biznes.Services.Import
{
    public interface IImportServiceFactory
    {
        IChannelImportService Resolve(ChannelType channel);
    }
    public class ImportServiceFactory : IImportServiceFactory
    {
        private readonly IEnumerable<IChannelImportService> _services;
        public ImportServiceFactory(IEnumerable<IChannelImportService> services) => _services = services;
        public IChannelImportService Resolve(ChannelType channel) =>
            _services.First(s => s.channel == channel);
    }
}
