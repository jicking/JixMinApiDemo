using FluentValidation;
using FluentValidation.Results;
using JixMinApi.Features.Todo.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JixMinApi.Features.Todo.Queries;

public record GetTodoByIdQuery(Guid Id) :IRequest<Result<TodoDto>>;

public sealed class GetTodoByIdQueryValidator
    : AbstractValidator<GetTodoByIdQuery>
{
    public GetTodoByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotNull()
            .NotEmpty();
    }
}

public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, Result<TodoDto>>
{
    private readonly TodoDb _db;
    private readonly ILogger<GetTodoByIdQueryHandler> _logger;

    public GetTodoByIdQueryHandler(TodoDb db, ILogger<GetTodoByIdQueryHandler> logger) => (_db, _logger) = (db, logger);

    public async Task<Result<TodoDto>> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        // validate
        var validator = new GetTodoByIdQueryValidator();
        ValidationResult validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new KeyValuePair<string, string>(e.PropertyName, e.ErrorMessage));
            return new Result<TodoDto>(errors);
        }

        _logger.LogDebug($"Start query");
        Todo? todo = await _db.Todos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        _logger.LogDebug($"End query");

        return new Result<TodoDto>(todo?.ToDto());
    }
}