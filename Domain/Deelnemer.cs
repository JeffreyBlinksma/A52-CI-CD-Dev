namespace Area52Entertainment.Domain;

public class Deelnemer
{
    public int DeelnemerId { get; set; }
    public string Naam { get; set; } = string.Empty;
    public int Leeftijd { get; set; }
    public string Email { get; set; } = string.Empty;

    public void Valideer()
    {
        if (string.IsNullOrWhiteSpace(Naam))
        {
            throw new DomeinException("Naam is verplicht.");
        }

        if (Leeftijd < 0)
        {
            throw new DomeinException("Leeftijd kan niet negatief zijn.");
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            throw new DomeinException("E-mailadres is verplicht.");
        }
    }
}