using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace task_management_api.Models.Tasks;

[Table("Tasks")]
public class TaskModel : BaseModel
{
    [PrimaryKey("id",false)]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("description")]
    public string Description { get; set; }
    [Column("due_date")]
    public DateTime DueDate { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
