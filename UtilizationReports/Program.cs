using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;
using NLog;
using NLog.Web;
using UtilizationReports;
using UtilizationReports.Middlewares;
using Microsoft.OpenApi.Models;

var logger = NLog.LogManager.Setup()
	.LoadConfigurationFromAppSettings()
	.GetCurrentClassLogger();

try
{
	logger.Debug("init main");
	var builder = WebApplication.CreateBuilder(args);

	// Add services to the container.
	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	builder.Services.AddControllers();

	// To display enum values as strings in the response
	builder.Services
		.AddControllers()
		.AddJsonOptions(options =>
			options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen(c =>
	{
		c.SwaggerDoc("v1", new OpenApiInfo 
		{ 
			Title = "UtilizationReports", 
			Version = "v1",
			Description = "API to generate utilization reports for a given account.",
		});
	});

	// To cache the results of the API calls
    builder.Services.AddMemoryCache(options =>
        new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(DateTimeOffset.Now.AddMinutes(10)));
	// To make HTTP requests
	builder.Services.AddHttpClient();

	// NLog: Setup NLog for Dependency injection
	builder.Logging.ClearProviders();
	builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
	builder.Host.UseNLog();

	// Registering services and options with the dependency injection container
	builder.Services
		.AddConfig(builder.Configuration)
		.AddDependencyGroup();

	var app = builder.Build();

	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}
	
	// So that the Swagger UI is available in production
	// To invoke the Swagger UI, go to https://<host>/swagger or https://<host>/swagger/index.html
	if (app.Environment.IsProduction())
	{
		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "UtilizationReports v1");
			c.RoutePrefix = "swagger";
		});
	}
	app.UseRouting();

	app.UseHttpsRedirection();
	// Adding middleware to the pipeline to handle exceptions
	app.UseCustomExceptionHandler();
	app.MapControllers();
	app.Run();
}
catch (Exception ex)
{
	logger.Error(ex, "Stopped program because of exception");
	throw;
}
finally
{
	NLog.LogManager.Shutdown();
}

// For Integration Tests
public partial class Program 
{
	protected Program() { }
}