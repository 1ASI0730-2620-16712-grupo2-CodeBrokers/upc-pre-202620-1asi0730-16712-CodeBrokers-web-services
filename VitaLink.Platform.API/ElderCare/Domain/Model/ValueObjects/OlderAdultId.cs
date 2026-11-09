namespace VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

public record OlderAdultId(Guid Value)
{
    public static OlderAdultId New()
    {
        return new OlderAdultId(Guid.NewGuid());
    }
}