using Ewallet.Api.DTOs;
using Ewallet.Api.Models;
using Ewallet.Api.Repositories;

namespace Ewallet.Api.Services;

public sealed class UserServices : IUserServices
{
    private readonly IUserRepository _repository;
    private readonly ITransactionRepository _transactionRepository;

    public UserServices(IUserRepository repository, ITransactionRepository transactionRepository)
    {
        _repository = repository;
        _transactionRepository = transactionRepository;
    }

    public async Task<UserResponse?> CreateAsync(UserRequest request, CancellationToken cancellationToken)
    {
        var users = new Users(request.Name, request.Email, request.Phone);
        var wallets = new Wallets(request.WalletNumber);
        var created = await _repository.AddUserWithWalletAsync(users, wallets, cancellationToken);

        return created ? UserResponse.FromModel(users) : null;
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _repository.GetAllAsync(cancellationToken);
        return users.Select(UserResponse.FromModel).ToArray();
    }

    public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (id < 0)
        {
            return null;
        }
        var users = await _repository.GetByIdAsync(id, cancellationToken);
        return users is null ? null : UserResponse.FromModel(users);
    }

    public async Task<UserResponse?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var users = new Users(id, request.Name, request.Email, request.Phone);
        var updated = await _repository.UpdateUserAsync(users, cancellationToken);
        return updated ? UserResponse.FromModel(users) : null;
    }

    public async Task<IReadOnlyList<TransactionResponse>> GetTransactionsByUsersIdAsync(int usersId, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetByUsersIdAsync(usersId, cancellationToken);

        return transactions.Select(TransactionResponse.FromModel).ToList();
    }
}