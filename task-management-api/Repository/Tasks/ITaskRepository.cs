using System;
using task_management_api.Models;
using task_management_api.Models.Tasks;

namespace task_management_api.Repository.Tasks;

public interface ITaskRepository
{
    Task<IEnumerable<GetTaskResponseDto>> GetAllTasksAsync();
    Task<GetTaskResponseDto> GetTaskByIdAsync(Guid taskId);
    Task<GetTaskResponseDto> CreateTask(CreateTaskDto task);
    Task<GetTaskResponseDto> UpdateTask(Guid id,UpdateTaskDto task);
    Task DeleteTask(Guid taskId);
    Task<IEnumerable<GetTaskResponseDto>> GetTasksByUserIdAsync(Guid userId);

}
