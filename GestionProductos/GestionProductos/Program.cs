using GestionProductos;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

// Add services to the container.
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? string.Empty;
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
var dbServer = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
var dbDatabase = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "defaultDB";

connectionString = $"Server={dbServer};Database={dbDatabase};User ID={dbUser};Password={dbPassword};TrustServerCertificate=true;MultipleActiveResultSets=true";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(connectionString));

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

await app.RunAsync();