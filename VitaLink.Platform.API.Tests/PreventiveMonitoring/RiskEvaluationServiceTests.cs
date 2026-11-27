using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Services;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Tests.PreventiveMonitoring;

/// <summary>
///     Acceptance criteria of US-15 (alert priority) checked on the risk evaluation domain service.
/// </summary>
public class RiskEvaluationServiceTests
{
    private const string TestMeasure = "TEST_MEASURE";
    private static readonly Guid OlderAdultId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    private static readonly Guid RecordedBy = Guid.Parse("00000000-0000-4000-8000-0000000000d1");

    private static MonitoringRule Rule(decimal? min, decimal? max, SeverityLevel severity, string type = TestMeasure) =>
        new(new CreateMonitoringRuleCommand(OlderAdultId, type, min, max, severity));

    private static HealthRecord Record(decimal value, string type = TestMeasure) =>
        new(new CreateHealthRecordCommand(OlderAdultId, type, value, null, null, RecordSource.CareProvider, RecordedBy));

    // CA-04: synthetic rule TEST_MEASURE, min 10, max 20, HIGH -> 9 and 21 raise a high priority.
    [Theory]
    [InlineData(9)]
    [InlineData(21)]
    public async Task ValueOutsideTheSyntheticRuleIsAHighPriorityViolation(decimal value)
    {
        var rule = Rule(10, 20, SeverityLevel.High);
        var service = new RiskEvaluationService(new InMemoryMonitoringRuleRepository(rule));

        var result = await service.EvaluateAsync(Record(value));

        Assert.Equal(RiskEvaluationOutcome.Violated, result.Outcome);
        Assert.Equal(SeverityLevel.High, result.Severity);
        Assert.Equal(rule.Id, result.Rule!.Id);
    }

    // CA-04: the limits themselves (10 and 20) do not raise an alert for that rule.
    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(15)]
    public async Task ValueOnOrInsideTheLimitsIsWithinLimits(decimal value)
    {
        var service = new RiskEvaluationService(new InMemoryMonitoringRuleRepository(Rule(10, 20, SeverityLevel.High)));

        var result = await service.EvaluateAsync(Record(value));

        Assert.Equal(RiskEvaluationOutcome.WithinLimits, result.Outcome);
        Assert.Null(result.Severity);
    }

    // CA-01: with several matching rules, the highest severity wins (HIGH > MEDIUM > LOW).
    [Fact]
    public async Task HighestSeverityWinsAmongMatchingRules()
    {
        var low = Rule(10, 20, SeverityLevel.Low);
        var high = Rule(5, 20, SeverityLevel.High);
        var medium = Rule(8, 20, SeverityLevel.Medium);
        var service = new RiskEvaluationService(new InMemoryMonitoringRuleRepository(low, medium, high));

        var result = await service.EvaluateAsync(Record(25));

        Assert.Equal(SeverityLevel.High, result.Severity);
        Assert.Equal(high.Id, result.Rule!.Id);
    }

    // CA-01: with equal severity, the rule with the lowest identifier is kept.
    [Fact]
    public async Task EqualSeverityKeepsTheRuleWithTheLowestId()
    {
        var first = Rule(10, 20, SeverityLevel.High);
        var second = Rule(12, 18, SeverityLevel.High);
        var expected = first.Id.Value.CompareTo(second.Id.Value) < 0 ? first : second;
        var service = new RiskEvaluationService(new InMemoryMonitoringRuleRepository(second, first));

        var result = await service.EvaluateAsync(Record(30));

        Assert.Equal(expected.Id, result.Rule!.Id);
    }

    [Fact]
    public async Task NoActiveRuleMeansNoRuleConfigured()
    {
        var service = new RiskEvaluationService(new InMemoryMonitoringRuleRepository(Rule(10, 20, SeverityLevel.High, "GLUCOSE")));

        var result = await service.EvaluateAsync(Record(99));

        Assert.Equal(RiskEvaluationOutcome.NoRuleConfigured, result.Outcome);
    }
}
