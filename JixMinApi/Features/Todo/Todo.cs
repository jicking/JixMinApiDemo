namespace JixMinApi.Features.Todo;

public class Todo
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsComplete { get; set; }

}
