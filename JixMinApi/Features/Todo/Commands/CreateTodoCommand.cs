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
        var todo = new Todo() { Name = request.input.Name, IsComplete = request.input.IsComplete };

        try
        {
            await _db.Todos.AddAsync(todo);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Todo {Id} is successfully created", todo.Id);
            // await _emailService.SendEmail(email);
        }
        catch (Exception ex)
        {
            _logger.LogError("Todo {Id} failed due to an error: {ExMessage}",
            todo.Id, ex.Message);
            return new Result<TodoDto>(ex);
        }

        return new Result<TodoDto>(todo.ToDto());
    }
}