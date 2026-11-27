namespace VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

public record ClinicId(Guid Value)
{
    public static ClinicId New()
    {
        return new ClinicId(Guid.NewGuid());
    }
}
