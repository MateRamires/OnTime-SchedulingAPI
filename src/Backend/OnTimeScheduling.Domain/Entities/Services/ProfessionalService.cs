using OnTimeScheduling.Domain.Entities.DefaultEntity;

namespace OnTimeScheduling.Domain.Entities.Services;

public class ProfessionalService : TenantEntity
{
    public Guid UserId { get; private set; }
    public Guid ServiceId { get; private set; }

    public User.User Professional { get; private set; } = null!;
    public Service Service { get; private set; } = null!;

    private ProfessionalService() { }

    public ProfessionalService(Guid userId, Guid serviceId)
    {
        UserId = userId;
        ServiceId = serviceId;
    }
}
