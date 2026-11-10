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

    public async Task<bool> TopUpAddAsync(int usersId, decimal amount, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE dbo.Wallets SET
                Balance = Balance + @amount
            WHERE UsersId = @usersId
        """;

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);

        command.Parameters.Add("@usersId", SqlDbType.Int).Value = usersId;
        var parameter = command.Parameters.Add("@amount", SqlDbType.Decimal);
        parameter.Precision = 18;
        parameter.Scale = 2;
        parameter.Value = amount;

        var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
        return affectedRows == 1;
    }

    public async Task<bool> TransferAsync(int usersId, decimal amount, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE dbo.Wallets SET
             Balance = Balance - @amount
            WHERE UsersId = @usersId
        """;

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);

        command.Parameters.Add("@usersId", SqlDbType.Int).Value = usersId;
        var parameter = command.Parameters.Add("@amount", SqlDbType.Decimal);
        parameter.Precision = 18;
        parameter.Scale = 2;
        parameter.Value = amount;

        var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
        return affectedRows == 1;
    }

    public async Task<bool> ReduceBalanceAsync(int id, decimal amount, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE dbo.Wallets
            SET Balance = Balance - @amount
            WHERE Id = @id
            AND Balance >= @amount
        """;

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);

        command.Parameters.Add("@id", SqlDbType.Int).Value = id;
        var parameter = command.Parameters.Add("@amount", SqlDbType.Decimal);
        parameter.Precision = 18;
        parameter.Scale = 2;
        parameter.Value = amount;

        return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
    }

    public async Task<bool> AddBalanceAsync(int id, decimal amount, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE dbo.Wallets
            SET Balance = Balance + @amount
            WHERE Id = @id
            """;

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);

        command.Parameters.Add("@id", SqlDbType.Int).Value = id;
        var parameter = command.Parameters.Add("@amount", SqlDbType.Decimal);
        parameter.Precision = 18;
        parameter.Scale = 2;
        parameter.Value = amount;

        return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
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
}