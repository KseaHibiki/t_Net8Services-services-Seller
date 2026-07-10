# Seller.API — 商家通知微服务

商家通知微服务，负责在订单完成后通知商家准备发货。服务本身无 HTTP 端点，仅通过 MassTransit 消费 RabbitMQ 事件，属于纯消息驱动服务。

## 架构

| 层 | 项目 | 职责 |
|---|---|---|
| Domain | Seller.Domain | 预留领域层（当前无实体，可扩展 `Notification` 模型） |
| Application | Seller.Application | 事件消费者（`OrderCompletedConsumer`） |
| Infrastructure | Seller.Infrastructure | 预留基础设施层（当前无数据库，可扩展通知日志仓储） |
| API | Seller.API | 启动入口、MassTransit 配置 |

## 技术栈

- **框架**: ASP.NET Core 8
- **消息队列**: MassTransit + RabbitMQ
- **数据库**: 无（纯消费者）

## 事件参与

| 事件 | 方向 | 说明 |
|---|---|---|
| `OrderCompletedEvent` | 消费 | 订单完成后接收，模拟商家发货通知 |

## 本地运行

```bash
# 仅依赖 RabbitMQ (5672)
dotnet run --project services/Seller/src/Seller.API/Seller.API.csproj
```

> 无 HTTP 端口，通过 `docker logs seller-api` 查看日志确认通知是否发出。
