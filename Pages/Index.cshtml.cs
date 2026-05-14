using Microsoft.AspNetCore.Mvc.RazorPages;
using FitLife.PersonalTrainer.API.Models;
using FitLife.PersonalTrainer.API.Services;

namespace FitLife.PersonalTrainer.API.Pages;

public class IndexModel : PageModel
{
    private readonly ITrainerService _trainerService;
    private readonly ILogger<IndexModel> _logger;

    public List<Trainer>? Trainers { get; set; }

    public IndexModel(ITrainerService trainerService, ILogger<IndexModel> logger)
    {
        _trainerService = trainerService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Trainers = await _trainerService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fejl ved hentning af trænere");
        }
    }
}