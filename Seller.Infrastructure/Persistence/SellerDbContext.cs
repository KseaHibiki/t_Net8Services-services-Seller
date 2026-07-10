using MassTransit;
using Microsoft.EntityFrameworkCore;
using Seller.Domain.Entities;

namespace Seller.Infrastructure.Persistence;

public class SellerDbContext : DbContext
{
    public DbSet<SellerNotification> Notifications => Set<SellerNotification>();

    public SellerDbContext(DbContextOptions<SellerDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        modelBuilder.Entity<SellerNotification>(b =>
        {
            b.ToTable("seller_notifications");
            b.HasKey(n => n.Id);
            b.Property(n => n.OrderId).IsRequired();
            b.Property(n => n.CompletedAt).IsRequired();
            b.Property(n => n.NotifiedAt).IsRequired();
            b.HasIndex(n => n.OrderId).IsUnique();
        });
    }
}
