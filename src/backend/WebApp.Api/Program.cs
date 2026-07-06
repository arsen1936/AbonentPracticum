using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using WebApp.Api.Data;
using WebApp.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Reverse proxy support ---
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor
                             | ForwardedHeaders.XForwardedProto
                             | ForwardedHeaders.XForwardedHost
                             | ForwardedHeaders.XForwardedPrefix;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// --- База данных (PostgreSQL) ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Сервисы ---
// ВАЖНО ДЛЯ СТУДЕНТОВ: здесь регистрируются ВСЕ утилиты.
// Чтобы добавить свою — скопируйте строку ниже и замените SumNumbersService на свой класс:
builder.Services.AddSingleton<UtilityOrchestrator>();
builder.Services.AddSingleton<IUtilityService, SumNumbersService>();
builder.Services.AddSingleton<IUtilityService, HashCalculatorService>();
builder.Services.AddSingleton<IUtilityService, NumberConverterService>();
builder.Services.AddSingleton<IUtilityService, TextToListService>();
builder.Services.AddSingleton<IUtilityService, StringSorterService>();
// builder.Services.AddSingleton<IUtilityService, MyNewService>();  // ← пример для новой утилиты

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Internal Utils API",
        Version = "v1",
        Description = "API внутренних утилит компании."
    });
});

// --- CORS (для разработки — разрешаем всё) ---
// В production нужно ограничить конкретными доменами
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers();

var app = builder.Build();

// --- Forwarded headers (должен быть первым middleware) ---
app.UseForwardedHeaders();

// --- Регистрация утилит в оркестраторе ---
var orchestrator = app.Services.GetRequiredService<UtilityOrchestrator>();
var utilityServices = app.Services.GetServices<IUtilityService>();
foreach (var srv in utilityServices)
    orchestrator.Register(srv);

// --- Миграция БД при старте ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Internal Utils API v1");
    c.RoutePrefix = "swagger";
});

app.MapControllers();

// --- Раздача фронтенда в режиме Development ---
// В production фронтенд раздаётся Nginx'ом (docker-compose)
if (app.Environment.IsDevelopment())
{
    var frontendPath = Path.GetFullPath(Path.Combine(
        app.Environment.ContentRootPath, "..", "..", "frontend"));

    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = new PhysicalFileProvider(frontendPath)
    });
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(frontendPath)
    });
}

app.Run();
