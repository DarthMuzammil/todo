using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Application.Identity;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Persistence.Configurations;

public class TodoListConfiguration : IEntityTypeConfiguration<TodoList>
{
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {
        builder.ToTable("Lists");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(l => l.Color)
            .IsRequired()
            .HasMaxLength(32);
        builder.Property(l => l.WorkspaceId)
            .IsRequired();

        builder.HasIndex(l => l.OwnerId);
        builder.HasIndex(l => l.WorkspaceId);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(l => l.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne<Workspace>()
            .WithMany()
            .HasForeignKey(l => l.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(l => !l.IsDeleted);
    }
}
