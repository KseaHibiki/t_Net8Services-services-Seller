# Seller 微服务

商家通知微服务，采用 DDD 四层架构，负责在订单完成后持久化通知记录。服务本身无 HTTP 端点，仅通过 MassTransit 消费 RabbitMQ 事件，属于纯消息驱动服务。

## 架构

| 层 | 项目 | 职责 |
|---|---|---|
| Domain | `Seller.Domain` | `SellerNotification` 实体、`ISellerNotificationRepository` 接口 |
| Application | `Seller.Application` | `OrderCompletedConsumer`（消费订单完成事件，持久化通知） |
| Infrastructure | `Seller.Infrastructure` | EF Core `SellerDbContext`、`SellerNotificationRepository` 实现、MassTransit Outbox 配置 |
| API | `Seller.API` | 启动入口、DI 注册、Serilog 日志 |

## 技术栈

| 组件 | 技术 |
|------|------|
| 运行时 | .NET 8 (ASP.NET Core) |
| 数据库 | MySQL 8.0 (via Pomelo.EntityFrameworkCore) |
| 消息队列 | MassTransit 8.3.0 + RabbitMQ |
| 发件箱模式 | MassTransit EntityFramework Outbox |
| 日志 | Serilog (控制台 + 按天滚动文件) |
| 部署 | Docker |

## 领域模型

### SellerNotification（商家通知实体）

| 属性 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 通知记录唯一标识 |
| OrderId | Guid | 关联订单 ID（唯一索引） |
| CompletedAt | DateTime | 订单完成时间 |
| NotifiedAt | DateTime | 通知创建时间 |

**工厂方法**: `SellerNotification.Create(orderId, completedAt)`

## API 接口

本服务不暴露 HTTP 接口。所有业务逻辑通过消费 RabbitMQ 事件触发。

## 事件驱动

### 消费事件

| 事件 | 来源 | 消费者 | 行为 |
|------|------|--------|------|
| OrderCompletedEvent | Shop 服务 | OrderCompletedConsumer | 持久化通知记录到 seller_db |

### 发布事件

无（终端消费者）。

### 消息可靠性

- MassTransit **EntityFramework Outbox** 模式（MySQL）
- `AddInboxStateEntity()` 入站消息去重（防止重复通知）
- 队列：Durable=true, AutoDelete=false, PurgeOnStartup=false

## 并发保护

| 机制 | 位置 |
|------|------|
| OrderId 唯一索引 | 数据库层（防止重复通知） |
| MassTransit Inbox | 消息去重 |

## 依赖关系

```
Seller.API
  ├── Seller.Infrastructure
  │     ├── Seller.Application
  │     │     └── Seller.Domain
  │     └── Seller.Domain
  └── shared/Shop.Events
```

## 环境配置

| 配置文件 | 适用环境 | 日志级别 | 数据库连接 |
|----------|:--------:|:--------:|:----------:|
| `appsettings.json` | 基础公共 | — | `localhost:3308` |
| `appsettings.Development.json` | Development | Debug | `localhost:3308` |
| `appsettings.Production.json` | Production | Warning | Docker 内部 (环境变量覆写) |

## 本地运行

```bash
dotnet run --project services/Seller/src/Seller.API/Seller.API.csproj
```

Docker Compose 方式（推荐）：

```bash
docker-compose up -d seller-api
```

> 无 Swagger 页面，无 HTTP 端口。通过 `docker logs seller-api` 或日志文件 `logs/seller-api-YYYYMMDD.log` 查看通知记录。

## 数据库

- 数据库：seller_db（MySQL 8.0，端口 3308）
- 表：seller_notifications, MassTransitInboxState, MassTransitOutboxMessage, MassTransitOutboxState
- ORM：Entity Framework Core（Code-First）
- 启动时自动创建表（`EnsureCreated`）