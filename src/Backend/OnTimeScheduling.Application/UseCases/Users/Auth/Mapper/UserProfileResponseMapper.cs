using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.User;

namespace OnTimeScheduling.Application.UseCases.Users.Auth.Mapper;

internal static class UserProfileResponseMapper
{
    public static ResponseUserProfileJson Map(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        CompanyId = user.CompanyId,
        Role = (Communication.Enums.UserRole)(int)user.Role
    };
}
