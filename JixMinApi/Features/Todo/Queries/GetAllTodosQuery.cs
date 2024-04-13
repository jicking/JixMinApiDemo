﻿using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JixMinApi.Features.Todo.Queries;

public record GetAllTodosQuery : IRequest<List<TodoDto>>;

public class GetAllTodosQueryHandler : IRequestHandler<GetAllTodosQuery, List<TodoDto>>
{
    private readonly TodoDb _db;
    private readonly ILogger<GetAllTodosQueryHandler> _logger;

    public GetAllTodosQueryHandler(TodoDb db, ILogger<GetAllTodosQueryHandler> logger) => (_db, _logger) = (db, logger);

    public async Task<List<TodoDto>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Start query");
        var data = await _db.Todos
            .AsNoTracking()
            .ToListAsync();
        _logger.LogDebug($"End query");
        return data.ToDto();
    }
}