using MassTransit;
using Serilog;
using Shop.Events;

namespace Seller.Application.Consumers;

public class OrderCompletedConsumer : IConsumer<OrderCompletedEvent>
{
    public Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var evt = context.Message;

        Log.Information("收到订单完成事件: OrderId={OrderId}, CompletedAt={CompletedAt}",
            evt.OrderId, evt.CompletedAt);

        // 模拟商家通知（实际场景：发短信/邮件/推送）
        Log.Information("=== 商家通知 ===");
        Log.Information("订单 {OrderId} 已完成，请准备发货", evt.OrderId);
        Log.Information("完成时间: {CompletedAt:O}", evt.CompletedAt);
        Log.Information("=================");

        return Task.CompletedTask;
    }
}
