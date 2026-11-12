
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OzClima.Api.Data;
using OzClima.Api.Exceptions;
using OzClima.Api.Interfaces;
using OzClima.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Entity Framework Core + SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Injeção de Dependência
builder.Services.AddScoped<IRiscoTermicoService, RiscoTermicoService>();
builder.Services.AddScoped<IAreaMonitoradaService, AreaMonitoradaService>();
builder.Services.AddScoped<ILeituraClimaticaService, LeituraClimaticaService>();
builder.Services.AddScoped<IAlertaCalorService, AlertaCalorService>();
builder.Services.AddScoped<IPontoApoioService, PontoApoioService>();

// Tratamento global de exceções
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Tratamento de exceções
app.UseExceptionHandler(new ExceptionHandlerOptions
{
    AllowStatusCode404Response = true
});

// Swagger em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
