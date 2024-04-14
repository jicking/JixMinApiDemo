namespace JixMinApi.Shared;


public class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }

    public bool IsError { get; init; }
    public Exception? Exception { get; init; }

    public bool HasValidationError { get; init; }
    public List<KeyValuePair<string, string>> ValidationErrors { get; init; } = new List<KeyValuePair<string, string>>();


    public Result(T value)
    {
        Value = value;
        IsSuccess = true;
    }

    public Result(Exception exception)
    {
        Exception = exception;
        IsError = true;
    }

    public Result(List<KeyValuePair<string, string>> validationErrors)
    {
        ValidationErrors = validationErrors;
        HasValidationError = true;
    }

    public Result(string field, string validationErrorMessage)
    {
        var validationErrors = new List<KeyValuePair<string, string>>();
        validationErrors.Add(new KeyValuePair<string, string>(field, validationErrorMessage));
        ValidationErrors = validationErrors;
        HasValidationError = true;
    }
}