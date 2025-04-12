using Gateway.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Database;

public class AppDbContext : DbContext
{
    public required DbSet<UserEntity> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresExtension("uuid-ossp");
        
        builder.Entity<UserEntity>()
            .HasKey(x => x.Id);

        builder.Entity<UserEntity>()
            .Property(x => x.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("uuid_generate_v4()")
            .IsRequired();
        
        builder.Entity<UserEntity>()
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql("current_timestamp at time zone 'utc'");

        builder.Entity<UserEntity>()
            .HasIndex(x => x.Email)
            .IsUnique();
    }
}