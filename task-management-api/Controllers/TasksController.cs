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

        await publishEndpoint.Publish(new TaskCreated()
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

    public async Task<IActionResult> UploadAttachment(Guid taskId, IFormFile file, CancellationToken cancellationToken,  HttpContext context)
    {
        if (file == null || file.Length == 0)
            return new BadRequestObjectResult("No file uploaded.");

        const long maxFileSize = 4 * 1024 * 1024; // 4MB
        if (file.Length > maxFileSize)
            return new BadRequestObjectResult("File size exceeds 4MB limit.");

        byte[] fileBytes;
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms, cancellationToken);
            fileBytes = ms.ToArray();
        }

        // Resolve scoped IPublishEndpoint from HttpContext
        var publishEndpoint = context.RequestServices.GetRequiredService<IPublishEndpoint>();

        await publishEndpoint.Publish(new FileUploadRequested
        {
            TaskId = taskId,
            FileName = file.FileName,
            FileBytes = fileBytes,
            ContentType = file.ContentType
        }, cancellationToken);

        return new OkObjectResult("File upload started.");
    }

    #endregion

    #region Routes
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var tasks = app.MapGroup("api/tasks").RequireAuthorization().DisableAntiforgery();

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

        tasks.MapPost("{taskId:Guid}/attachments", UploadAttachment)
            .Accepts<IFormFile>("multipart/form-data")
            .WithName(nameof(UploadAttachment));
    }
    #endregion
}
