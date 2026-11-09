using System.Data;
using Ewallet.Api.Models;
using Microsoft.Data.SqlClient;

namespace Ewallet.Api.Repositories;

public sealed class SqlWalletRepository : IWalletRepository
{
    private readonly string _connection;

    public SqlWalletRepository(IConfiguration configuration)
    {
        _connection = configuration.GetConnectionString("Ewallet") ?? throw new InvalidOperationException("Connection hilang");
    }

    public async Task<IReadOnlyList<Wallets>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, UsersId, WalletNumber, Balance
            FROM dbo.Wallets
        """;

        var wallets = new List<Wallets>();

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            wallets.Add(ReadWallets(reader));
        }

        return wallets;
    }

    public async Task<Wallets?> GetByIdUsers(int usersId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, UsersId, WalletNumber, Balance FROM dbo.Wallets
            WHERE UsersId = @usersId
        """;

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);

        command.Parameters.Add("@usersId", SqlDbType.Int).Value = usersId;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return ReadWallets(reader);
    }

    public async Task<bool> UpdateAsync(Wallets wallets, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE dbo.Wallets SET 
                Balance = @balance
            WHERE Id = @id
        """;

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);

        AddWalletParameter(command, wallets);

        var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);

        return affectedRows == 1;
    }

    private static Wallets ReadWallets(SqlDataReader reader)
    {
        return new Wallets(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetString(2),
            reader.GetDecimal(3)
        );
    }

    private static void AddWalletParameter(SqlCommand command, Wallets wallets)
    {
        command.Parameters.Add("@id", SqlDbType.Int).Value = wallets.Id;

        var balanceParameter = command.Parameters.Add("@balance", SqlDbType.Decimal);
        balanceParameter.Precision = 18;
        balanceParameter.Scale = 2;
        balanceParameter.Value = wallets.Balance;
    }
}