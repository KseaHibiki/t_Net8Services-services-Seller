namespace Seller.Domain.Entities;

public class SellerNotification
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public DateTime CompletedAt { get; private set; }
    public DateTime NotifiedAt { get; private set; }

    private SellerNotification() { }

    public static SellerNotification Create(Guid orderId, DateTime completedAt)
    {
        return new SellerNotification
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            CompletedAt = completedAt,
            NotifiedAt = DateTime.UtcNow
        };
    }
}