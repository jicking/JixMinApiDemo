using JixMinApi.Features.Todo.Commands;
using JixMinApi.Features.Todo.Queries;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Net.Mime;

namespace JixMinApi.Features.Todo;

public static class TodoEndpoints
{
    public static void AddTodoEndpointServices(this IServiceCollection services)
    {
        services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase(Constants.TodoApiGroupName));
    }

    public static void UseTodoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(Constants.TodoApiRootPath)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Tags = new List<OpenApiTag> {
                    new() { Name = Constants.TodoApiGroupName }
                },
            });

        // Swagger options
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-8.0

        group.MapGet("/", GetAllTodosAsync)
            .Produces<TodoDto[]>(StatusCodes.Status200OK)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Get All Todos.",
                Description = $"Get all Todos from db...",
                OperationId = "GetAllTodos"
            });

        group.MapGet("/{id}", GetTodoByIdAsync)
            .Produces<TodoDto>(StatusCodes.Status200OK)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateTodoAsync)
            .Accepts<CreateTodoDto>(MediaTypeNames.Application.Json)
            .Produces<TodoDto>(StatusCodes.Status201Created)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest);

        //group.MapDelete("/{id}", GetAllTodosAsync)
        //    .Produces(StatusCodes.Status204NoContent)
        //    .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<Ok<TodoDto[]>> GetAllTodosAsync(IMediator mediator)
    {
        var todos = await mediator.Send(new GetAllTodosQuery());
        return TypedResults.Ok(todos?.ToArray());
    }

    /// <summary>
    /// Fetches a todo by Id
    /// </summary>
    public static async Task<Results<ValidationProblem, NotFound, Ok<TodoDto>>> GetTodoByIdAsync(Guid id, IMediator mediator)
    {
        if (id == Guid.Empty)
        {
            var errors = new Dictionary<string, string[]>
            {
                ["id"] = ["id parameter must not be an empty guid."],
            };
            return TypedResults.ValidationProblem(errors);
        }

        var todos = await mediator.Send(new GetAllTodosQuery());
        var todo = todos.FirstOrDefault(t => t.Id == id);

        if (todo is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(todo);
    }

    /// <summary>
    /// Creates a new todo
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /Todo
    ///     {
    ///        "name": "Item #1",
    ///        "isComplete": true
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created item</response>
    /// <response code="400">Invalid payload</response>
    public static async Task<Results<Created<TodoDto>, ValidationProblem>> CreateTodoAsync(CreateTodoDto input, IMediator mediator)
    {
        var result = await mediator.Send(new CreateTodoCommand(input));

        if (result.HasValidationError)
        {
            return TypedResults.ValidationProblem(result.ValidationErrors.ToDictionary());
        }

        var todo = result.Value;
        return TypedResults.Created($"{Constants.TodoApiRootPath}/{todo.Id}", todo);
    }
}
