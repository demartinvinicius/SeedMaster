// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewSeederTester.Data;
using NLog.Extensions.Logging;
using Nudes.SeedMaster;
using Nudes.SeedMaster.Interfaces;
using Nudes.SeedMaster.Seeder;
using System.Reflection;
using static Nudes.SeedMaster.SeedScanner;

IConfiguration myconfig = null;
ILoggerFactory loggerFactory = LoggerFactory.Create(x =>
{
    x.AddConsole();
});
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configHost =>
    {
        configHost.AddJsonFile("appsettings.json");
        myconfig = configHost.Build();
        NLog.LogManager.Configuration = new NLogLoggingConfiguration(myconfig.GetSection("NLog"));
    })
    .ConfigureServices(services =>
    {
        services.AddLogging(x => x.AddConsole());
        services.AddSingleton(loggerFactory);

        services.AddDbContext<ContextToSeed>(config =>
        config.UseSqlServer(myconfig.GetConnectionString("MainConnection")));
        services.AddScoped<DbContext>(provider => provider.GetService<ContextToSeed>());
        services.AddScoped<IEnumerable<ScanResult>>(provider => SeedScanner.GetSeeds(Assembly.GetExecutingAssembly()));

        services.AddScoped<ISeeder, EfCoreSeeder>();
    }).Build();

var coreSeeder = host.Services.GetService<ISeeder>();
coreSeeder.Seed();

//host.RunAsync();

//logger.LogInformation(host.Services.GetService<ISeeder>().ToString());

Console.WriteLine("Hello, World!");