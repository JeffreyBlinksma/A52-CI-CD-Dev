using Area52Entertainment.Domain;
using Area52Entertainment.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Area52Entertainment.Pages.Activiteiten;

public class IndexModel : PageModel
{
    private readonly ActiviteitService _activiteitService;

    public IndexModel(ActiviteitService activiteitService)
    {
        _activiteitService = activiteitService;
    }

    public IReadOnlyList<Activiteit> Activiteiten { get; private set; } = new List<Activiteit>();

    public async Task OnGetAsync()
    {
        Activiteiten = await _activiteitService.GetAlleActiviteitenAsync();
    }
}