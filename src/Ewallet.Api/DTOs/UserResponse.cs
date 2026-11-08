using Ewallet.Api.Models;

namespace Ewallet.Api.DTOs;

public sealed record class UserResponse(int Id, string Name, string Email, string Phone)
{
    public static UserResponse FromModel(Users users) => new(users.Id, users.Name, users.Email, users.Phone);
}