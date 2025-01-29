using System;
using Carter;
using Microsoft.AspNetCore.Mvc;
using task_management_api.Models.Tasks;
using task_management_api.Repository.Tasks;

namespace task_management_api.Controllers;

public class TasksController : ICarterModule
{
    private readonly ITaskRepository _taskRepository;
    public TasksController(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    #region private functions
    private async Task<IActionResult> GetAllTasks()
    {
        var tasks = await _taskRepository.GetAllTasksAsync();
        return new OkObjectResult(tasks);
    }

    private async Task<IActionResult> GetTaskById(int id)
    {
        var task = await _taskRepository.GetTaskByIdAsync(id);
        if (task == null)
        {
            return new NotFoundResult();
        }
        return new OkObjectResult(task);
    }

    private async Task<IActionResult> CreateTask([FromBody] CreateTaskDto task)
    {
        var createdTask = await _taskRepository.CreateTask(task);
        return new OkObjectResult(createdTask);
    }

    private async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto task)
    {
        var updatedTask = await _taskRepository.UpdateTask(id, task);
        if (updatedTask == null)
        {
            return new NotFoundResult();
        }
        return new OkObjectResult(updatedTask);
    }

    private async Task<IActionResult> DeleteTask(int id)
    {
        await _taskRepository.DeleteTask(id);
        return new OkResult();
    }

    private async Task<IActionResult> GetTasksByUserId(int userId)
    {
        var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);
        return new OkObjectResult(tasks);
    }

    #endregion

    #region  Routes
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var tasks = app.MapGroup("api/tasks");

        tasks.MapGet("", GetAllTasks)
            .WithName(nameof(GetAllTasks));

        tasks.MapGet("user/{userId:int}", GetTasksByUserId)
            .WithName(nameof(GetTasksByUserId));

        tasks.MapGet("{id:int}", GetTaskById)
            .WithName(nameof(GetTaskById));

        tasks.MapPost("", CreateTask)
            .WithName(nameof(CreateTask));

        tasks.MapPut("{id:int}", UpdateTask)
            .WithName(nameof(UpdateTask));

        tasks.MapDelete("{id:int}", DeleteTask)
            .WithName(nameof(DeleteTask));
    }
    #endregion
    
}
