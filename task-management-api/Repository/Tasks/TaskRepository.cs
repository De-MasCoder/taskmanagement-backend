using System;

namespace task_management_api.Repository.Tasks;
using task_management_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supabase.Interfaces;
using task_management_api.Models.Tasks;
using task_management_api.Enums;

public class TaskRepository : ITaskRepository
{
    private  readonly Supabase.Client _supabaseClient;

    public TaskRepository(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<IEnumerable<GetTaskResponseDto>> GetAllTasksAsync()
    {
        var response = await _supabaseClient.From<TaskModel>().Get();

        if (response == null)
        {
            return Enumerable.Empty<GetTaskResponseDto>();
        }

        var tasks = response.Models.Select(t => new GetTaskResponseDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            DueDate = t.DueDate,
            CreatedAt = t.CreatedAt,
            Status = t.Status,
            UserId = t.UserId
        });
        
        return tasks.ToList();
    }

    public async Task<GetTaskResponseDto> GetTaskByIdAsync(Guid taskId)
    {
        var response = await _supabaseClient.From<TaskModel>().Where(t => t.Id == taskId).Get();

        if (response == null)
        {
            return null;
        }

        var task = response.Models.FirstOrDefault();

        if (task == null)
        {
            return null;
        }

        var taskResponse = new GetTaskResponseDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            Status = task.Status
        };

        return taskResponse;
    }

    public async Task<GetTaskResponseDto> CreateTask(CreateTaskDto createTaskDto)
    {
        var task = new TaskModel
        {
            Id = createTaskDto.Id,
            Name = createTaskDto.Name,
            Description = createTaskDto.Description,
            DueDate = createTaskDto.DueDate,
            CreatedAt = DateTime.UtcNow,
            Status = TaskProgressStatus.PENDING.ToString()
        };

        var response = await _supabaseClient.From<TaskModel>().Insert(task);

        if (response == null)
        {
            return null;
        }

        var createdTask = response.Models.First();
        var taskResponse = new GetTaskResponseDto
        {
            Id = createdTask.Id,
            Name = createdTask.Name,
            Description = createdTask.Description,
            DueDate = createdTask.DueDate,
            CreatedAt = createdTask.CreatedAt,
            Status = createdTask.Status
        };

        return taskResponse;
    }

    public async Task<GetTaskResponseDto> UpdateTask(Guid id, UpdateTaskDto updateTaskDto)
    {
        var task = new TaskModel
        {
            Id = id,
            Name = updateTaskDto.Name,
            Description = updateTaskDto.Description,
            DueDate = updateTaskDto.DueDate,
            CreatedAt = DateTime.UtcNow,
            Status = updateTaskDto.Status,
            UserId = updateTaskDto.UserId
        };

        var response = await _supabaseClient.From<TaskModel>().Update(task);

        if (response == null)
        {
            return null;
        }

        var updatedTask = response.Models.First();
        var taskResponse = new GetTaskResponseDto
        {
            Id = updatedTask.Id,
            Name = updatedTask.Name,
            Description = updatedTask.Description,
            DueDate = updatedTask.DueDate,
            CreatedAt = updatedTask.CreatedAt,
            Status = updatedTask.Status,
            UserId = updatedTask.UserId
        };

        return taskResponse;
    }

    public async Task DeleteTask(Guid id)
    {
        await _supabaseClient.From<TaskModel>().Where(t => t.Id == id).Delete();
    }

    public async Task<IEnumerable<GetTaskResponseDto>> GetTasksByUserIdAsync(Guid userId)
    {
        var response = await _supabaseClient.From<TaskModel>().Where(t => t.UserId == userId).Get();

        if (response == null)
        {
            return Enumerable.Empty<GetTaskResponseDto>();
        }

        var tasks = response.Models.Select(t => new GetTaskResponseDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            DueDate = t.DueDate,
            CreatedAt = t.CreatedAt,
            Status = t.Status,
            UserId = t.UserId
        });

        return tasks.ToList();
    }
}
