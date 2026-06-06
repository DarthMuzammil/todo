using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Application.Identity;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Persistence.Configurations;

public class ActivityEntryConfiguration : IEntityTypeConfiguration<ActivityEntry>
{
    public void Configure(EntityTypeBuilder<ActivityEntry> builder)
    {
        builder.ToTable("ActivityEntries");

        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.TaskTitle)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(entry => entry.ListTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(entry => entry.Action)
            .HasConversion<int>();

        builder.HasIndex(entry => entry.ListId);
        builder.HasIndex(entry => entry.WorkspaceId);
        builder.HasIndex(entry => entry.CreatedAt);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(entry => entry.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
