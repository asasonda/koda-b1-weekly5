namespace Ewallet.Api.Models;

public sealed class Wallets
{
    public int Id {get; private set;}
    public int UsersId {get; private set;}
    public string WalletNumber {get; private set;}
    public decimal Balance {get; private set;}

    public Wallets(int id, int usersId, string walletNumber, decimal balance)
    {
        if(id < 0)
        {
            throw new ArgumentException("Id required", nameof(id));
        }
        if(usersId < 0)
        {
            throw new ArgumentException("UsersId is required", nameof(usersId));
        }
        if (string.IsNullOrWhiteSpace(walletNumber))
        {
            throw new ArgumentException("Wallet Number is Required", nameof(walletNumber));
        }
        if(Balance < 0)
        {
            throw new ArgumentException("Balance not be negative", nameof(balance));
        }

        Id = id;
        UsersId = usersId;
        WalletNumber = walletNumber;
        Balance = balance;
    }

    public Wallets(string walletNumber)
    {
        if (string.IsNullOrWhiteSpace(walletNumber)){
            throw new ArgumentException("Wallet Number is Required", nameof(walletNumber));
        }

        WalletNumber = walletNumber;
    }

}