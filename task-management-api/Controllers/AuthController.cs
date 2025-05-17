using Carter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Supabase;
using System.Text.Json.Serialization;
using task_management_api.Models;

namespace task_management_api.Controllers;

public class AuthController : ICarterModule
{
    private readonly Client _supabaseClient;

    public AuthController(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("api/auth");

        auth.MapPost("login", async ([FromBody] LoginRequest request) =>
        {
            try
            {
                var session = await _supabaseClient.Auth.SignIn(request.Email, request.Password);
                return Results.Ok(new { Token = session?.AccessToken });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { Message = JsonConvert.DeserializeObject<AuthResponseModel>(ex.Message) });
            }
        });
    }
} 