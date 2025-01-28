using System;
using Carter;
using Microsoft.AspNetCore.Mvc;
using task_management_api.Models.Tasks;

namespace task_management_api.Controllers;

public class TasksController : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var tasks = app.MapGroup("api/tasks");

        tasks.MapGet("", GetAllTasks)
            .WithName(nameof(GetAllTasks));

        tasks.MapGet("{id:int}", GetTaskById)
            .WithName(nameof(GetTaskById));

        tasks.MapPost("", CreateTask)
            .WithName(nameof(CreateTask));

        tasks.MapPut("{id:int}", UpdateTask)
            .WithName(nameof(UpdateTask));

        tasks.MapDelete("/{id:int}", DeleteTask)
            .WithName(nameof(DeleteTask));
    }

    public async Task<IResult> GetAllTasks(Supabase.Client supabaseClient)
    {
        var response = await supabaseClient.From<Tasks>().Get();

        if (response == null)
        {
            return Results.Problem("Failed to fetch tasks");
        }

        var tasks = response.Models.Select(t => new GetTaskResponseDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            DueDate = t.DueDate,
            CreatedAt = t.CreatedAt
        });
        
        return Results.Ok(tasks);
    }

    public async Task<IResult> GetTaskById(Supabase.Client supabaseClient, int id)
    {
        var response = await supabaseClient.From<Tasks>().Where(t => t.Id == id).Get();

        if (response == null)
        {
            return Results.Problem("Failed to fetch task");
        }

        var task = response.Models.FirstOrDefault();

        if (task == null)
        {
            return Results.NotFound();
        }

        var taskResponse = new GetTaskResponseDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt
        };

        return Results.Ok(taskResponse);
    }

    public async Task<IResult> CreateTask(Supabase.Client supabaseClient, [FromBody] CreateTaskDto createTaskDto)
    {
        var task = new Tasks
        {
            Name = createTaskDto.Name,
            Description = createTaskDto.Description,
            DueDate = createTaskDto.DueDate,
            CreatedAt = DateTime.UtcNow
        };

        var response = await supabaseClient.From<Tasks>().Insert(task);

        if (response == null)
        {
            return Results.Problem("Failed to create task");
        }

        var createdTask = response.Models.First();
        var taskResponse = new GetTaskResponseDto
        {
            Id = createdTask.Id,
            Name = createdTask.Name,
            Description = createdTask.Description,
            DueDate = createdTask.DueDate,
            CreatedAt = createdTask.CreatedAt
        };

        return Results.Created($"/tasks/{createdTask.Id}", taskResponse);
    }

    public async Task<IResult> UpdateTask(Supabase.Client supabaseClient, int id, [FromBody] CreateTaskDto createTaskDto)
    {
        var task = new Tasks
        {
            Id = id,
            Name = createTaskDto.Name,
            Description = createTaskDto.Description,
            DueDate = createTaskDto.DueDate
        };

        var response = await supabaseClient.From<Tasks>().Update(task);

        if (response == null)
        {
            return Results.Problem("Failed to update task");
        }

        if (response == null || response.Models.Count == 0)
        {
            return Results.Problem("Failed to update task");
        }

        return Results.Ok(response.Models.First());
    }

    public async Task<IResult> DeleteTask(Supabase.Client supabaseClient, int id)
    {
        await supabaseClient.From<Tasks>().Where(t => t.Id == id).Delete();

        return Results.NoContent();
    }
}
