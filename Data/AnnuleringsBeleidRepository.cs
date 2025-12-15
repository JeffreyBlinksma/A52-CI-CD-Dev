using Area52Entertainment.Domain;

namespace Area52Entertainment.Data;

public class AnnuleringsBeleidRepository : IAnnuleringsBeleidRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public AnnuleringsBeleidRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IAnnuleringsBeleid> GetByIdAsync(int annuleringsBeleidId)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT Type, Waarde
                                FROM AnnuleringsBeleid
                                WHERE AnnuleringsBeleidId = @id";
        var p = command.CreateParameter();
        p.ParameterName = "@id";
        p.Value = annuleringsBeleidId;
        command.Parameters.Add(p);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            string type = reader.GetString(0);
            decimal waarde = reader.GetDecimal(1);

            return type.ToLower() switch
            {
                "gratis" => new GratisAnnuleringsBeleid(),
                "vast" => new VastBedragAnnuleringsBeleid(waarde),
                "percentage" => new PercentageAnnuleringsBeleid(waarde),
                _ => new GratisAnnuleringsBeleid()
            };
        }

        throw new InvalidOperationException("Annuleringsbeleid niet gevonden.");
    }
}