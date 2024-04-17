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
    public static void InjectTodoEndpointServices(this IServiceCollection services)
    {
        services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase(Constants.TodoApiGroupName));
    }

    public static void MapTodoEndpoints(this WebApplication app)
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
            .Produces<ValidationErrorDto>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateTodoAsync)
            .Accepts<TodoCreateDto>(MediaTypeNames.Application.Json)
            .Produces<TodoDto>(StatusCodes.Status201Created)
            .Produces<ValidationErrorDto>(StatusCodes.Status400BadRequest);

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
    public static async Task<Results<BadRequest<ValidationErrorDto>, NotFound, Ok<TodoDto>>> GetTodoByIdAsync(Guid id, IMediator mediator)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.BadRequest<ValidationErrorDto>(
                new(
                    ValidationErrors: [new ValidationErrorItem("id", "id must not be an empty guid.")])
                );
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
    public static async Task<Results<Created<TodoDto>, BadRequest>> CreateTodoAsync(TodoCreateDto input, IMediator mediator)
    {
        var result = await mediator.Send(new CreateTodoCommand(input));
        if (result.IsError && result.Exception is not null)
        {
            throw result.Exception;
        }

        if (result.HasValidationError && result.ValidationErrors.Any())
        {
            return TypedResults.BadRequest();
        }

        var todo = result.Value;
        return TypedResults.Created($"{Constants.TodoApiRootPath}/{todo.Id}", todo);
    }
}
