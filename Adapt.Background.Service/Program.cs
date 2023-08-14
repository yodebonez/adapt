using Adapt.Background.Service.Config;
using Adapt.Background.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static void Main(string[] args)
    {
        IServiceCollection serviceDescriptors = new ServiceCollection();
        Host.CreateDefaultBuilder(args)
           .UseWindowsService()
           .ConfigureHostConfiguration(configHost =>
           {
               configHost.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
           })
           .ConfigureServices((hostContext, services) =>
           {
               var appSettingsConfig = hostContext.Configuration.GetSection(nameof(AppSettings));

               services.AddOptions();
               services.Configure<AppSettings>(appSettingsConfig);
               services.AddSingleton(appSettingsConfig);
               services.AddHostedService<CraeteOrderHandler>();
           }).Build().Run();
    }
}
