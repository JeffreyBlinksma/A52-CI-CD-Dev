using Area52Entertainment.Domain;
using Area52Entertainment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Area52Entertainment.Pages.Reserveringen;

public class AnnulerenModel : PageModel
{
    private readonly ReserveringService _reserveringService;

    public AnnulerenModel(ReserveringService reserveringService)
    {
        _reserveringService = reserveringService;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public Reservering? Reservering { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Reservering = await _reserveringService.GetReserveringAsync(Id);
        if (Reservering == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Reservering = await _reserveringService.GetReserveringAsync(Id);
        if (Reservering == null)
        {
            return NotFound();
        }

        try
        {
            var (kosten, terug) = await _reserveringService.AnnuleerReserveringAsync(Id, DateTime.Now);

            TempData["AnnuleringsResultaat"] =
                $"Annulering verwerkt. Kosten: {kosten:C}. Terug te betalen bedrag: {terug:C}.";
        }
        catch (DomeinException ex)
        {
            TempData["AnnuleringsResultaat"] = ex.Message;
        }

        return RedirectToPage("Index");
    }
}