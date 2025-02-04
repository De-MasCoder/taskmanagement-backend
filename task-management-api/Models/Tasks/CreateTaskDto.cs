using System;

namespace task_management_api.Models.Tasks;

public class CreateTaskDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
}
