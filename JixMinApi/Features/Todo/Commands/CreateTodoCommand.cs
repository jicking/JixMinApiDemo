using JixMinApi.Shared;
using MediatR;

namespace JixMinApi.Features.Todo.Commands;

public record CreateTodoCommand(TodoCreateDto input) : IRequest<Result<TodoDto>>;

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, Result<TodoDto>>
{
    private readonly TodoDb _db;
    private readonly ILogger<CreateTodoCommandHandler> _logger;

    public CreateTodoCommandHandler(TodoDb db, ILogger<CreateTodoCommandHandler> logger) => (_db, _logger) = (db, logger);

    public async Task<Result<TodoDto>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        // add validation then set Result.ValidationErrors

        var todo = new Todo() { 
            Name = request.input.Name,
            IsComplete = request.input.IsComplete,
            DateCreated = DateTimeOffset.UtcNow,
        };

        try
        {
            await _db.Todos.AddAsync(todo);
            await _db.SaveChangesAsync();

            _logger.LogInformation($"Todo {todo.Id} is successfully created");
            // publish mediatr notification
            // await _mediator.Publish(new TodoCreatedNotification(todo), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Todo {todo.Id} failed due to an error: {ex.Message}",
            todo.Id, ex.Message);
            return new Result<TodoDto>(ex);
        }

        return new Result<TodoDto>(todo.ToDto());
    }
}