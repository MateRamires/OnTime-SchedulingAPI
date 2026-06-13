using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.User;

namespace OnTimeScheduling.Application.UseCases.Users.Management.Mapper;

public class UserResponseMapper
{
    public static ResponseUserJson Map(User user)
    {
        return new ResponseUserJson
        {
            Id = user.Id,
            CompanyId = user.CompanyId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            Status = user.Status.ToString(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

}
