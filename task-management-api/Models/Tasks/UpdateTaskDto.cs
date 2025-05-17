using System;

namespace task_management_api.Models.Tasks;

public class UpdateTaskDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public required string Status { get; set; }
    public Guid UserId { get; set; }

}
