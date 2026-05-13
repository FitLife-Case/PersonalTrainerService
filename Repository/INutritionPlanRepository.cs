using FitLife.PersonalTrainer.API.Models;

namespace FitLife.PersonalTrainer.API.Repositories;

public interface INutritionPlanRepository
{
    Task<NutritionPlan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<NutritionPlan>> GetByMemberIdAsync(Guid memberId, CancellationToken ct = default);
    Task<List<NutritionPlan>> GetByTrainerIdAsync(Guid trainerId, CancellationToken ct = default);
    Task<NutritionPlan?> GetActivePlanAsync(Guid memberId, Guid trainerId, CancellationToken ct = default);
    Task AddAsync(NutritionPlan plan, CancellationToken ct = default);
    Task UpdateAsync(NutritionPlan plan, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}