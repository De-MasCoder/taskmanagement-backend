using System;
using task_management_api.Models;
using task_management_api.Models.Tasks;

namespace task_management_api.Repository.Tasks;

public interface ITaskRepository
{
    Task<IEnumerable<GetTaskResponseDto>> GetAllTasksAsync();
    Task<GetTaskResponseDto> GetTaskByIdAsync(int taskId);
    Task<GetTaskResponseDto> CreateTask(CreateTaskDto task);
    Task<GetTaskResponseDto> UpdateTask(int id,CreateTaskDto task);
    Task DeleteTask(int taskId);

}
