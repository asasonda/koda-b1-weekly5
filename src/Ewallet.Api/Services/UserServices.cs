using Ewallet.Api.DTOs;
using Ewallet.Api.Models;
using Ewallet.Api.Repositories;

namespace Ewallet.Api.Services;

public sealed class UserServices : IUserServices
{
    private readonly IUserRepository _repository;

    public UserServices(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserResponse?> CreateAsync(UserRequest request, CancellationToken cancellationToken)
    {
        var users = new Users(request.Name, request.Email, request.Password, request.Phone);
        var created = await _repository.AddUserWithWalletAsync(users, request.WalletNumber, request.Pin, cancellationToken);
        if(created is null)
        {
            return null;
        }

        return new UserResponse(created.Value, users.Name, users.Email, users.Phone);
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _repository.GetAllAsync(cancellationToken);
        return users.Select(UserResponse.FromModel).ToArray();
    }

    public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if(id < 0)
        {
            return null;
        }
        var users = await _repository.GetByIdAsync(id, cancellationToken);
        return users is null ? null : UserResponse.FromModel(users);
    }

    public async Task<UserResponse?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var users = new Users(id, request.Name, request.Email, request.Password, request.Phone);
        var updated = await _repository.UpdateUserAsync(users, cancellationToken);
        return updated ? UserResponse.FromModel(users) : null;
    }
}