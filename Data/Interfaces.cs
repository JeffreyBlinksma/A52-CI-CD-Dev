using Area52Entertainment.Domain;

namespace Area52Entertainment.Data;

public interface IActiviteitRepository
{
    Task<IReadOnlyList<Activiteit>> GetAllAsync();
    Task<Activiteit?> GetByIdAsync(int id);
    Task<int> GetTotaalIngeschrevenVoorShowAsync(int activiteitId);
}

public interface IDeelnemerRepository
{
    Task<Deelnemer> InsertAsync(Deelnemer deelnemer);
}

public interface IReserveringRepository
{
    Task InsertAsync(Reservering reservering, int activiteitId, int deelnemerId);
    Task<IReadOnlyList<Reservering>> GetAllAsync();
    Task<Reservering?> GetByIdAsync(int reserveringId);
    Task UpdateStatusAsync(int reserveringId, string status);
}

public interface IAnnuleringsBeleidRepository
{
    Task<IAnnuleringsBeleid> GetByIdAsync(int annuleringsBeleidId);
}