using System.Data;
using Ewallet.Api.Models;
using Microsoft.Data.SqlClient;

namespace Ewallet.Api.Repositories;

public sealed class SqlUserRepository : IUserRepository
{
    private readonly string _connection;

    public SqlUserRepository(IConfiguration configuration)
    {
        _connection = configuration.GetConnectionString("Ewallet") ?? throw new InvalidOperationException("Connection hilang");
    }

    public async Task<bool> AddUserWithWalletAsync(Users users, Wallets wallets, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO dbo.Users(Name, Email, Phone) 
            VALUES (@name, @email, @phone);

            DECLARE @newUserId INT = SCOPE_IDENTITY();

            INSERT INTO dbo.Wallets(UsersId, WalletNumber) 
            VALUES (@newUserId, @walletNumber);
        """;

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        command.Transaction = (SqlTransaction)transaction;

        command.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = users.Name;
        command.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = users.Email;
        command.Parameters.Add("@phone", SqlDbType.NVarChar, 20).Value = users.Phone;
        command.Parameters.Add("@walletNumber", SqlDbType.NVarChar, 50).Value = wallets.WalletNumber;

        try
        {
            var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
            if (affectedRows == 2)
            {
                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }
        catch (SqlException e) when (e.Number is 2601 or 2627)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }
        catch (SqlException e)
        {
            await transaction.RollbackAsync(cancellationToken);
            Console.WriteLine($"SQL Error tidak terduga: {e.Number} - {e.Message}");
            return false;
        }
    }

    public async Task<IReadOnlyList<Users>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, Name, Email, Phone FROM dbo.Users 
        """;

        var users = new List<Users>();

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while(await reader.ReadAsync(cancellationToken))
        {
            users.Add(ReadUsers(reader));
        }
        return users;
    }

    public async Task<Users?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, Name, Email, Phone FROM dbo.Users WHERE id =@id
        """;

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);

        AddUserId(command, id);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }
        return ReadUsers(reader);

    }

    public async Task<bool> UpdateUserAsync(Users users, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE dbo.Users SET
                Name = @name,
                Email = @email,
                Phone = @phone
            WHERE Id = @id
        """;

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        AddUserParameter(command, users);

        var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
        return affectedRows == 1;
    }

    private static void AddUserParameter(SqlCommand command, Users users)
    {
        AddUserId(command, users.Id);
        command.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = users.Name;
        command.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = users.Email;
        command.Parameters.Add("@phone", SqlDbType.NVarChar, 20).Value = users.Phone;
    }

    private static void AddUserId(SqlCommand command, int id)
    {
        command.Parameters.Add("@id", SqlDbType.Int).Value = id;
    }

    private static Users ReadUsers(SqlDataReader reader)
    {
        return new Users(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3)
        );
    }
}