using IfsDashboardApi.Repositories;
using IfsDashboardApi.Repositories.Interfaces;
using IfsDashboardApi.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Controllers + Swagger
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal", p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IIfsRepository, IfsRepository>();
builder.Services.AddScoped<IIfsService, IfsService>();

var app = builder.Build();

// 🔹 Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// 🔹 CORS → BURASI ÇOK ÖNEMLİ
app.UseCors("AllowLocal");

// 🔹 Static dashboard files
app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/ui",
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "IfsDashboardUI"))
});

app.UseAuthorization();

// 🔹 Map controllers
app.MapControllers();

app.Run();
