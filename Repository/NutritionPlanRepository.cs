using MongoDB.Driver;
using FitLife.PersonalTrainer.API.Models;

namespace FitLife.PersonalTrainer.API.Repositories;

public class NutritionPlanRepository : INutritionPlanRepository
{
    private readonly IMongoCollection<NutritionPlan> _plans;

    public NutritionPlanRepository(IMongoDatabase database)
    {
        _plans = database.GetCollection<NutritionPlan>("nutritionPlans");

        var memberIndex = Builders<NutritionPlan>.IndexKeys.Ascending(p => p.MemberId);
        var trainerIndex = Builders<NutritionPlan>.IndexKeys.Ascending(p => p.TrainerId);
        _plans.Indexes.CreateOne(new CreateIndexModel<NutritionPlan>(memberIndex));
        _plans.Indexes.CreateOne(new CreateIndexModel<NutritionPlan>(trainerIndex));
    }

    public async Task<NutritionPlan?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _plans.Find(p => p.Id == id).FirstOrDefaultAsync(ct);

    public async Task<List<NutritionPlan>> GetByMemberIdAsync(Guid memberId, CancellationToken ct = default)
        => await _plans.Find(p => p.MemberId == memberId).ToListAsync(ct);

    public async Task<List<NutritionPlan>> GetByTrainerIdAsync(Guid trainerId, CancellationToken ct = default)
        => await _plans.Find(p => p.TrainerId == trainerId).ToListAsync(ct);

    public async Task<NutritionPlan?> GetActivePlanAsync(Guid memberId, Guid trainerId, CancellationToken ct = default)
        => await _plans
            .Find(p => p.MemberId == memberId
                && p.TrainerId == trainerId
                && p.Status == PlanStatusNutrition.Active)
            .FirstOrDefaultAsync(ct);

    public async Task AddAsync(NutritionPlan plan, CancellationToken ct = default)
        => await _plans.InsertOneAsync(plan, cancellationToken: ct);

    public async Task UpdateAsync(NutritionPlan plan, CancellationToken ct = default)
        => await _plans.ReplaceOneAsync(p => p.Id == plan.Id, plan, cancellationToken: ct);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        => await _plans.DeleteOneAsync(p => p.Id == id, ct);
}