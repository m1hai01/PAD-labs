using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using Serilog.Exceptions;
using System;

namespace UserServer.Extensions;

public static class LogginExtensions
{
    public static void AddLogging(this WebApplicationBuilder _)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{environment}.json",
                optional: true)
            .Build();

        Console.WriteLine(configuration);

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
            .Enrich.WithProperty("Environment", environment)
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    private static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string environment)
    {
        return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]!))
        {
            AutoRegisterTemplate = true,
            IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
            ModifyConnectionSettings = x => x.BasicAuthentication(
                configuration["ElasticConfiguration:Username"],
                configuration["ElasticConfiguration:Password"])
        };
    }
}