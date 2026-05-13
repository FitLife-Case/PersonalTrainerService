using FitLife.PersonalTrainer.API.Models;
using FitLife.PersonalTrainer.API.Repositories;

namespace FitLife.PersonalTrainer.API.Services;

public class NutritionPlanService : INutritionPlanService
{
    private readonly INutritionPlanRepository _repository;
    private readonly ILogger<NutritionPlanService> _logger;

    public NutritionPlanService(
        INutritionPlanRepository repository,
        ILogger<NutritionPlanService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<NutritionPlan?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _repository.GetByIdAsync(id, ct);

    public async Task<List<NutritionPlan>> GetByMemberIdAsync(Guid memberId, CancellationToken ct = default)
        => await _repository.GetByMemberIdAsync(memberId, ct);

    public async Task<List<NutritionPlan>> GetByTrainerIdAsync(Guid trainerId, CancellationToken ct = default)
        => await _repository.GetByTrainerIdAsync(trainerId, ct);

    public async Task<NutritionPlan> CreateAsync(NutritionPlan plan, CancellationToken ct = default)
    {
        var existingPlan = await _repository.GetActivePlanAsync(plan.MemberId, plan.TrainerId, ct);
        if (existingPlan is not null)
            throw new InvalidOperationException("Der er allerede en aktiv kostplan for dette medlem");

        plan.Id = Guid.NewGuid();
        plan.Status = PlanStatusNutrition.Active;
        plan.Meals = new List<Meal>();

        await _repository.AddAsync(plan, ct);
        _logger.LogInformation("NutritionPlan {PlanId} oprettet for Member {MemberId}", plan.Id, plan.MemberId);
        return plan;
    }

    public async Task UpdateAsync(Guid id, NutritionPlan updatedPlan, CancellationToken ct = default)
    {
        var plan = await _repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"NutritionPlan {id} ikke fundet");

        plan.Name = updatedPlan.Name;
        plan.DailyCalorieTarget = updatedPlan.DailyCalorieTarget;
        plan.StartDate = updatedPlan.StartDate;
        plan.EndDate = updatedPlan.EndDate;

        await _repository.UpdateAsync(plan, ct);
        _logger.LogInformation("NutritionPlan {PlanId} opdateret", id);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var plan = await _repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"NutritionPlan {id} ikke fundet");

        await _repository.DeleteAsync(id, ct);
        _logger.LogInformation("NutritionPlan {PlanId} slettet", id);
    }

    public async Task AddMealAsync(Guid planId, Meal meal, CancellationToken ct = default)
    {
        var plan = await _repository.GetByIdAsync(planId, ct)
            ?? throw new KeyNotFoundException($"NutritionPlan {planId} ikke fundet");

        meal.Id = Guid.NewGuid();
        plan.Meals.Add(meal);

        await _repository.UpdateAsync(plan, ct);
        _logger.LogInformation("Meal tilføjet til NutritionPlan {PlanId}", planId);
    }

    public async Task RemoveMealAsync(Guid planId, Guid mealId, CancellationToken ct = default)
    {
        var plan = await _repository.GetByIdAsync(planId, ct)
            ?? throw new KeyNotFoundException($"NutritionPlan {planId} ikke fundet");

        plan.Meals.RemoveAll(m => m.Id == mealId);
        await _repository.UpdateAsync(plan, ct);
        _logger.LogInformation("Meal {MealId} fjernet fra NutritionPlan {PlanId}", mealId, planId);
    }

    public async Task UpdateStatusAsync(Guid id, PlanStatusNutrition status, CancellationToken ct = default)
    {
        var plan = await _repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"NutritionPlan {id} ikke fundet");

        plan.Status = status;
        await _repository.UpdateAsync(plan, ct);
        _logger.LogInformation("NutritionPlan {PlanId} status opdateret til {Status}", id, status);
    }
}