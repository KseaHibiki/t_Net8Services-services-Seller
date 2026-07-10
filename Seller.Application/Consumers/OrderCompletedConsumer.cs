using MassTransit;
using Serilog;
using Shop.Events;
using Seller.Domain.Entities;
using Seller.Domain.Interfaces;

namespace Seller.Application.Consumers;

public class OrderCompletedConsumer : IConsumer<OrderCompletedEvent>
{
    private readonly ISellerNotificationRepository _notificationRepository;

    public OrderCompletedConsumer(ISellerNotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var evt = context.Message;

        Log.Information("收到订单完成事件: OrderId={OrderId}, CompletedAt={CompletedAt}",
            evt.OrderId, evt.CompletedAt);

        var notification = SellerNotification.Create(evt.OrderId, evt.CompletedAt);
        await _notificationRepository.AddAsync(notification, context.CancellationToken);

        Log.Information("卖家通知已持久化: NotificationId={NotificationId}, OrderId={OrderId}",
            notification.Id, evt.OrderId);

        Log.Information("=== 商家通知 ===");
        Log.Information("订单 {OrderId} 已完成，请准备发货", evt.OrderId);
        Log.Information("完成时间: {CompletedAt:O}", evt.CompletedAt);
        Log.Information("=================");
    }
}
