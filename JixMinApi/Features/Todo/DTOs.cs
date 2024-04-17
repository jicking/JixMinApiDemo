namespace JixMinApi.Features.Todo;

public record TodoDto(Guid Id, string Name, bool IsComplete);
public record TodoCreateDto(string Name, bool IsComplete);

public record ValidationErrorDto(ValidationErrorItem[] ValidationErrors);
public record ValidationErrorItem(string Name, string Message);

