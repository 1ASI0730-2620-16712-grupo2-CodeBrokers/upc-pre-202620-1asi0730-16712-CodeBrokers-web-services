namespace VitaLink.Platform.API.Shared.Application.Patterns;

/// <summary>
///     Represents the outcome of an operation that can either succeed with a value
///     or fail with an expected, typed error.
/// </summary>
/// <remarks>
///     Command services return a <see cref="Result{TValue, TError}"/> instead of throwing
///     exceptions for expected business failures (for example, a duplicated or not found resource).
///     Exceptions remain reserved for unexpected errors.
/// </remarks>
/// <typeparam name="TValue">The type of the value returned on success.</typeparam>
/// <typeparam name="TError">The type of the error returned on failure, usually an enum.</typeparam>
public abstract record Result<TValue, TError>
{
    /// <summary>
    ///     Successful outcome carrying the resulting value.
    /// </summary>
    /// <param name="Value">The resulting value.</param>
    public sealed record Success(TValue Value) : Result<TValue, TError>;

    /// <summary>
    ///     Failed outcome carrying the expected error.
    /// </summary>
    /// <param name="Error">The error that caused the failure.</param>
    public sealed record Failure(TError Error) : Result<TValue, TError>;

    /// <summary>
    ///     Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess => this is Success;

    /// <summary>
    ///     Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => this is Failure;

    /// <summary>
    ///     Transforms the value of a successful result, keeping the error of a failed one.
    /// </summary>
    /// <param name="onSuccess">The function applied to the successful value.</param>
    /// <typeparam name="TNext">The type of the transformed value.</typeparam>
    /// <returns>A new result with the transformed value or the original error.</returns>
    public Result<TNext, TError> Map<TNext>(Func<TValue, TNext> onSuccess) =>
        this switch
        {
            Success s => new Result<TNext, TError>.Success(onSuccess(s.Value)),
            Failure f => new Result<TNext, TError>.Failure(f.Error),
            _ => throw new InvalidOperationException("Unknown Result type")
        };

    /// <summary>
    ///     Reduces the result to a single value by handling both outcomes.
    /// </summary>
    /// <param name="onSuccess">The function applied when the operation succeeded.</param>
    /// <param name="onFailure">The function applied when the operation failed.</param>
    /// <typeparam name="TResult">The type of the returned value.</typeparam>
    /// <returns>The value produced by the matching function.</returns>
    public TResult Fold<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onFailure) =>
        this switch
        {
            Success s => onSuccess(s.Value),
            Failure f => onFailure(f.Error),
            _ => throw new InvalidOperationException("Unknown Result type")
        };

    /// <summary>
    ///     Executes the action that matches the outcome.
    /// </summary>
    /// <param name="onSuccess">The action executed when the operation succeeded.</param>
    /// <param name="onFailure">The action executed when the operation failed.</param>
    public void Match(Action<TValue> onSuccess, Action<TError> onFailure)
    {
        if (this is Success s)
            onSuccess(s.Value);
        else if (this is Failure f)
            onFailure(f.Error);
    }
}
