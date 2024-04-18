namespace JixMinApi.Features.Todo;


public class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }

    public bool HasValidationError { get; init; }
    public IReadOnlyList<KeyValuePair<string, string[]>> ValidationErrors { get; init; } = [];

    //public bool IsError { get; init; }
    //public Exception? Exception { get; init; }


    public Result(T value)
    {
        Value = value;
        IsSuccess = true;
    }

    public Result(IEnumerable<KeyValuePair<string, string[]>> validationErrors)
    {
        ValidationErrors = validationErrors.ToList();
        HasValidationError = true;
    }

    public Result(string field, string validationErrorMessage)
    {
        List<KeyValuePair<string, string[]>> validationErrors
            = [new KeyValuePair<string, string[]>(field, [validationErrorMessage])];
        ValidationErrors = validationErrors;
        HasValidationError = true;
    }
}

