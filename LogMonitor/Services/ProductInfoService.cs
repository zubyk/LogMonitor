using System.Reflection;

namespace LogMonitor.Services
{
    public class ProductInfoService
    {
        public string ProductVersion { get; private set; }
        public string ProductName { get; private set; }

        public ProductInfoService()
        {
            var assemblyName = Assembly.GetEntryAssembly()?.GetName();
            ProductVersion = $"{assemblyName?.Version}";
            ProductName = assemblyName?.Name;
        }
    }
}
