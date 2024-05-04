using FluentValidation;
using JixMinApi.Features.Todo;
using JixMinApi.Shared;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(new CompactJsonFormatter(), "Logs/applog-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

Log.Information("App Starting up ===================");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "JixMinApi Demo",
            Description = "A simple minimal API Demo that uses vertical slice architecture.",
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

    builder.Services.AddTodoEndpointServices();
    builder.Services.AddHealthChecks();

    var app = builder.Build();
    app.UseHttpsRedirection();

    if (!app.Environment.IsDevelopment())
    {
        app.UseStatusCodePages();
        app.UseExceptionHandler();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    app.UseTodoEndpoints();
    app.MapHealthChecks("/healthz");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "App terminated unexpectedly ===================");
}
finally
{
    Log.CloseAndFlush();
}


