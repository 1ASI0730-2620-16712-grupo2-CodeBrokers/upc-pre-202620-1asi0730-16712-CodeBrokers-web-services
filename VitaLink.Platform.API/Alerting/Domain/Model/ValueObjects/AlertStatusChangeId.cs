namespace VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;

/// <summary>
///     Unique identifier of an alert status change.
/// </summary>
public sealed record AlertStatusChangeId
{
  public AlertStatusChangeId(Guid value)
  {
    if (value == Guid.Empty)
      throw new ArgumentException("AlertStatusChangeId cannot be empty.", nameof(value));

    Value = value;
  }

  public Guid Value { get; }

  public static AlertStatusChangeId New() => new(Guid.NewGuid());

  public override string ToString() => Value.ToString();
}
