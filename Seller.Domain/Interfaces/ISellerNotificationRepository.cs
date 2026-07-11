using Seller.Domain.Entities;

namespace Seller.Domain.Interfaces;

public interface ISellerNotificationRepository
{
    Task AddAsync(SellerNotification notification, CancellationToken cancellationToken = default);
    Task<bool> ExistsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}