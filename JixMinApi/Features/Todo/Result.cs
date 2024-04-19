namespace JixMinApi.Features.Todo;


public class Result<T>
{
    public bool IsSuccess => !Errors.Any() && !IsNotFound;
    public T? Value { get; init; }
    public IReadOnlyList<KeyValuePair<string, string>> Errors { get; init; } = [];
    public bool IsNotFound { get; set; }

    public Result(T value)
    {
        if (value is null)
        {
            IsNotFound = true;
        }

        Value = value;
    }

    public Result(IEnumerable<KeyValuePair<string, string>> validationErrors)
    {
        Errors = validationErrors.ToList();
    }

    public Result(string field, string validationErrorMessage)
    {
        Errors = [new(field, validationErrorMessage)];
    }

    public static Result<T> NotFound()
    {
        T val = default;
        return new Result<T>(val);
    }
}

public static class ResultExtensions
{
    public static IDictionary<string, string[]> ToErrorDictionary(this IEnumerable<KeyValuePair<string, string>> errors)
    {
        Dictionary<string, string[]> result = [];

        foreach (var e in errors)
        {
            if (!result.TryGetValue(e.Key, out var messages))
            {
                result[e.Key] = [e.Value];
                continue;
            }

            var newArray = messages.Concat([e.Value]).ToArray();
            result[e.Key] = newArray;
        }

        return result;
    }
}
