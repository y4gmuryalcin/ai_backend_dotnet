using Microsoft.EntityFrameworkCore;
using ai_backend_dotnet.Data;
using ai_backend_dotnet.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısı (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=messages.db"));

// CORS ayarları — frontend (React) erişebilsin diye
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapGet("/", () => "API çalışıyor 🚀");

app.Run();
