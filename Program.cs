using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using TodoApi.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add persistent database context
builder.Services.AddDbContext<TodoContext>(options =>
{
    var connectionString = BuildConnectionString(builder.Configuration);
    Console.WriteLine($"ConnectionString: {connectionString}");
    options.UseNpgsql(connectionString);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS configuration here
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder
                .WithOrigins("https://frontheroes.azurewebsites.net")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// app.Environment.IsDevelopment()
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHsts();

  


// Use CORS here
app.UseCors("AllowSpecificOrigin");

// Explicitly specify the HTTPS port for redirection
// app.Use((context, next) =>
// {
//     if (!context.Request.IsHttps)
//     {
//         var httpsUrl = "https://" + context.Request.Host + context.Request.Path;
//         context.Response.Redirect(httpsUrl);
//         return Task.CompletedTask;
//     }
//     return next();
// });

app.UseAuthorization();

app.MapControllers();

app.Run();

string BuildConnectionString(IConfiguration configuration)
{
    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    IConfigurationBuilder builder = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

    if (environmentName?.ToLower() == "docker")
    {
        builder.AddJsonFile("appsettings.docker.json", optional: true, reloadOnChange: true);
    }

    var config = builder.Build();

    // Utiliser la chaîne de connexion du fichier de configuration
    var connectionString = config.GetConnectionString("TodoDbConnection");

    return connectionString;
}
