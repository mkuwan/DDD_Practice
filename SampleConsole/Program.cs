// See https://aka.ms/new-console-template for more information
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SampleConsole.BackgroundTasks;
using SampleConsole.Infrastructure.Mediator;


//Console.WriteLine("Hello, World!");
//Console.ReadLine();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new MediatorModule());
});

//// dbContext
//var postalConnectionString = builder.Configuration.GetConnectionString("PostalConnection");
//builder.Services.AddDbContextFactory<PostaldbContext>(options =>
//    options.UseSqlServer(postalConnectionString));

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

public partial class Program { }