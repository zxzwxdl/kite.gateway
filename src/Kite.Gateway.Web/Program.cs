using Kite.Gateway.Web;
using Microsoft.AspNetCore.Http.Features;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

//��־����
builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
          .Enrich.FromLogContext()
          .WriteTo.Console()// ��־���������̨
          .WriteTo.File($"data/logs/log-.txt", restrictedToMinimumLevel: LogEventLevel.Warning, rollingInterval: RollingInterval.Day)
          .MinimumLevel.Information()
          .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
          .CreateLogger();
builder.Host
     .UseSerilog(dispose: true)
     .UseAutofac();


//�����������С
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = int.MaxValue;
});
builder.Services.ReplaceConfiguration(builder.Configuration);//�������ô���

builder.Services.AddApplication<WebModule>();
var app = builder.Build();
app.InitializeApplication();
app.MapGet("/", context => context.Response.WriteAsync("kite gateway run success!"));
app.Run();