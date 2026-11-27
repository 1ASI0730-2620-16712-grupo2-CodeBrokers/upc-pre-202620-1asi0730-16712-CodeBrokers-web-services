using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Tests.PreventiveMonitoring;

/// <summary>
///     Rules of the <see cref="Threshold" /> value object.
/// </summary>
public class ThresholdTests
{
    [Theory]
    [InlineData(9, true)]
    [InlineData(10, false)]
    [InlineData(20, false)]
    [InlineData(21, true)]
    public void LimitsAreInclusive(decimal value, bool violated) =>
        Assert.Equal(violated, new Threshold(10, 20).IsViolatedBy(value));

    [Fact]
    public void OnlyMinimumIsAllowed() => Assert.True(new Threshold(92, null).IsViolatedBy(91));

    [Fact]
    public void AtLeastOneLimitIsRequired() => Assert.Throws<ArgumentException>(() => new Threshold(null, null));

    [Fact]
    public void MinimumCannotBeGreaterThanMaximum() => Assert.Throws<ArgumentException>(() => new Threshold(20, 10));
}
