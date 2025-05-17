using Carter;
using Microsoft.AspNetCore.Mvc;
using task_management_api.Models.Tasks;
using task_management_api.Repository.Tasks;
using Microsoft.AspNetCore.Authorization;
using MassTransit;
using TaskContracts.Events;

namespace task_management_api.Controllers;

[Authorize]
public class TasksController : ICarterModule
{
    private readonly ITaskRepository _taskRepository;

    public TasksController(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    #region private functions
    private async Task<IResult> GetAllTasks()
    {
        var tasks = await _taskRepository.GetAllTasksAsync();
        return Results.Ok(tasks);
    }

    private async Task<IResult> GetTaskById(Guid id)
    {
        var task = await _taskRepository.GetTaskByIdAsync(id);
        return task is null ? Results.NotFound() : Results.Ok(task);
    }

    private async Task<IResult> CreateTask(HttpContext context, [FromBody] CreateTaskDto task)
    {
        var createdTask = await _taskRepository.CreateTask(task);

        // Resolve scoped IPublishEndpoint from HttpContext
        var publishEndpoint = context.RequestServices.GetRequiredService<IPublishEndpoint>();

        await publishEndpoint.Publish(new TaskCreatedEvent()
        {
            Id = createdTask.Id,
            Name = createdTask.Name,
            Description = createdTask.Description,
            DueDate = createdTask.DueDate,
            CreatedAt = createdTask.CreatedAt,
            Status = createdTask.Status,
            UserId = createdTask.UserId
        });

        return Results.Ok(createdTask);
    }

    private async Task<IResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto task)
    {
        var updatedTask = await _taskRepository.UpdateTask(id, task);
        return updatedTask is null ? Results.NotFound() : Results.Ok(updatedTask);
    }

    private async Task<IResult> DeleteTask(Guid id)
    {
        await _taskRepository.DeleteTask(id);
        return Results.Ok();
    }

    private async Task<IResult> GetTasksByUserId(Guid userId)
    {
        var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);
        return Results.Ok(tasks);
    }
    #endregion

    #region Routes
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var tasks = app.MapGroup("api/tasks").RequireAuthorization();

        tasks.MapGet("", GetAllTasks)
            .WithName(nameof(GetAllTasks));

        tasks.MapGet("user/{userId:Guid}", GetTasksByUserId)
            .WithName(nameof(GetTasksByUserId));

        tasks.MapGet("{id:Guid}", GetTaskById)
            .WithName(nameof(GetTaskById));

        tasks.MapPost("", CreateTask)
            .WithName(nameof(CreateTask));

        tasks.MapPut("{id:Guid}", UpdateTask)
            .WithName(nameof(UpdateTask));

        tasks.MapDelete("{id:Guid}", DeleteTask)
            .WithName(nameof(DeleteTask));
    }
    #endregion
}
