using Area52Entertainment.Domain;
using Microsoft.Data.SqlClient;

namespace Area52Entertainment.Data;

public class ReserveringRepository : IReserveringRepository
{
    private readonly SqlConnectionFactory _connectionFactory;
    private readonly IActiviteitRepository _activiteitRepository;

    public ReserveringRepository(SqlConnectionFactory connectionFactory, IActiviteitRepository activiteitRepository)
    {
        _connectionFactory = connectionFactory;
        _activiteitRepository = activiteitRepository;
    }

    public async Task InsertAsync(Reservering reservering, int activiteitId, int deelnemerId)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Reservering (DeelnemerId, ActiviteitId, StartTijd, AantalPersonen, TotaalPrijs, Status)
                                VALUES (@deelnemerId, @activiteitId, @startTijd, @aantal, @totaalPrijs, @status)";

        var p1 = command.CreateParameter();
        p1.ParameterName = "@deelnemerId";
        p1.Value = deelnemerId;
        command.Parameters.Add(p1);

        var p2 = command.CreateParameter();
        p2.ParameterName = "@activiteitId";
        p2.Value = activiteitId;
        command.Parameters.Add(p2);

        var p3 = command.CreateParameter();
        p3.ParameterName = "@startTijd";
        p3.Value = reservering.StartTijd;
        command.Parameters.Add(p3);

        var p4 = command.CreateParameter();
        p4.ParameterName = "@aantal";
        p4.Value = reservering.AantalPersonen;
        command.Parameters.Add(p4);

        var p5 = command.CreateParameter();
        p5.ParameterName = "@totaalPrijs";
        p5.Value = reservering.TotaalPrijs;
        command.Parameters.Add(p5);

        var p6 = command.CreateParameter();
        p6.ParameterName = "@status";
        p6.Value = reservering.Status;
        command.Parameters.Add(p6);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<IReadOnlyList<Reservering>> GetAllAsync()
    {
        var result = new List<Reservering>();

        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT r.ReserveringId, r.StartTijd, r.AantalPersonen, r.TotaalPrijs, r.Status,
                                       d.DeelnemerId, d.Naam, d.Leeftijd, d.Email,
                                       a.ActiviteitId, a.Naam, a.DuurInMinuten, a.BasisPrijs, a.Type, a.MinimumLeeftijd, a.MaximumLeeftijd, a.LeeftijdsToeslag, a.MaxCapaciteit, a.AnnuleringsBeleidId
                                FROM Reservering r
                                JOIN Deelnemer d ON r.DeelnemerId = d.DeelnemerId
                                JOIN Activiteit a ON r.ActiviteitId = a.ActiviteitId";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var reservering = await MapReserveringAsync(reader);
            result.Add(reservering);
        }

        return result;
    }

    public async Task<Reservering?> GetByIdAsync(int reserveringId)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT r.ReserveringId, r.StartTijd, r.AantalPersonen, r.TotaalPrijs, r.Status,
                                       d.DeelnemerId, d.Naam, d.Leeftijd, d.Email,
                                       a.ActiviteitId, a.Naam, a.DuurInMinuten, a.BasisPrijs, a.Type, a.MinimumLeeftijd, a.MaximumLeeftijd, a.LeeftijdsToeslag, a.MaxCapaciteit, a.AnnuleringsBeleidId
                                FROM Reservering r
                                JOIN Deelnemer d ON r.DeelnemerId = d.DeelnemerId
                                JOIN Activiteit a ON r.ActiviteitId = a.ActiviteitId
                                WHERE r.ReserveringId = @id";
        var p = command.CreateParameter();
        p.ParameterName = "@id";
        p.Value = reserveringId;
        command.Parameters.Add(p);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return await MapReserveringAsync(reader);
        }

        return null;
    }

    public async Task UpdateStatusAsync(int reserveringId, string status)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"UPDATE Reservering SET Status = @status WHERE ReserveringId = @id";

        var p1 = command.CreateParameter();
        p1.ParameterName = "@status";
        p1.Value = status;
        command.Parameters.Add(p1);

        var p2 = command.CreateParameter();
        p2.ParameterName = "@id";
        p2.Value = reserveringId;
        command.Parameters.Add(p2);

        await command.ExecuteNonQueryAsync();
    }

    private async Task<Reservering> MapReserveringAsync(System.Data.IDataRecord record)
    {
        int reserveringId = record.GetInt32(0);
        DateTime startTijd = record.GetDateTime(1);
        int aantalPersonen = record.GetInt32(2);
        decimal totaalPrijs = record.GetDecimal(3);
        string status = record.GetString(4);

        var deelnemer = new Deelnemer
        {
            DeelnemerId = record.GetInt32(5),
            Naam = record.GetString(6),
            Leeftijd = record.GetInt32(7),
            Email = record.GetString(8)
        };

        int activiteitId = record.GetInt32(9);
        // Gebruik ActiviteitRepository voor beleid-mapping
        var activiteit = await _activiteitRepository.GetByIdAsync(activiteitId);
        if (activiteit == null)
        {
            throw new InvalidOperationException("Activiteit niet gevonden.");
        }

        return new Reservering
        {
            ReserveringId = reserveringId,
            Deelnemer = deelnemer,
            Activiteit = activiteit,
            StartTijd = startTijd,
            AantalPersonen = aantalPersonen,
            TotaalPrijs = totaalPrijs,
            Status = status
        };
    }
}