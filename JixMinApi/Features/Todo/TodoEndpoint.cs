using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace JixMinApi.Features.Todo;

public static class TodoEndpoint
{
    const string ROOT_API_PATH = "/todos";

    public static void MapTodoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ROOT_API_PATH)
            .WithTags("Todos");
        group.MapGet("/", GetAllTodos);
        group.MapPost("/", CreateTodo);
    }

    public static async Task<Ok<Todo[]>> GetAllTodos(TodoDb db)
    {
        var todos = await db.Todos.ToArrayAsync();
        return TypedResults.Ok(todos);
    }

    public static async Task<Created<Todo>> CreateTodo(Todo todo, TodoDb db)
    {
        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        return TypedResults.Created($"{ROOT_API_PATH}/{todo.Id}", todo);
    }
}
