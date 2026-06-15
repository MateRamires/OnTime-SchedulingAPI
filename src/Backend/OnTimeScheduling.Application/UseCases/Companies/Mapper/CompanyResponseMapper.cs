using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Company;

namespace OnTimeScheduling.Application.UseCases.Companies.Mapper;

public class CompanyResponseMapper
{
    public static ResponseCompanyJson Map(Company company) => new()
    {
        Id = company.Id,
        SocialReason = company.SocialReason,
        FantasyName = company.FantasyName,
        CNPJ = company.Document,
        Phone = company.Phone,
        CompanyEmail = company.Email,
        Status = company.Status.ToString(),
        CreatedAt = company.CreatedAt,
        UpdatedAt = company.UpdatedAt
    };
}
