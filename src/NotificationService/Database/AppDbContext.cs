using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationService.Database.Entities;

namespace NotificationService.Database;

public class AppDbContext : DbContext
{
    public virtual DbSet<NotificationTriggerEntity> NotificationTriggers { get; set; }
    
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
        
        builder.Entity<NotificationTriggerEntity>()
            .HasKey(x => x.TriggerName);

        builder.Entity<NotificationTriggerEntity>()
            .Property(x => x.TriggerName)
            .IsRequired();
        
        builder.Entity<NotificationTriggerEntity>()
            .Property(x => x.Subject)
            .IsRequired();
        
        builder.Entity<NotificationTriggerEntity>()
            .Property(x => x.LiquidTemplate)
            .IsRequired();
    }
}