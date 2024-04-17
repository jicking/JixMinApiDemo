using Microsoft.EntityFrameworkCore;

namespace JixMinApi.Features.Todo;

public class Todo
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsComplete { get; set; }
    public DateTimeOffset DateCreated { get; set; }
}


public class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options)
        : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();
}

public static class TodoDbDefaultValues
{
    public static Guid TestTodoId = new Guid("684d921c-755a-450c-91d7-af495e116d9b");

    public static List<Todo> TestTodos = new List<Todo> {
        new Todo() {
            Id = Guid.NewGuid(),
            Name = "Completed Todo",
            IsComplete = true,
        },
        new Todo() {
            Id = TestTodoId,
            Name = "Test Todo",
        }
    };
}