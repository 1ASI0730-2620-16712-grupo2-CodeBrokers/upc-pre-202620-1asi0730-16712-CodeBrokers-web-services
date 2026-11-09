namespace VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

public record CareProviderId(Guid Value)
{
    public static CareProviderId New()
    {
        return new CareProviderId(Guid.NewGuid());
    }
}