using Area52Entertainment.Data;
using Area52Entertainment.Domain;

namespace Area52Entertainment.Services;

public class ActiviteitService
{
    private readonly IActiviteitRepository _activiteitRepository;
    private readonly IReserveringRepository _reserveringRepository;

    public ActiviteitService(IActiviteitRepository activiteitRepository, IReserveringRepository reserveringRepository)
    {
        _activiteitRepository = activiteitRepository;
        _reserveringRepository = reserveringRepository;
    }

    public Task<IReadOnlyList<Activiteit>> GetAlleActiviteitenAsync()
        => _activiteitRepository.GetAllAsync();

    public Task<Activiteit?> GetActiviteitAsync(int id)
        => _activiteitRepository.GetByIdAsync(id);

    public Task<int> GetTotaalIngeschrevenVoorShowAsync(int activiteitId)
        => _activiteitRepository.GetTotaalIngeschrevenVoorShowAsync(activiteitId);
}

public class ReserveringService
{
    private readonly IActiviteitRepository _activiteitRepository;
    private readonly IDeelnemerRepository _deelnemerRepository;
    private readonly IReserveringRepository _reserveringRepository;

    public ReserveringService(
        IActiviteitRepository activiteitRepository,
        IDeelnemerRepository deelnemerRepository,
        IReserveringRepository reserveringRepository)
    {
        _activiteitRepository = activiteitRepository;
        _deelnemerRepository = deelnemerRepository;
        _reserveringRepository = reserveringRepository;
    }

    public async Task<Reservering> MaakReserveringAsync(string naam, int leeftijd, string email, int activiteitId, int aantalPersonen, DateTime startTijd)
    {
        var activiteit = await _activiteitRepository.GetByIdAsync(activiteitId)
                        ?? throw new DomeinException("Activiteit niet gevonden.");

        var deelnemer = new Deelnemer
        {
            Naam = naam,
            Leeftijd = leeftijd,
            Email = email
        };

        int reedsIngeschreven = 0;
        if (activiteit is Show)
        {
            reedsIngeschreven = await _activiteitRepository.GetTotaalIngeschrevenVoorShowAsync(activiteitId);
        }

        var reservering = Reservering.MaakNieuwe(deelnemer, activiteit, aantalPersonen, startTijd, reedsIngeschreven);

        var opgeslagenDeelnemer = await _deelnemerRepository.InsertAsync(deelnemer);
        await _reserveringRepository.InsertAsync(reservering, activiteitId, opgeslagenDeelnemer.DeelnemerId);

        return reservering;
    }

    public Task<IReadOnlyList<Reservering>> GetAlleReserveringenAsync()
        => _reserveringRepository.GetAllAsync();

    public Task<Reservering?> GetReserveringAsync(int reserveringId)
        => _reserveringRepository.GetByIdAsync(reserveringId);

    public async Task<(decimal annuleringsKosten, decimal terugTeBetalen)> AnnuleerReserveringAsync(int reserveringId, DateTime annuleringMoment)
    {
        var reservering = await _reserveringRepository.GetByIdAsync(reserveringId)
                          ?? throw new DomeinException("Reservering niet gevonden.");

        var (kosten, terug) = reservering.Annuleer(annuleringMoment);
        await _reserveringRepository.UpdateStatusAsync(reserveringId, reservering.Status);

        return (kosten, terug);
    }
}