using FitLife.PersonalTrainer.API.Models;
namespace FitLife.PersonalTrainer.API.Repositories;

public interface ITrainingPlanRepository
{
    Task<TrainingPlan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<TrainingPlan>> GetByMemberIdAsync(Guid memberId, CancellationToken ct = default);
    Task<List<TrainingPlan>> GetByTrainerIdAsync(Guid trainerId, CancellationToken ct = default);
    Task<TrainingPlan?> GetActivePlanAsync(Guid memberId, Guid trainerId, CancellationToken ct = default);
    Task AddAsync(TrainingPlan plan, CancellationToken ct = default);
    Task UpdateAsync(TrainingPlan plan, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}