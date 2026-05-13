using MongoDB.Driver;
using FitLife.PersonalTrainer.API.Models;

namespace FitLife.PersonalTrainer.API.Repositories;

public class TrainingPlanRepository : ITrainingPlanRepository
{
    private readonly IMongoCollection<TrainingPlan> _plans;

    public TrainingPlanRepository(IMongoDatabase database)
    {
        _plans = database.GetCollection<TrainingPlan>("trainingPlans");

        var memberIndex = Builders<TrainingPlan>.IndexKeys.Ascending(p => p.MemberId);
        var trainerIndex = Builders<TrainingPlan>.IndexKeys.Ascending(p => p.TrainerId);
        _plans.Indexes.CreateOne(new CreateIndexModel<TrainingPlan>(memberIndex));
        _plans.Indexes.CreateOne(new CreateIndexModel<TrainingPlan>(trainerIndex));
    }

    public async Task<TrainingPlan?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _plans.Find(p => p.Id == id).FirstOrDefaultAsync(ct);

    public async Task<List<TrainingPlan>> GetByMemberIdAsync(Guid memberId, CancellationToken ct = default)
        => await _plans.Find(p => p.MemberId == memberId).ToListAsync(ct);

    public async Task<List<TrainingPlan>> GetByTrainerIdAsync(Guid trainerId, CancellationToken ct = default)
        => await _plans.Find(p => p.TrainerId == trainerId).ToListAsync(ct);

    public async Task<TrainingPlan?> GetActivePlanAsync(Guid memberId, Guid trainerId, CancellationToken ct = default)
        => await _plans
            .Find(p => p.MemberId == memberId
                && p.TrainerId == trainerId
                && p.Status == PlanStatusTraining.Active)
            .FirstOrDefaultAsync(ct);

    public async Task AddAsync(TrainingPlan plan, CancellationToken ct = default)
        => await _plans.InsertOneAsync(plan, cancellationToken: ct);

    public async Task UpdateAsync(TrainingPlan plan, CancellationToken ct = default)
        => await _plans.ReplaceOneAsync(p => p.Id == plan.Id, plan, cancellationToken: ct);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        => await _plans.DeleteOneAsync(p => p.Id == id, ct);
}