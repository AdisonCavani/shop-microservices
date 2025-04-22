using Microsoft.EntityFrameworkCore;
using OrderService.Database.Entities;

namespace OrderService.Database;

public class AppDbContext : DbContext
{
    public virtual required DbSet<OrderEntity> Orders { get; set; }
    
    public virtual required DbSet<PaymentEntity> Payments { get; set; }

    public AppDbContext()
    {
        
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresExtension("uuid-ossp");
        
        builder.Entity<OrderEntity>()
            .HasKey(x => x.Id);

        builder.Entity<OrderEntity>()
            .Property(x => x.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("uuid_generate_v4()")
            .IsRequired();

        builder.Entity<OrderEntity>()
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql("current_timestamp at time zone 'utc'");
        
        builder.Entity<PaymentEntity>()
            .HasKey(x => x.StripeCheckoutId);

        builder.Entity<PaymentEntity>()
            .HasOne(x => x.Order)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.OrderId);

        builder.Entity<PaymentEntity>()
            .HasIndex(x => x.OrderId);
    }
}