using MassTransit;
using Serilog;
using Seller.Application.Consumers;

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

// MassTransit (no database for Seller)
builder.Services.AddMassTransit(x =>
{
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

app.UseSerilogRequestLogging();

app.MapControllers();

Log.Information("Seller.API 启动完成，监听 RabbitMQ 队列: seller-order-completed-queue");
app.Run();
