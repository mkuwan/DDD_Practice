// See https://aka.ms/new-console-template for more information

using System.Net;
using Autofac.Extensions.DependencyInjection;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleConsole.BackgroundTasks;
using SampleConsole.Infrastructure;
using SampleConsole.Infrastructure.Mediator;
using Serilog;


var configuration = GetConfiguration();
Log.Logger = CreateSerilogLogger(configuration);



var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new MediatorModule());
});

// dbContext
var postalConnectionString = builder.Configuration.GetConnectionString("OrderConnection");
builder.Services.AddDbContextFactory<OrderDbContext>(options =>
    options.UseSqlServer(postalConnectionString));

//builder.Services.Configure<FileUploadSettings>(builder.Configuration.GetSection("FileUploadSettings"));

//// DI
//builder.Services.AddTransient<ISettingService, SettingService>();
//builder.Services.AddTransient<IFileUploadService, FileUploadService>();
//builder.Services.AddTransient<IStoreService, StoreService>();
//builder.Services.AddTransient<IImportAddressRepository, ImportAddressRepository>();

builder.Services.AddHostedService<TickerBackgroundService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

IConfiguration GetConfiguration()
{
    // appsettings.jsonの読み込み
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    var config = builder.Build();

    // Vault設定
    if (config.GetValue<bool>("UseVault", false))
    {
        TokenCredential credential = new ClientSecretCredential(
            config["Vault:TenantId"],
            config["Vault:ClientId"],
            config["Vault:ClientSecret"]);
        builder.AddAzureKeyVault(new Uri($"https://{config["Vault:Name"]}.vault.azure.net/"), credential);
    }

    return builder.Build();
}


// Serilog設定
Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    var seqServerUrl = configuration["Serilog:SeqServerUrl"];
    var logstashUrl = configuration["Serilog:LogstashgUrl"];
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", Program.AppName)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
        .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .CaptureStartupErrors(false)
        .ConfigureKestrel(options =>
        {
            var ports = GetDefinedPorts(configuration);
            options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });

            options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });

        })
        .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
        //.UseStartup<Startup>()
        .UseContentRoot(Directory.GetCurrentDirectory())
        //.UseSerilog()
        .Build();

(int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
{
    var grpcPort = config.GetValue("GRPC_PORT", 5001);
    var port = config.GetValue("PORT", 80);
    return (port, grpcPort);
}


public partial class Program
{
    //public static string Namespace = typeof(Startup).Namespace;
    //public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
    public static string AppName = "SampleConsoleApp";
}

