using FitLife.PersonalTrainer.API.Models;

namespace FitLife.PersonalTrainer.API.Services;

public interface ITrainerService
{
    Task<Trainer?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Trainer>> GetAllAsync(CancellationToken ct = default);
    Task<List<Trainer>> GetByCenterIdAsync(Guid centerId, CancellationToken ct = default);
    Task<Trainer> CreateAsync(Trainer trainer, CancellationToken ct = default);
    Task UpdateAsync(Guid id, Trainer trainer, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task AddAvailabilitySlotAsync(Guid trainerId, AvailabilitySlot slot, CancellationToken ct = default);
    Task RemoveAvailabilitySlotAsync(Guid trainerId, Guid slotId, CancellationToken ct = default);
}