using demo_docker.Extension;
using demo_docker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using demo_docker.Endpoints;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(op => op.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddStackExchangeRedisCache(op => op.Configuration = builder.Configuration.GetConnectionString("Cache"));
var app = builder.Build();

app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.ApplyMigration();
}

app.UseHttpsRedirection();
app.AddEndpoints();
app.MapGet("/all", () =>
{
    return Results.Ok(new { message="users are valid" });
});

app.Run();

