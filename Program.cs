using Microsoft.EntityFrameworkCore;
using wt_lab8_8razor_postgresql_makarevich.Data;

var builder = WebApplication.CreateBuilder(args);

// Подключение к PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"=== Строка подключения: {(string.IsNullOrEmpty(connectionString) ? "ПУСТАЯ!" : "ЕСТЬ")} ===");

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
        // Не падаем, а продолжаем работу (чтобы сайт запустился и показал ошибку)
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

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");