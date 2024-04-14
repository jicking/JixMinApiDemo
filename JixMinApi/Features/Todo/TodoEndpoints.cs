using JixMinApi.Features.Todo.Commands;
using JixMinApi.Features.Todo.Queries;
using JixMinApi.Shared;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

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
            .WithTags(Constants.TodoApiGroupName);
        group.MapGet("/", GetAllTodos);
        group.MapPost("/", CreateTodo);
    }

    public static async Task<Ok<TodoDto[]>> GetAllTodos(IMediator mediator)
    {
        var todos = await mediator.Send(new GetAllTodosQuery());
        return TypedResults.Ok(todos?.ToArray());
    }

    public static async Task<Results<Created<TodoDto>, BadRequest> > CreateTodo(TodoCreateDto input, IMediator mediator)
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
