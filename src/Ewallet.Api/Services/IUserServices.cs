using Ewallet.Api.DTOs;
using Ewallet.Api.Models;

namespace Ewallet.Api.Services;

public interface IUserServices
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<UserResponse?> CreateAsync(UserRequest request, CancellationToken cancellationToken);
    Task<UserResponse?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyList<TransactionResponse>> GetTransactionsByUsersIdAsync(int usersId, CancellationToken cancellationToken);
}