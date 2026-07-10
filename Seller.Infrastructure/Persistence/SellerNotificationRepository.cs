using Microsoft.EntityFrameworkCore;
using Seller.Domain.Entities;
using Seller.Domain.Interfaces;

namespace Seller.Infrastructure.Persistence;

public class SellerNotificationRepository : ISellerNotificationRepository
{
    private readonly SellerDbContext _context;

    public SellerNotificationRepository(SellerDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SellerNotification notification, CancellationToken cancellationToken = default)
    {
        await _context.Notifications.AddAsync(notification, cancellationToken);
    }

    public async Task<bool> ExistsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications.AnyAsync(n => n.OrderId == orderId, cancellationToken);
    }
}
