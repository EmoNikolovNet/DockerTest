using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();



builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    //// Add support for XML comments
    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //options.IncludeXmlComments(xmlPath);
});

Log.Logger = new LoggerConfiguration()
       .Enrich.WithMachineName()
       .Enrich.WithThreadId()
       .Enrich.FromLogContext() // Allows properties to be logged
       .WriteTo.Console()
       .CreateLogger();

// Add Serilog to ASP.NET Core logging
builder.Host.UseSerilog();


builder.Services.AddOpenTelemetry().UseAzureMonitor();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
    options.RoutePrefix = string.Empty; // Set the Swagger UI at the root URL
});

app.UseSerilogRequestLogging(); // Logs HTTP requests

app.MapControllers();

app.Run();
