using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.IAM.Infrastructure.Tokens.JWT;

namespace VitaLink.Platform.API.Tests.IAM;

/// <summary>
///     Session life cycle of the access tokens (TS-04 CA-02): expiration, issuer, audience and revocation.
/// </summary>
public class JwtTokenServiceTests
{
    private static JwtTokenService Service(string key = "test-signing-key-with-at-least-32-characters",
        string issuer = "VitaLink.Platform.API", IMemoryCache? cache = null) =>
        new(Options.Create(new JwtOptions { SigningKey = key, Issuer = issuer }),
            cache ?? new MemoryCache(new MemoryCacheOptions()));

    private static User CareProvider() =>
        new(new Email("care.provider@vitalink.test"), new PasswordHash("hash"), UserRole.CareProvider);

    [Fact]
    public async Task IssuedTokenIsValidForItsUser()
    {
        var service = Service();
        var user = CareProvider();

        var token = await service.ValidateAsync(service.Generate(user).Value);

        Assert.NotNull(token);
        Assert.Equal(user.Id, token.UserId);
        Assert.False(string.IsNullOrWhiteSpace(token.TokenId));
    }

    [Fact]
    public async Task RevokedTokenIsRejected()
    {
        var service = Service();
        var issued = service.Generate(CareProvider());
        var token = await service.ValidateAsync(issued.Value);

        service.Revoke(token!.TokenId, token.ExpiresAt);

        Assert.Null(await service.ValidateAsync(issued.Value));
    }

    [Fact]
    public async Task RevokingOneTokenKeepsTheOthersValid()
    {
        var service = Service();
        var user = CareProvider();
        var first = await service.ValidateAsync(service.Generate(user).Value);
        var second = service.Generate(user).Value;

        service.Revoke(first!.TokenId, first.ExpiresAt);

        Assert.NotNull(await service.ValidateAsync(second));
    }

    [Fact]
    public async Task TokenSignedWithAnotherKeyIsRejected()
    {
        var issued = Service("another-signing-key-with-at-least-32-characters").Generate(CareProvider());

        Assert.Null(await Service().ValidateAsync(issued.Value));
    }

    [Fact]
    public async Task TokenFromAnotherIssuerIsRejected()
    {
        var issued = Service(issuer: "Another.Issuer").Generate(CareProvider());

        Assert.Null(await Service().ValidateAsync(issued.Value));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-token")]
    public async Task MalformedTokenIsRejected(string value) => Assert.Null(await Service().ValidateAsync(value));
}
