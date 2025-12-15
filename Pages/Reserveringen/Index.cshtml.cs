using Area52Entertainment.Domain;
using Area52Entertainment.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Area52Entertainment.Pages.Reserveringen;

public class IndexModel : PageModel
{
    private readonly ReserveringService _reserveringService;

    public IndexModel(ReserveringService reserveringService)
    {
        _reserveringService = reserveringService;
    }

    public IReadOnlyList<Reservering> Reserveringen { get; private set; } = new List<Reservering>();

    public async Task OnGetAsync()
    {
        Reserveringen = await _reserveringService.GetAlleReserveringenAsync();
    }
}