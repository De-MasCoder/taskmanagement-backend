using System;

namespace task_management_api.Models.Tasks;

public class GetTaskResponseDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; } 
    public required string Status { get; set; }
    public int UserId { get; set; }
}
