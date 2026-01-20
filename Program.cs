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
builder.Services.AddScoped<IfsDashboardApi.Services.IfsService>();

var app = builder.Build();

// 🔹 Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// 🔹 CORS → BURASI ÇOK ÖNEMLİ
app.UseCors("AllowLocal");

// 🔹 Middleware
// app.UseHttpsRedirection();

app.UseAuthorization();

// 🔹 Map controllers
app.MapControllers();

app.Run();
