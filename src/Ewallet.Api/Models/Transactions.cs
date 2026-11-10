namespace Ewallet.Api.Models;

public sealed class Transactions
{
    public int Id {get; private set;}
    public int? SenderWalletId {get; private set;}
    public int ReceiverWalletId {get; private set;}
    public string Type {get; private set;}
    public decimal Amount {get; private set;}
    public string? Description {get; private set;}
    public DateTime CreatedAt {get; private set;}


    public Transactions(int id, int? senderWalletId, int receiverWalletId, string type, decimal amount, string? description, DateTime createdAt)
    {
        if(id < 0)
        {
            throw new ArgumentException("Id tidak valid and must be positive id", nameof(id));
        }
        if(senderWalletId < 0)
        {
            throw new ArgumentException("SenderId tidak valid, must be positive id", nameof(senderWalletId));
        }
        if(receiverWalletId < 0)
        {
            throw new ArgumentException("ReceiverId tidak valid, must be positive id", nameof(receiverWalletId));
        }
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("type is required", nameof(type));
        }
        if(amount < 0)
        {
            throw new ArgumentException("Amount tidak valid, must be positive value", nameof(amount));
        }

        Id = id;
        SenderWalletId = senderWalletId;
        ReceiverWalletId = receiverWalletId;
        Type = type;
        Amount = amount;
        Description = description;
        CreatedAt = createdAt;
    }

    public Transactions(int? senderWalletId, int receiverWalletId, string type, decimal amount, string? description)
    {
        if (senderWalletId <= 0 && senderWalletId.HasValue)
            throw new ArgumentException("SenderWalletId tidak valid", nameof(senderWalletId));

        if (receiverWalletId <= 0)
            throw new ArgumentException("ReceiverWalletId tidak valid", nameof(receiverWalletId));

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type is required", nameof(type));

        if (amount <= 0)
            throw new ArgumentException("Amount harus lebih dari 0", nameof(amount));

        SenderWalletId = senderWalletId;
        ReceiverWalletId = receiverWalletId;
        Type = type;
        Amount = amount;
        Description = description;
    }
}