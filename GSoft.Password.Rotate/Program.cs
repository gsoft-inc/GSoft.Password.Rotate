using GSoft.Password.Rotate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;

// Setup Host
await Host.CreateDefaultBuilder()
    .ConfigureLogging(HostConfiguration.Logging)
    .ConfigureServices(HostConfiguration.Services)
    .ConfigureAppConfiguration(HostConfiguration.AppConfiguration)
    .RunConsoleAsync();

internal static class HostConfiguration
{
    public static void AppConfiguration(IConfigurationBuilder builder)
    {
        builder.AddJsonFile("appsettings.json");
    }

    public static void Logging(ILoggingBuilder builder)
    {
        builder.ClearProviders();

        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(new ExpressionTemplate(
                "[{@t:HH:mm:ss} {@l:u3} {Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] {@m}\n{@x}",
                theme: TemplateTheme.Literate))
            .CreateLogger();

        Log.Logger = logger;

        builder.AddSerilog(logger, true);
    }

    public static void Services(IServiceCollection services)
    {
        services
            .AddHostedService<JobRunner>()
            .AddOptions<SecretSourceOptions>();
    }
}