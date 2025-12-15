using Area52Entertainment.Domain;
using Microsoft.Data.SqlClient;

namespace Area52Entertainment.Data;

public class DeelnemerRepository : IDeelnemerRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public DeelnemerRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Deelnemer> InsertAsync(Deelnemer deelnemer)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Deelnemer (Naam, Leeftijd, Email)
                                OUTPUT INSERTED.DeelnemerId
                                VALUES (@naam, @leeftijd, @email)";

        var p1 = command.CreateParameter();
        p1.ParameterName = "@naam";
        p1.Value = deelnemer.Naam;
        command.Parameters.Add(p1);

        var p2 = command.CreateParameter();
        p2.ParameterName = "@leeftijd";
        p2.Value = deelnemer.Leeftijd;
        command.Parameters.Add(p2);

        var p3 = command.CreateParameter();
        p3.ParameterName = "@email";
        p3.Value = deelnemer.Email;
        command.Parameters.Add(p3);

        var idObj = await command.ExecuteScalarAsync();
        deelnemer.DeelnemerId = Convert.ToInt32(idObj);

        return deelnemer;
    }
}