using FitLife.PersonalTrainer.API.Models;

namespace FitLife.PersonalTrainer.API.Services;

public interface INutritionPlanService
{
    Task<NutritionPlan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<NutritionPlan>> GetByMemberIdAsync(Guid memberId, CancellationToken ct = default);
    Task<List<NutritionPlan>> GetByTrainerIdAsync(Guid trainerId, CancellationToken ct = default);
    Task<NutritionPlan> CreateAsync(NutritionPlan plan, CancellationToken ct = default);
    Task UpdateAsync(Guid id, NutritionPlan plan, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task AddMealAsync(Guid planId, Meal meal, CancellationToken ct = default);
    Task RemoveMealAsync(Guid planId, Guid mealId, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid id, PlanStatusNutrition status, CancellationToken ct = default);
}