using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SCXAuctionGrabber.Domain.DataStructures;
using SCXAuctionGrabber.Domain.Workers;
using SCXAuctionGrabber.IO.ServiceExtensions;
using SCXAuctionGrabber.Model.ServiceExtensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
       services.AddOptions<AppSettings>()
            .Bind(hostContext.Configuration.GetSection("AppSettings"))
            .ValidateDataAnnotations()
            .Validate(config => Directory.Exists(Path.Combine(config.BasePath, config.InputFolder)),
              "Input folder does not exist")
            .ValidateOnStart();

        services.AddStalcraftAuctionServices();
        services.AddIOComponents();

        services.AddHostedService<AuctionWorker>();
    })
    .UseConsoleLifetime()
    .Build();

try
{
    var config = host.Services.GetRequiredService<IConfiguration>();
    var appSettings = config.GetSection("AppSettings").Get<AppSettings>()
        ?? throw new InvalidOperationException("AppSettings configuration is missing");

    var outputPath = Path.Combine(appSettings.BasePath, appSettings.OutputFolder);
    Directory.CreateDirectory(outputPath);

    await host.RunAsync();
}
catch (OptionsValidationException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Configuration validation error:");

    foreach (var failure in ex.Failures)
    {
        Console.WriteLine($" - {failure}");
    }

    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Fatal error during startup: {ex.Message}");
    Console.ResetColor();
}