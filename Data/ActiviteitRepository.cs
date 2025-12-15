using System.Data;
using Area52Entertainment.Domain;
using Microsoft.Data.SqlClient;

namespace Area52Entertainment.Data;

public class ActiviteitRepository : IActiviteitRepository
{
    private readonly SqlConnectionFactory _connectionFactory;
    private readonly IAnnuleringsBeleidRepository _annuleringsBeleidRepository;

    public ActiviteitRepository(SqlConnectionFactory connectionFactory, IAnnuleringsBeleidRepository annuleringsBeleidRepository)
    {
        _connectionFactory = connectionFactory;
        _annuleringsBeleidRepository = annuleringsBeleidRepository;
    }

    public async Task<IReadOnlyList<Activiteit>> GetAllAsync()
    {
        var activiteiten = new List<Activiteit>();

        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT ActiviteitId, Naam, DuurInMinuten, BasisPrijs, Type, MinimumLeeftijd, MaximumLeeftijd, LeeftijdsToeslag, MaxCapaciteit, AnnuleringsBeleidId
                                FROM Activiteit";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var activiteit = await MapActiviteitAsync(reader);
            activiteiten.Add(activiteit);
        }

        return activiteiten;
    }

    public async Task<Activiteit?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT ActiviteitId, Naam, DuurInMinuten, BasisPrijs, Type, MinimumLeeftijd, MaximumLeeftijd, LeeftijdsToeslag, MaxCapaciteit, AnnuleringsBeleidId
                                FROM Activiteit
                                WHERE ActiviteitId = @id";
        var p = command.CreateParameter();
        p.ParameterName = "@id";
        p.Value = id;
        command.Parameters.Add(p);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return await MapActiviteitAsync(reader);
        }

        return null;
    }

    public async Task<int> GetTotaalIngeschrevenVoorShowAsync(int activiteitId)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT ISNULL(SUM(AantalPersonen), 0) 
                                FROM Reservering 
                                WHERE ActiviteitId = @id AND Status = 'Actief'";
        var p = command.CreateParameter();
        p.ParameterName = "@id";
        p.Value = activiteitId;
        command.Parameters.Add(p);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    private async Task<Activiteit> MapActiviteitAsync(IDataRecord record)
    {
        int activiteitId = record.GetInt32(0);
        string naam = record.GetString(1);
        int duur = record.GetInt32(2);
        decimal basisPrijs = record.GetDecimal(3);
        string type = record.GetString(4);
        int? minLeeftijd = record.IsDBNull(5) ? null : record.GetInt32(5);
        int? maxLeeftijd = record.IsDBNull(6) ? null : record.GetInt32(6);
        decimal? leeftijdsToeslag = record.IsDBNull(7) ? null : record.GetDecimal(7);
        int? maxCapaciteit = record.IsDBNull(8) ? null : record.GetInt32(8);
        int annuleringsBeleidId = record.GetInt32(9);

        var beleid = await _annuleringsBeleidRepository.GetByIdAsync(annuleringsBeleidId);

        Activiteit activiteit;
        if (string.Equals(type, "Workshop", StringComparison.OrdinalIgnoreCase))
        {
            activiteit = new Workshop
            {
                MinimumLeeftijd = minLeeftijd,
                MaximumLeeftijd = maxLeeftijd,
                LeeftijdsToeslag = leeftijdsToeslag
            };
        }
        else
        {
            activiteit = new Show
            {
                MaxCapaciteit = maxCapaciteit
            };
        }

        activiteit.ActiviteitId = activiteitId;
        activiteit.Naam = naam;
        activiteit.DuurInMinuten = duur;
        activiteit.BasisPrijs = basisPrijs;
        activiteit.Type = type;
        activiteit.AnnuleringsBeleid = beleid;

        return activiteit;
    }
}