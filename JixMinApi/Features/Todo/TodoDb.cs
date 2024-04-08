using Microsoft.EntityFrameworkCore;

namespace JixMinApi.Features.Todo;

public class Todo
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsComplete { get; set; }
}


public class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options)
        : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();
}