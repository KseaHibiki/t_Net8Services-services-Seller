using MassTransit;
using Seller.Application.Consumers;

var builder = WebApplication.CreateBuilder(args);

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

app.MapControllers();
app.Run();