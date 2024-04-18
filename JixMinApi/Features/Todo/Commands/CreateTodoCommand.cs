using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace JixMinApi.Features.Todo.Commands;

public record CreateTodoCommand(string Name, bool IsComplete) : IRequest<Result<TodoDto>>;

public sealed class CreateTodoCommandValidator
    : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MinimumLength(4)
            .MaximumLength(24);
    }
}

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, Result<TodoDto>>
{
    private readonly TodoDb _db;
    private readonly ILogger<CreateTodoCommandHandler> _logger;

    public CreateTodoCommandHandler(TodoDb db, ILogger<CreateTodoCommandHandler> logger) => (_db, _logger) = (db, logger);

    public async Task<Result<TodoDto>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        // fluent validation inside handler, if needed validate on pipeline using behaviors https://github.com/jbogard/MediatR/wiki/Behaviors
        var validator = new CreateTodoCommandValidator();
        ValidationResult validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new KeyValuePair<string, string>(e.PropertyName, e.ErrorMessage));
            return new Result<TodoDto>(errors);
        }

        var todo = new Todo()
        {
            Name = request.Name,
            IsComplete = request.IsComplete,
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