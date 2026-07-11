using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Seller.Application.Consumers;
using Seller.Domain.Interfaces;
using Seller.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "Seller.API")
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Service} | {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/seller-api-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate:
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Service} | {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

// Database
var connectionString = builder.Configuration.GetConnectionString("seller")
    ?? "Server=localhost;Port=3308;Database=seller_db;User=root;Password=114514;";
builder.Services.AddDbContext<SellerDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Repositories
builder.Services.AddScoped<ISellerNotificationRepository, SellerNotificationRepository>();

// MassTransit with Transactional Outbox
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<SellerDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(1);
        o.UseMySql();
    });

    x.AddConsumer<OrderCompletedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq")
            ?? "amqp://guest:guest@localhost:5672/");

        cfg.ReceiveEndpoint("seller-order-completed-queue", e =>
        {
            e.ConfigureConsumer<OrderCompletedConsumer>(context);
        });
    });
});

var app = builder.Build();

// Auto-migrate
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SellerDbContext>();
    db.Database.EnsureCreated();
    Log.Information("数据库初始化完成 (seller_db)");
}

app.UseSerilogRequestLogging();

app.MapControllers();

Log.Information("Seller.API 启动完成，监听 RabbitMQ 队列: seller-order-completed-queue");
app.Run();
