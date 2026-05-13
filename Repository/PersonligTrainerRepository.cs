using MongoDB.Driver;
using FitLife.PersonalTrainer.API.Models;

namespace FitLife.PersonalTrainer.API.Repositories;

public class TrainerRepository : ITrainerRepository
{
    private readonly IMongoCollection<Trainer> _trainers;

    public TrainerRepository(IMongoDatabase database)
    {
        _trainers = database.GetCollection<Trainer>("trainers");

        var indexKeys = Builders<Trainer>.IndexKeys.Ascending(t => t.UserAccountId);
        _trainers.Indexes.CreateOne(new CreateIndexModel<Trainer>(indexKeys));
    }

    public async Task<Trainer?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _trainers.Find(t => t.Id == id).FirstOrDefaultAsync(ct);

    public async Task<List<Trainer>> GetAllAsync(CancellationToken ct = default)
        => await _trainers.Find(_ => true).ToListAsync(ct);

    public async Task<List<Trainer>> GetByCenterIdAsync(Guid centerId, CancellationToken ct = default)
        => await _trainers.Find(t => t.WorksAtCenterIds.Contains(centerId)).ToListAsync(ct);

    public async Task AddAsync(Trainer trainer, CancellationToken ct = default)
        => await _trainers.InsertOneAsync(trainer, cancellationToken: ct);

    public async Task UpdateAsync(Trainer trainer, CancellationToken ct = default)
        => await _trainers.ReplaceOneAsync(t => t.Id == trainer.Id, trainer, cancellationToken: ct);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        => await _trainers.DeleteOneAsync(t => t.Id == id, ct);
}