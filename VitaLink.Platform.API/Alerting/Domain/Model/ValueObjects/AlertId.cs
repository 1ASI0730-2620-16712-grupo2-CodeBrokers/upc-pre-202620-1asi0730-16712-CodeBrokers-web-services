namespace VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;

/// <summary>
///     Unique identifier of an alert.
/// </summary>
public sealed record AlertId
{
  public AlertId(Guid value)
  {
    if (value == Guid.Empty)
      throw new ArgumentException("AlertId cannot be empty.", nameof(value));

    Value = value;
  }

  public Guid Value { get; }

  public static AlertId New() => new(Guid.NewGuid());

  public override string ToString() => Value.ToString();
}
