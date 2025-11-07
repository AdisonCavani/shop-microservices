using Gateway.Database.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProtobufSpec;

namespace Gateway.Database;

public class AppDbContext : DbContext
{
    public virtual DbSet<UserEntity> Users { get; set; }
    
    public AppDbContext()
    {
        
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.AddInboxStateEntity();
        builder.AddOutboxStateEntity();
        builder.AddOutboxMessageEntity();
        
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
        
        builder.HasPostgresEnum<UserRoleEnum>(null, nameof(UserRoleEnum));

        builder.Entity<UserEntity>()
            .Property(x => x.UserRole)
            .HasColumnType($"\"{nameof(UserRoleEnum)}\"")
            .HasDefaultValueSql($"'{UserRoleEnum.User.ToString().ToLower()}'");
    }
}