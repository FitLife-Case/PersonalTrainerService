using FitLife.PersonalTrainer.API.Models;
using FitLife.PersonalTrainer.API.Repositories;

namespace FitLife.PersonalTrainer.API.Services;

public class TrainingPlanService : ITrainingPlanService
{
    private readonly ITrainingPlanRepository _repository;
    private readonly ILogger<TrainingPlanService> _logger;

    public TrainingPlanService(
        ITrainingPlanRepository repository,
        ILogger<TrainingPlanService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TrainingPlan?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _repository.GetByIdAsync(id, ct);

    public async Task<List<TrainingPlan>> GetByMemberIdAsync(Guid memberId, CancellationToken ct = default)
        => await _repository.GetByMemberIdAsync(memberId, ct);

    public async Task<List<TrainingPlan>> GetByTrainerIdAsync(Guid trainerId, CancellationToken ct = default)
        => await _repository.GetByTrainerIdAsync(trainerId, ct);

    public async Task<TrainingPlan> CreateAsync(TrainingPlan plan, CancellationToken ct = default)
    {
        var existingPlan = await _repository.GetActivePlanAsync(plan.MemberId, plan.TrainerId, ct);
        if (existingPlan is not null)
            throw new InvalidOperationException("Der er allerede en aktiv træningsplan for dette medlem");

        plan.Id = Guid.NewGuid();
        plan.Status = PlanStatusTraining.Active;
        plan.Exercises = new List<Exercise>();

        await _repository.AddAsync(plan, ct);
        _logger.LogInformation("TrainingPlan {PlanId} oprettet for Member {MemberId}", plan.Id, plan.MemberId);
        return plan;
    }

    public async Task UpdateAsync(Guid id, TrainingPlan updatedPlan, CancellationToken ct = default)
    {
        var plan = await _repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"TrainingPlan {id} ikke fundet");

        plan.Name = updatedPlan.Name;
        plan.Description = updatedPlan.Description;
        plan.StartDate = updatedPlan.StartDate;
        plan.EndDate = updatedPlan.EndDate;

        await _repository.UpdateAsync(plan, ct);
        _logger.LogInformation("TrainingPlan {PlanId} opdateret", id);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var plan = await _repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"TrainingPlan {id} ikke fundet");

        await _repository.DeleteAsync(id, ct);
        _logger.LogInformation("TrainingPlan {PlanId} slettet", id);
    }

    public async Task AddExerciseAsync(Guid planId, Exercise exercise, CancellationToken ct = default)
    {
        var plan = await _repository.GetByIdAsync(planId, ct)
            ?? throw new KeyNotFoundException($"TrainingPlan {planId} ikke fundet");

        exercise.Id = Guid.NewGuid();
        exercise.Order = plan.Exercises.Count + 1;

        plan.Exercises.Add(exercise);
        await _repository.UpdateAsync(plan, ct);
        _logger.LogInformation("Exercise tilføjet til TrainingPlan {PlanId}", planId);
    }

    public async Task RemoveExerciseAsync(Guid planId, Guid exerciseId, CancellationToken ct = default)
    {
        var plan = await _repository.GetByIdAsync(planId, ct)
            ?? throw new KeyNotFoundException($"TrainingPlan {planId} ikke fundet");

        plan.Exercises.RemoveAll(e => e.Id == exerciseId);

        for (int i = 0; i < plan.Exercises.Count; i++)
            plan.Exercises[i].Order = i + 1;

        await _repository.UpdateAsync(plan, ct);
        _logger.LogInformation("Exercise {ExerciseId} fjernet fra TrainingPlan {PlanId}", exerciseId, planId);
    }

    public async Task UpdateStatusAsync(Guid id, PlanStatusTraining status, CancellationToken ct = default)
    {
        var plan = await _repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"TrainingPlan {id} ikke fundet");

        plan.Status = status;
        await _repository.UpdateAsync(plan, ct);
        _logger.LogInformation("TrainingPlan {PlanId} status opdateret til {Status}", id, status);
    }
}