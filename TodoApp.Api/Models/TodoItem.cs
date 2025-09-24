using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Api.Models;

[Table("todos")]
public class TodoItem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    [Column("title")]
    public string Title { get; set; } = default!;
    
    [Column("is_done")]
    public bool IsDone { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
