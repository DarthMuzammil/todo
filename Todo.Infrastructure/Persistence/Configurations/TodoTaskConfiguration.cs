using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Application.Identity;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Persistence.Configurations;

public class TodoTaskConfiguration : IEntityTypeConfiguration<TodoTask>
{
    public void Configure(EntityTypeBuilder<TodoTask> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(t => t.Status)
            .HasConversion<int>();

        builder.Property(t => t.Priority)
            .HasConversion<int>();

        builder.HasIndex(t => t.ListId);

        builder.HasOne<TodoList>()
            .WithMany()
            .HasForeignKey(t => t.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<TodoTask>()
            .WithMany()
            .HasForeignKey(t => t.ParentTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.HasMany<Tag>()
            .WithMany()
            .UsingEntity(
                "TaskTags",
                j => j.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne(typeof(TodoTask)).WithMany().HasForeignKey("TaskId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("TaskId", "TagId");
                    j.ToTable("TaskTags");
                });
    }
}
