namespace TicketFlow.Application.Common.Models;

public enum ResultErrorType
{
    None,
    Validation,
    NotFound,
}

public class Result
{
    protected Result(bool isSuccess, ResultErrorType errorType, string? error, IReadOnlyDictionary<string, string[]>? validationErrors)
    {
        IsSuccess = isSuccess;
        ErrorType = errorType;
        Error = error;
        ValidationErrors = validationErrors;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ResultErrorType ErrorType { get; }
    public string? Error { get; }
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }

    public static Result Success() => new(true, ResultErrorType.None, null, null);

    public static Result NotFound(string error) => new(false, ResultErrorType.NotFound, error, null);

    public static Result Validation(IReadOnlyDictionary<string, string[]> validationErrors) =>
        new(false, ResultErrorType.Validation, null, validationErrors);

    public static Result<T> Success<T>(T value) => new(value);

    public static Result<T> NotFound<T>(string error) => new(ResultErrorType.NotFound, error, null);

    public static Result<T> Validation<T>(IReadOnlyDictionary<string, string[]> validationErrors) =>
        new(ResultErrorType.Validation, null, validationErrors);
}

public sealed class Result<T> : Result
{
    private readonly T? _value;

    internal Result(T value) : base(true, ResultErrorType.None, null, null) => _value = value;

    internal Result(ResultErrorType errorType, string? error, IReadOnlyDictionary<string, string[]>? validationErrors)
        : base(false, errorType, error, validationErrors)
    {
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Não é possível acessar Value de um Result com falha.");
}
