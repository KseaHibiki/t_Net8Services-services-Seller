using MassTransit;
using Microsoft.Extensions.Logging;
using Shop.Events;

namespace Seller.Application.Consumers;

public class OrderCompletedConsumer : IConsumer<OrderCompletedEvent>
{
    private readonly ILogger<OrderCompletedConsumer> _logger;

    public OrderCompletedConsumer(ILogger<OrderCompletedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var eventMessage = context.Message;

        _logger.LogInformation(
            "[Seller] Order {OrderId} has been completed at {CompletedAt}. " +
            "Notification sent to the merchant.",
            eventMessage.OrderId, eventMessage.CompletedAt);

        Console.WriteLine($"[Seller] === Merchant Notification ===");
        Console.WriteLine($"[Seller] Order {eventMessage.OrderId} completed at {eventMessage.CompletedAt:O}");
        Console.WriteLine($"[Seller] Please prepare the order for shipping.");
        Console.WriteLine($"[Seller] ==============================");

        return Task.CompletedTask;
    }
}