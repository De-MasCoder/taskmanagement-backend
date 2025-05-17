using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace TaskEmailService.Models
{
    [Table("Tasks")]
    public class TaskModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }
        [Column("name")]
        public required string Name { get; set; }
        [Column("description")]
        public string? Description { get; set; }
        [Column("due_date")]
        public DateTime DueDate { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("status")]
        public required string Status { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
    }
}
