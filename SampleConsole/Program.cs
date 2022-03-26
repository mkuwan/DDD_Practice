// See https://aka.ms/new-console-template for more information

using System.Net;
using Autofac.Extensions.DependencyInjection;
using Azure.Core;
using Azure.Identity;
using BenchmarkDotNet.Running;
using Castle.DynamicProxy;
using FluentValidation;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleConsole.Application.Behaviors;
using SampleConsole.BackgroundTasks;
using SampleConsole.Benchmarks;
using SampleConsole.Domain.Repositories;
using SampleConsole.Infrastructure;
using SampleConsole.Infrastructure.Mediator;
using SampleConsole.Infrastructure.Repositories;
using Serilog;


var configuration = GetConfiguration();

Log.Logger = CreateSerilogLogger(configuration);


var builder = WebApplication.CreateBuilder(args);
// Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// MediatR
builder.Services.AddMediatR(typeof(Program));

// FluentValidation
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// dbContext
var orderConnectionString = builder.Configuration.GetConnectionString("SampleConnection");
builder.Services.AddDbContextFactory<SampleDbContext>(options =>
    options.UseSqlServer(orderConnectionString));


//builder.Services.Configure<FileUploadSettings>(builder.Configuration.GetSection("FileUploadSettings"));

// DI
builder.Services.AddTransient<IScheduleRepository, ScheduleRepository>();

builder.Services.AddHostedService<TickerBackgroundService>();

builder.Services.AddControllers();
builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.UseStaticFiles();
//app.MapGet("/", () => "Hello Sample Console");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

//BenchmarkRunner.Run<DateParserBenchmarks>();
app.Run();



// app.Run() ではなくHostを使用するパターン
// https://andrewlock.net/exploring-dotnet-6-part-2-comparing-webapplicationbuilder-to-the-generic-host/
//var hostBuilder = Host.CreateDefaultBuilder(args)
//    .ConfigureServices(services =>
//    {
//        services.AddAutofac();
//    })
//    .ConfigureWebHostDefaults(webBuilder =>
//    {
//        webBuilder.Configure((ctx, app) =>
//        {
//            if (ctx.HostingEnvironment.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }

//            app.UseStaticFiles();
//            app.UseRouting();
//            app.UseEndpoints(endPoints =>
//            {
//                endPoints.MapGet("/", () => "Hello");
//                //endPoints.MapRazorPages();
//            });
//        });
//    })
//    .ConfigureHostOptions(options =>
//    {

//    });
//hostBuilder.Build().Run();


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

IHost BuildHost(IConfiguration configuration, string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {

        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureKestrel(options =>
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
            });

            webBuilder.Configure((ctx, app) =>
            {
                app.UseStaticFiles();
                app.UseRouting();
                app.UseEndpoints(endPoints =>
                {
                    endPoints.MapGet("/", () => "Hello by Host");
                });

            });
        })
        .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
        .UseContentRoot(Directory.GetCurrentDirectory())
        .Build();

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

