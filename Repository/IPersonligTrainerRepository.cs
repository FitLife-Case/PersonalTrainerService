using FitLife.PersonalTrainer.API.Models;
namespace FitLife.PersonalTrainer.API.Repositories;

public interface ITrainerRepository
{
    Task<Trainer?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Trainer>> GetAllAsync(CancellationToken ct = default);
    Task<List<Trainer>> GetByCenterIdAsync(Guid centerId, CancellationToken ct = default);
    Task AddAsync(Trainer trainer, CancellationToken ct = default);
    Task UpdateAsync(Trainer trainer, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}