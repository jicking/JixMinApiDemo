namespace JixMinApi.Features.Todo;

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
