using Microsoft.EntityFrameworkCore;

namespace JixMinApi.Features.Todo;

public static class Extensions
{
    public static void InjectTodoApiDependencies(this IServiceCollection services)
    {
        services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase(Constants.TodoApiGroupName));
    }

    public static void MapTodoApi(this WebApplication app)
    {
        var group = app.MapGroup(Constants.TodoApiRootPath)
            .WithTags(Constants.TodoApiGroupName);
        group.MapGet("/", TodoApi.GetAllTodos);
        group.MapPost("/", TodoApi.CreateTodo);
    }

    public static TodoDto ToDto(this Todo model)
    {
        return new TodoDto(model.Id, model.Name, model.IsComplete);
    }

    public static List<TodoDto> ToDto(this IList<Todo> model)
    {
        var result = new List<TodoDto>();
        foreach (var item in model)
            result.Add(item.ToDto());
        return result;
    }
    public static void SeedTestData(this TodoDb db)
    {
        if (db.Todos.Any())
            return;

        db.Todos.AddRange(TodoDbDefaultValues.TestTodos);
        db.SaveChanges();
    }
}
