using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace JixMinApi.Features.Todo;

public static class TodoApi
{
    public static async Task<Ok<Todo[]>> GetAllTodos(TodoDb db)
    {
        var todos = await db.Todos.ToArrayAsync();
        return TypedResults.Ok(todos);
    }

    public static async Task<Created<Todo>> CreateTodo(TodoCreateDto input, TodoDb db)
    {
        var todo = new Todo() { Name = input.Name, IsComplete = input.IsComplete };

        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        return TypedResults.Created($"{Constants.TodoApiRootPath}/{todo.Id}", todo);
    }
}
