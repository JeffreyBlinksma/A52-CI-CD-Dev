using System.ComponentModel.DataAnnotations;
using Area52Entertainment.Domain;
using Area52Entertainment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Area52Entertainment.Pages.Reserveringen;

public class CreateModel : PageModel
{
    private readonly ActiviteitService _activiteitService;
    private readonly ReserveringService _reserveringService;

    public CreateModel(ActiviteitService activiteitService, ReserveringService reserveringService)
    {
        _activiteitService = activiteitService;
        _reserveringService = reserveringService;
    }

    [BindProperty(SupportsGet = true)]
    public int ActiviteitId { get; set; }

    public Activiteit? Activiteit { get; private set; }

    [BindProperty]
    [Required]
    [Display(Name = "Naam")]
    public string Naam { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [Range(0, 120)]
    [Display(Name = "Leeftijd")]
    public int Leeftijd { get; set; }

    [BindProperty]
    [Required]
    [EmailAddress]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [Range(1, 100)]
    [Display(Name = "Aantal personen")]
    public int AantalPersonen { get; set; }

    [BindProperty]
    [Required]
    [Display(Name = "Starttijd")]
    public DateTime StartTijd { get; set; } = DateTime.Now.AddDays(1).Date.AddHours(19);

    public string? Foutmelding { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Activiteit = await _activiteitService.GetActiviteitAsync(ActiviteitId);
        if (Activiteit == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Activiteit = await _activiteitService.GetActiviteitAsync(ActiviteitId);
        if (Activiteit == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var reservering = await _reserveringService.MaakReserveringAsync(Naam, Leeftijd, Email, ActiviteitId, AantalPersonen, StartTijd);
            TempData["SuccesMessage"] = $"Reservering succesvol aangemaakt. Totaalprijs: {reservering.TotaalPrijs:C}.";
            return RedirectToPage("Index");
        }
        catch (DomeinException ex)
        {
            Foutmelding = ex.Message;
            return Page();
        }
    }
}