namespace Ewallet.Api.Models;

public sealed class Wallets
{
    public int Id {get;}
    public int UsersId {get;}
    public string WalletNumber {get;}
    public string Pin {get;}

    public Wallets(int id, int usersId, string walletNumber, string pin)
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
        if (string.IsNullOrWhiteSpace(pin))
        {
            throw new ArgumentException("Pin is required", nameof(pin));
        }

        Id = id;
        UsersId = usersId;
        WalletNumber = walletNumber;
        Pin = pin;
    }
}