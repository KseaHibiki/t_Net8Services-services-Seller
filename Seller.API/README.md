# Seller.API — 商家通知微服务

商家通知微服务，负责在订单完成后通知商家准备发货。服务本身无 HTTP 端点，仅通过 MassTransit 消费 RabbitMQ 事件，属于纯消息驱动服务。

## 架构

| 层 | 项目 | 职责 |
|---|---|---|
| Domain | Seller.Domain | 预留领域层（当前无实体，可扩展 `Notification` 模型） |
| Application | Seller.Application | 事件消费者（`OrderCompletedConsumer`） |
| Infrastructure | Seller.Infrastructure | 预留基础设施层（当前无数据库，可扩展通知日志仓储） |
| API | Seller.API | 启动入口、MassTransit 配置、Serilog 日志 |

## 技术栈

- **框架**: ASP.NET Core 8
- **消息队列**: MassTransit + RabbitMQ
- **日志**: Serilog（控制台 + 按天滚动文件）
- **数据库**: 无（纯消费者）

## API 接口

本服务不暴露 HTTP 接口。所有业务逻辑通过消费 RabbitMQ 事件触发。

## 消费的事件

### OrderCompletedEvent

订单完成事件，由 Shop.API 在订单状态变为 `Completed` 时发布。

**事件字段**

| 字段 | 类型 | 说明 |
|---|---|---|
| OrderId | guid | 完成的订单 ID |
| CompletedAt | datetime | 完成时间 |

**消费行为**

收到事件后，Seller.API 通过 Serilog 输出商家通知日志（实际场景可扩展为短信、邮件、App 推送）。

**日志输出示例**

```
[15:04:35 INF] Seller.API | 收到订单完成事件: OrderId=c4b4463f-..., CompletedAt=2026-07-10T07:04:35.0000000Z
[15:04:35 INF] Seller.API | === 商家通知 ===
[15:04:35 INF] Seller.API | 订单 c4b4463f-... 已完成，请准备发货
[15:04:35 INF] Seller.API | 完成时间: 2026-07-10T15:04:35.0000000+08:00
[15:04:35 INF] Seller.API | =================
```

## 监听队列

| RabbitMQ 队列名 | 消费者 | 说明 |
|---|---|---|
| `seller-order-completed-queue` | `OrderCompletedConsumer` | 监听订单完成事件 |

## 本地运行

```bash
# 仅依赖 RabbitMQ (5672)
dotnet run --project services/Seller/src/Seller.API/Seller.API.csproj
```

> 无 Swagger 页面，无 HTTP 端口。通过 `docker logs seller-api` 或日志文件 `logs/seller-api-YYYYMMDD.log` 查看通知记录。