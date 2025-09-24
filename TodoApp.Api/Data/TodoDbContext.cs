using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Models;

namespace TodoApp.Api.Data;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

    public DbSet<TodoItem> Todos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.ToTable("todos");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                  .HasColumnName("id")
                  .ValueGeneratedOnAdd();
                  
            entity.Property(e => e.Title)
                  .HasColumnName("title")
                  .IsRequired()
                  .HasMaxLength(500);
                  
            entity.Property(e => e.IsDone)
                  .HasColumnName("is_done")
                  .HasDefaultValue(false);
                  
            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
                  
            // Indexes untuk performance
            entity.HasIndex(e => e.IsDone).HasDatabaseName("idx_todos_is_done");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("idx_todos_created_at");
        });
    }
}