using CodeMechanic.FileSystem;
using CodeMechanic.Shargs;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;

internal class Program
{
    static async Task Main(string[] args)
    {
        // Load and inject .env files & values
        DotEnv.Load(debug: false);

        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(
                "/logs/justdoit.log",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true
            )
            .CreateLogger();

        var arguments = new ArgsMap(args);
        await RunAsCli(arguments, logger);
    }

    static async Task RunAsCli(ArgsMap arguments, Logger logger)
    {
        var services = CreateServices(arguments, logger);
        Application app = services.GetRequiredService<Application>();
        await app.Run();
    }

    private static ServiceProvider CreateServices(
        ArgsMap arguments
        , Logger logger)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton(arguments)
            .AddSingleton<ILogger>(logger)
            .AddSingleton<Application>()
            .AddScoped<LLamaSamples>()
            .BuildServiceProvider();

        return serviceProvider;
    }
}

internal class Application
{
    private readonly LLamaSamples llamaSamples;

    public Application(LLamaSamples llamaSamples)
    {
        this.llamaSamples = llamaSamples;
    }

    public async Task Run()
    {
        await llamaSamples.Run();
    }
}