using Microsoft.EntityFrameworkCore;
using wt_lab8_8razor_postgresql_makarevich.Data;

var builder = WebApplication.CreateBuilder(args);

// "Ядерный" вариант: собираем строку подключения вручную из переменных окружения
var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "wt_lab8";
var user = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";

string connectionString;

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production" || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DB_HOST")))
{
    // Для Render (облако)
    connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    Console.WriteLine($"=== RENDER MODE: Подключение к {host} ===");
}
else
{
    // Для локальной разработки (берем из appsettings.json)
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    Console.WriteLine("=== LOCAL MODE: Подключение к localhost ===");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddRazorPages();

var app = builder.Build();

// Автоматическое применение миграций при старте
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        Console.WriteLine("=== Попытка подключения к БД и применения миграций... ===");
        db.Database.Migrate();
        Console.WriteLine("=== БАЗА ДАННЫХ УСПЕШНО ПОДКЛЮЧЕНА! ===");
    }
    catch (Exception ex)
    {
        Console.WriteLine("=== ОШИБКА ПОДКЛЮЧЕНИЯ К БД ===");
        Console.WriteLine(ex.ToString());
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

var appPort = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{appPort}");