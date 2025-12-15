namespace Area52Entertainment.Domain;

public class Reservering
{
    public int ReserveringId { get; set; }
    public Deelnemer Deelnemer { get; set; } = new Deelnemer();
    public Activiteit Activiteit { get; set; } = null!;
    public DateTime StartTijd { get; set; }
    public int AantalPersonen { get; set; }
    public decimal TotaalPrijs { get; set; }
    public string Status { get; set; } = "Actief";

    public static Reservering MaakNieuwe(Deelnemer deelnemer, Activiteit activiteit, int aantalPersonen, DateTime startTijd, int reedsIngeschreven)
    {
        deelnemer.Valideer();

        if (aantalPersonen <= 0)
        {
            throw new DomeinException("Aantal personen moet groter dan nul zijn.");
        }

        activiteit.ValideerVoorInschrijving(deelnemer, aantalPersonen, reedsIngeschreven);

        var totaalPrijs = activiteit.BerekenPrijs(deelnemer, aantalPersonen);

        return new Reservering
        {
            Deelnemer = deelnemer,
            Activiteit = activiteit,
            AantalPersonen = aantalPersonen,
            StartTijd = startTijd,
            TotaalPrijs = totaalPrijs,
            Status = "Actief"
        };
    }

    public (decimal annuleringsKosten, decimal terugTeBetalen) Annuleer(DateTime annuleringsMoment)
    {
        if (Status == "Geannuleerd")
        {
            throw new DomeinException("Deze reservering is al geannuleerd.");
        }

        bool binnen24Uur = (StartTijd - annuleringsMoment).TotalHours <= 24;

        decimal kosten = Activiteit.AnnuleringsBeleid.BerekenAnnuleringsKosten(TotaalPrijs, binnen24Uur);
        if (kosten < 0)
        {
            kosten = 0;
        }
        if (kosten > TotaalPrijs)
        {
            kosten = TotaalPrijs;
        }

        decimal terug = TotaalPrijs - kosten;
        Status = "Geannuleerd";

        return (kosten, terug);
    }
}