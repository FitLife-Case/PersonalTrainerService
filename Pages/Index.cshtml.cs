using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FitLife.PersonalTrainer.API.Models;
using FitLife.PersonalTrainer.API.Services;

namespace FitLife.PersonalTrainer.API.Pages;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ITrainerService _trainerService;
    private readonly ITrainingPlanService _trainingPlanService;
    private readonly INutritionPlanService _nutritionPlanService;
    private readonly ILogger<IndexModel> _logger;

    public List<Trainer> Trainers { get; set; } = new();
    public List<TrainingPlan> TrainingPlans { get; set; } = new();
    public List<NutritionPlan> NutritionPlans { get; set; } = new();

    [BindProperty]
    public Trainer NewTrainer { get; set; } = new();

    [BindProperty]
    public TrainingPlan NewTrainingPlan { get; set; } = new();

    [BindProperty]
    public NutritionPlan NewNutritionPlan { get; set; } = new();

    public IndexModel(
        ITrainerService trainerService,
        ITrainingPlanService trainingPlanService,
        INutritionPlanService nutritionPlanService,
        ILogger<IndexModel> logger)
    {
        _trainerService = trainerService;
        _trainingPlanService = trainingPlanService;
        _nutritionPlanService = nutritionPlanService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        Trainers = await _trainerService.GetAllAsync() ?? new();
    }

    public async Task OnGetTrainerPlansAsync(Guid trainerId)
    {
        Trainers = await _trainerService.GetAllAsync() ?? new();
        TrainingPlans = await _trainingPlanService.GetByTrainerIdAsync(trainerId) ?? new();
        NutritionPlans = await _nutritionPlanService.GetByTrainerIdAsync(trainerId) ?? new();
    }

    public async Task<IActionResult> OnPostCreateTrainerAsync()
    {
        NewTrainer.IsActive = true;
        NewTrainer.AvailabilitySlots = new();
        NewTrainer.Specialties = new();
        NewTrainer.WorksAtCenterIds = new();
        await _trainerService.CreateAsync(NewTrainer);
        return Redirect("/personaltrainer/");
    }

    public async Task<IActionResult> OnPostDeleteTrainerAsync(Guid id)
    {
        await _trainerService.DeleteAsync(id);
        return Redirect("/personaltrainer/");
    }

    public async Task<IActionResult> OnPostCreateTrainingPlanAsync()
    {
        NewTrainingPlan.Exercises = new();
        await _trainingPlanService.CreateAsync(NewTrainingPlan);
        return Redirect("/personaltrainer/");
    }

    public async Task<IActionResult> OnPostDeleteTrainingPlanAsync(Guid id)
    {
        await _trainingPlanService.DeleteAsync(id);
        return Redirect("/personaltrainer/");
    }

    public async Task<IActionResult> OnPostCreateNutritionPlanAsync()
    {
        NewNutritionPlan.Meals = new();
        await _nutritionPlanService.CreateAsync(NewNutritionPlan);
        return Redirect("/personaltrainer/");
    }

    public async Task<IActionResult> OnPostDeleteNutritionPlanAsync(Guid id)
    {
        await _nutritionPlanService.DeleteAsync(id);
        return Redirect("/personaltrainer/");
    }
}