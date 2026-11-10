using System.Data;
using Ewallet.Api.Models;
using Microsoft.Data.SqlClient;

namespace Ewallet.Api.Repositories;

public sealed class SqlTransactionRepository : ITransactionRepository
{
    private readonly string _connection;

    public SqlTransactionRepository(IConfiguration configuration)
    {
        _connection = configuration.GetConnectionString("Ewallet") ?? throw new InvalidOperationException("Conncetion invalid");
    }

    public async Task<Transactions?> AddAsync(Transactions transactions, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO dbo.Transactions
                (SenderWalletId, ReceiverWalletId, Type, Amount, Description)
            VALUES
                (@senderWalletId, @receiverWalletId, @type, @amount, @description);

            SELECT Id, SenderWalletId, ReceiverWalletId, Type,
                Amount, Description, CreatedAt
            FROM dbo.Transactions
            WHERE Id = SCOPE_IDENTITY();
            """;

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);

        command.Parameters.Add("@senderWalletId", SqlDbType.Int).Value = (object?)transactions.SenderWalletId ?? DBNull.Value;

        command.Parameters.Add("@receiverWalletId", SqlDbType.Int).Value = transactions.ReceiverWalletId;

        command.Parameters.Add("@type", SqlDbType.NVarChar, 50).Value = transactions.Type;

        var amount = command.Parameters.Add("@amount", SqlDbType.Decimal);
        amount.Precision = 18;
        amount.Scale = 2;
        amount.Value = transactions.Amount;

        command.Parameters.Add("@description", SqlDbType.NVarChar, -1).Value =
            (object?)transactions.Description ?? DBNull.Value;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return ReadTransaction(reader);
    }

    public async Task<IReadOnlyList<Transactions>> GetByUsersIdAsync(int usersId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                t.Id,
                t.SenderWalletId,
                t.ReceiverWalletId,
                t.Type,
                t.Amount,
                t.Description,
                t.CreatedAt
            FROM dbo.Transactions t
            LEFT JOIN dbo.Wallets sender ON t.SenderWalletId = sender.Id
            JOIN dbo.Wallets receiver ON t.ReceiverWalletId = receiver.Id
            WHERE sender.UsersId = @usersId OR receiver.UsersId = @usersId
            ORDER BY t.CreatedAt DESC
            """;

        var transactions = new List<Transactions>();
        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);

        command.Parameters.Add("@usersId", SqlDbType.Int).Value = usersId;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            transactions.Add(ReadTransaction(reader));
        }

        return transactions;
    }

    private static Transactions ReadTransaction(SqlDataReader reader)
    {
        return new Transactions(
            reader.GetInt32(0),
            reader.IsDBNull(1) ? null : reader.GetInt32(1),
            reader.GetInt32(2),
            reader.GetString(3),
            reader.GetDecimal(4),
            reader.IsDBNull(5) ? null : reader.GetString(5),
            reader.GetDateTime(6)
        );
    }
}