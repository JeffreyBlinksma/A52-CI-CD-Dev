namespace Area52Entertainment.Domain;

public abstract class Activiteit
{
    public int ActiviteitId { get; set; }
    public string Naam { get; set; } = string.Empty;
    public int DuurInMinuten { get; set; }
    public decimal BasisPrijs { get; set; }
    public string Type { get; set; } = string.Empty;
    public int? MinimumLeeftijd { get; set; }
    public int? MaximumLeeftijd { get; set; }
    public decimal? LeeftijdsToeslag { get; set; }
    public int? MaxCapaciteit { get; set; }
    public IAnnuleringsBeleid AnnuleringsBeleid { get; set; } = new GratisAnnuleringsBeleid();

    public abstract decimal BerekenPrijs(Deelnemer deelnemer, int aantalPersonen);

    public virtual void ValideerVoorInschrijving(Deelnemer deelnemer, int aantalPersonen, int reedsIngeschreven)
    {
        // Default does nothing; subclasses implement specifieke checks
    }
}

public class Show : Activiteit
{
    public override void ValideerVoorInschrijving(Deelnemer deelnemer, int aantalPersonen, int reedsIngeschreven)
    {
        if (MaxCapaciteit.HasValue)
        {
            var totaalNaInschrijving = reedsIngeschreven + aantalPersonen;
            if (totaalNaInschrijving > MaxCapaciteit.Value)
            {
                throw new DomeinException("Deze show is vol. Er zijn onvoldoende plaatsen beschikbaar.");
            }
        }
    }

    public override decimal BerekenPrijs(Deelnemer deelnemer, int aantalPersonen)
    {
        return BasisPrijs * aantalPersonen;
    }
}

public class Workshop : Activiteit
{
    public override void ValideerVoorInschrijving(Deelnemer deelnemer, int aantalPersonen, int reedsIngeschreven)
    {
        if (!MinimumLeeftijd.HasValue || !MaximumLeeftijd.HasValue)
        {
            throw new DomeinException("Leeftijdsgrenzen voor deze workshop zijn niet correct geconfigureerd.");
        }

        if (deelnemer.Leeftijd < MinimumLeeftijd.Value || deelnemer.Leeftijd > MaximumLeeftijd.Value)
        {
            throw new DomeinException($"Deze workshop is voor deelnemers tussen {MinimumLeeftijd} en {MaximumLeeftijd} jaar.");
        }
    }

    public override decimal BerekenPrijs(Deelnemer deelnemer, int aantalPersonen)
    {
        decimal prijsPerPersoon = BasisPrijs;

        if (LeeftijdsToeslag.HasValue && MinimumLeeftijd.HasValue && MaximumLeeftijd.HasValue)
        {
            // Eenvoudige interpretatie: toeslag geldt voor bovenste helft van de leeftijdsrange
            int midden = (MinimumLeeftijd.Value + MaximumLeeftijd.Value) / 2;
            if (deelnemer.Leeftijd >= midden)
            {
                prijsPerPersoon += BasisPrijs * (LeeftijdsToeslag.Value / 100m);
            }
        }

        return prijsPerPersoon * aantalPersonen;
    }
}