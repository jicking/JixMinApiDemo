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
}
