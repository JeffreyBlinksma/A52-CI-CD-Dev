namespace Area52Entertainment.Domain;

public interface IAnnuleringsBeleid
{
    decimal BerekenAnnuleringsKosten(decimal totaalPrijs, bool binnen24Uur);
    string Beschrijving { get; }
}

public class GratisAnnuleringsBeleid : IAnnuleringsBeleid
{
    public string Beschrijving => "Gratis annuleren";

    public decimal BerekenAnnuleringsKosten(decimal totaalPrijs, bool binnen24Uur) => 0m;
}

public class VastBedragAnnuleringsBeleid : IAnnuleringsBeleid
{
    public decimal Bedrag { get; }

    public VastBedragAnnuleringsBeleid(decimal bedrag)
    {
        Bedrag = bedrag;
    }

    public string Beschrijving => $"Vast bedrag ({Bedrag:C}) binnen 24 uur";

    public decimal BerekenAnnuleringsKosten(decimal totaalPrijs, bool binnen24Uur)
    {
        if (!binnen24Uur)
        {
            return 0m;
        }

        return Math.Min(Bedrag, totaalPrijs);
    }
}

public class PercentageAnnuleringsBeleid : IAnnuleringsBeleid
{
    public decimal Percentage { get; }

    public PercentageAnnuleringsBeleid(decimal percentage)
    {
        Percentage = percentage;
    }

    public string Beschrijving => $"{Percentage}% annuleringskosten";

    public decimal BerekenAnnuleringsKosten(decimal totaalPrijs, bool binnen24Uur)
    {
        return totaalPrijs * (Percentage / 100m);
    }
}