using Microsoft.EntityFrameworkCore;
using ProductService.Database.Entities;

namespace ProductService.Database;

public class AppDbContext : DbContext
{
    public required DbSet<ProductEntity> Products { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // For generating UUID
        builder.HasPostgresExtension("uuid-ossp");
        
        builder.Entity<ProductEntity>()
            .HasKey(x => x.Id);
        
        builder.Entity<ProductEntity>()
            .Property(x => x.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("uuid_generate_v4()")
            .IsRequired();
        
        builder.Entity<ProductEntity>()
            .HasIndex(x => x.ActivationCode)
            .IsUnique();

        builder.Entity<ProductEntity>()
            .Property(x => x.CompletedOrderId)
            .HasColumnType("uuid");
    }
}