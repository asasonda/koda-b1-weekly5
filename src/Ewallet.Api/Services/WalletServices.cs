using Ewallet.Api.DTOs;
using Ewallet.Api.Models;
using Ewallet.Api.Repositories;

namespace Ewallet.Api.Services;

public sealed class WalletServices : IWalletServices
{
    private readonly IWalletRepository _repository;
    private readonly ITransactionRepository _transactionRepository;

    public WalletServices(IWalletRepository repository, ITransactionRepository transactionRepository)
    {
        _repository = repository;
        _transactionRepository = transactionRepository;
    }
    public async Task<WalletResponse?> GetByIdUsersAsync(int usersId, CancellationToken cancellationToken)
    {
        var wallets = await _repository.GetByIdUsers(usersId, cancellationToken);
        return wallets is null ? null : WalletResponse.FromModel(wallets);
    }

    public async Task<TransactionResponse?> TopUpAsync(int usersId, TopUpRequest request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0){
            throw new ArgumentException("Nominal top up harus lebih dari 0");
        }

        var wallet = await _repository.GetByIdUsers(usersId, cancellationToken);

        if (wallet is null){
            return null;
        }
        
        var result = await _repository.TopUpAddAsync(usersId, request.Amount, cancellationToken);

        if (!result){
            return null;
        }

        var transaction = new Transactions(null, wallet.Id, "TopUp", request.Amount, "Top up saldo");

        var savedTransaction = await _transactionRepository.AddAsync(transaction, cancellationToken);

        if (savedTransaction is null){
            throw new InvalidOperationException("Saldo berhasil ditambahkan, tetapi riwayat transaksi gagal disimpan.");
        }
        return TransactionResponse.FromModel(savedTransaction);
    }

    public async Task<TransactionResponse?> TransferAsync(TransferRequest request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Nominal transfer harus lebih dari 0.");

        var sender = await _repository.GetByIdUsers(request.SenderWalletId, cancellationToken);

        if (sender is null){
            throw new KeyNotFoundException("Wallet pengirim tidak ditemukan.");
        }

        var receiver = await _repository.GetByIdUsers(request.ReceiverWalletId, cancellationToken);

        if (receiver is null){
            throw new KeyNotFoundException("Wallet penerima tidak ditemukan.");
        }
        
        if (sender.Id == receiver.Id){
            throw new ArgumentException("Tidak bisa transfer ke wallet sendiri.");
        }
        
        var debit = await _repository.ReduceBalanceAsync(sender.Id, request.Amount, cancellationToken);

        if (!debit){
            throw new InvalidOperationException("Saldo pengirim tidak mencukupi.");
        }
        
        var credit = await _repository.AddBalanceAsync(receiver.Id,request.Amount, cancellationToken);

        if (!credit){
            throw new InvalidOperationException("Gagal menambahkan saldo penerima.");
        }

        var transaction = new Transactions(
            sender.Id,
            receiver.Id,
            "Transfer",
            request.Amount,
            request.Description
        );

        var savedTransaction = await _transactionRepository.AddAsync(transaction, cancellationToken);

        if (savedTransaction is null){
            throw new InvalidOperationException("Gagal menyimpan riwayat transaksi.");
        }
        
        return TransactionResponse.FromModel(savedTransaction);
    }
}