using AngularBackend.Services.Backgrounds;
using AngularBackend.Services.FileUpload;
using AngularBackend.Services.Setting;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Domain.Repository;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repository.Mediator;
using Repository.Models.Postal;
using Repository.Repositories.Postal;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

// host
// autofac
// how to use .Net6 https://andrewlock.net/exploring-dotnet-6-part-10-new-dependency-injection-features-in-dotnet-6/
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new MediatorModule());
});

// dbContext
var postalConnectionString = builder.Configuration.GetConnectionString("PostalConnection");
builder.Services.AddDbContextFactory<PostaldbContext>(options =>
    options.UseSqlServer(postalConnectionString));

builder.Services.Configure<FileUploadSettings>(builder.Configuration.GetSection("FileUploadSettings"));

// DI
builder.Services.AddTransient<ISettingService, SettingService>();
builder.Services.AddTransient<IFileUploadService, FileUploadService>();
builder.Services.AddTransient<IStoreService, StoreService>();
builder.Services.AddTransient<IImportAddressRepository, ImportAddressRepository>();

builder.Services.AddHostedService<TickerBackgroundService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// 部分クラス宣言を使用して、Program クラスをパブリックにします
/// こうすることで統合テストが可能になります
/// </summary>
public partial class Program{}