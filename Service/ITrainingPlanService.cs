using FitLife.PersonalTrainer.API.Models;

namespace FitLife.PersonalTrainer.API.Services;

public interface ITrainingPlanService
{
    Task<TrainingPlan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<TrainingPlan>> GetByMemberIdAsync(Guid memberId, CancellationToken ct = default);
    Task<List<TrainingPlan>> GetByTrainerIdAsync(Guid trainerId, CancellationToken ct = default);
    Task<TrainingPlan> CreateAsync(TrainingPlan plan, CancellationToken ct = default);
    Task UpdateAsync(Guid id, TrainingPlan plan, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task AddExerciseAsync(Guid planId, Exercise exercise, CancellationToken ct = default);
    Task RemoveExerciseAsync(Guid planId, Guid exerciseId, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid id, PlanStatusTraining status, CancellationToken ct = default);
}