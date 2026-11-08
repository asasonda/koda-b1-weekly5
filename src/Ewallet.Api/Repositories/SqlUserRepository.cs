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

    public async Task<int?> AddUserWithWalletAsync(Users users, string walletNumber, string pin, CancellationToken cancellationToken)
    {
        const string sqlUser = """
            INSERT INTO dbo.Users(Name, Email, Password, Phone)
            OUTPUT INSERTED.id
            VALUES(@name, @email, @password, @phone)
        """;

        const string sqlWallet = """
            INSERT INTO dbo.Wallets(UsersId, WalletNumber, Pin) VALUES
            (@usersId, @walletNumber, @pin)
        """;

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await using var userCommand = new SqlCommand(sqlUser, connection, transaction);
            AddUserParameter(userCommand, users);

            var userId = Convert.ToInt32(await userCommand.ExecuteScalarAsync(cancellationToken));

            await using var walletCommand = new SqlCommand(sqlWallet, connection, transaction);
            AddWalletParameter(walletCommand, userId, walletNumber, pin);

            var affectedRows = await walletCommand.ExecuteNonQueryAsync(cancellationToken);

            if (affectedRows != 1)
            {
                await transaction.RollbackAsync(cancellationToken);
                return null;
            }
            await transaction.CommitAsync(cancellationToken);

            return userId;

        }
        catch (SqlException e)when (e.Number is 2601 or 2627)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            return null;
        }
    }

    public async Task<IReadOnlyList<Users>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Name, Email, Password, Phone FROM dbo.Users join dbo.Wallets w on 
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
            SELECT Name, Email, Password, Phone FROM dbo.Users WHERE id =@id
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
                Password = @password,
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
        command.Parameters.Add("@password", SqlDbType.NVarChar, 255).Value = users.Password;
        command.Parameters.Add("@phone", SqlDbType.NVarChar, 20).Value = users.Phone;
    }

    private static void AddWalletParameter(SqlCommand command, int userId, string walletNumber, string pin)
    {
        command.Parameters.Add("@usersId", SqlDbType.Int).Value = userId;
        command.Parameters.Add("@walletNumber", SqlDbType.NVarChar, 50).Value = walletNumber;
        command.Parameters.Add("@pin", SqlDbType.NVarChar, 8).Value = pin;
    }

    private static void AddUserId(SqlCommand command, int id)
    {
        command.Parameters.Add("@id", SqlDbType.Int).Value = id;
    }

    private static Users ReadUsers(SqlDataReader reader)
    {
        return new Users(
            reader.GetString(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3)
        );
    }
}