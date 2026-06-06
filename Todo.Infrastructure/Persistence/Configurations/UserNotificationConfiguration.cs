using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Application.Identity;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Persistence.Configurations;

public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder.ToTable("UserNotifications");

        builder.HasKey(notification => notification.Id);

        builder.Property(notification => notification.Message)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(notification => new { notification.UserId, notification.IsRead });
        builder.HasIndex(notification => notification.CreatedAt);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(notification => notification.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ActivityEntry>()
            .WithMany()
            .HasForeignKey(notification => notification.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
