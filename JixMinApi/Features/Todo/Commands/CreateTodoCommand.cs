using MediatR;

namespace JixMinApi.Features.Todo.Commands;

public record CreateTodoCommand(CreateTodoDto input) : IRequest<Result<TodoDto>>;

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, Result<TodoDto>>
{
    private readonly TodoDb _db;
    private readonly ILogger<CreateTodoCommandHandler> _logger;

    public CreateTodoCommandHandler(TodoDb db, ILogger<CreateTodoCommandHandler> logger) => (_db, _logger) = (db, logger);

    public async Task<Result<TodoDto>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        // simple inline validation, if needed validate using behaviors https://github.com/jbogard/MediatR/wiki/Behaviors
        if (string.IsNullOrEmpty(request.input.Name))
        {
            return new Result<TodoDto>([new KeyValuePair<string, string[]>("Name", ["Must not be empty."])]);
        }

        var todo = new Todo()
        {
            Name = request.input.Name,
            IsComplete = request.input.IsComplete,
            DateCreated = DateTimeOffset.UtcNow,
        };

        await _db.Todos.AddAsync(todo);
        await _db.SaveChangesAsync();

        _logger.LogInformation($"Todo {todo.Id} is successfully created");

        // publish mediatr notification https://github.com/jbogard/MediatR/wiki#notifications
        // await _mediator.Publish(new TodoCreatedNotification(todo), cancellationToken);

        return new Result<TodoDto>(todo.ToDto());
    }
}