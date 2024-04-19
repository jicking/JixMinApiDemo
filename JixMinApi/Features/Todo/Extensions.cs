namespace JixMinApi.Features.Todo;

public static class Extensions
{
    public static TodoDto ToDto(this Todo model)
    {
        return new TodoDto(model.Id, model.Name, model.IsComplete);
    }

    public static List<TodoDto> ToDto(this IEnumerable<Todo> model)
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
