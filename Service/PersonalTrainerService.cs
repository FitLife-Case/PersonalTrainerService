using FitLife.PersonalTrainer.API.Models;
using FitLife.PersonalTrainer.API.Repositories;

namespace FitLife.PersonalTrainer.API.Services;

public class TrainerService : ITrainerService
{
    private readonly ITrainerRepository _repository;
    private readonly ILogger<TrainerService> _logger;

    public TrainerService(
        ITrainerRepository repository,
        ILogger<TrainerService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Trainer?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var trainer = await _repository.GetByIdAsync(id, ct);
        if (trainer is null)
            _logger.LogWarning("Trainer {TrainerId} ikke fundet", id);
        return trainer;
    }

    public async Task<List<Trainer>> GetAllAsync(CancellationToken ct = default)
        => await _repository.GetAllAsync(ct);

    public async Task<List<Trainer>> GetByCenterIdAsync(Guid centerId, CancellationToken ct = default)
        => await _repository.GetByCenterIdAsync(centerId, ct);

    public async Task<Trainer> CreateAsync(Trainer trainer, CancellationToken ct = default)
    {
        trainer.Id = Guid.NewGuid();
        trainer.IsActive = true;
        trainer.AvailabilitySlots = new List<AvailabilitySlot>();

        await _repository.AddAsync(trainer, ct);
        _logger.LogInformation("Trainer {TrainerId} oprettet", trainer.Id);
        return trainer;
    }

    public async Task UpdateAsync(Guid id, Trainer updatedTrainer, CancellationToken ct = default)
    {
        var trainer = await _repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Trainer {id} ikke fundet");

        trainer.FirstName = updatedTrainer.FirstName;
        trainer.LastName = updatedTrainer.LastName;
        trainer.Email = updatedTrainer.Email;
        trainer.PhoneNumber = updatedTrainer.PhoneNumber;
        trainer.YearsOfExperience = updatedTrainer.YearsOfExperience;
        trainer.Bio = updatedTrainer.Bio;
        trainer.Specialties = updatedTrainer.Specialties;
        trainer.WorksAtCenterIds = updatedTrainer.WorksAtCenterIds;

        await _repository.UpdateAsync(trainer, ct);
        _logger.LogInformation("Trainer {TrainerId} opdateret", id);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var trainer = await _repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Trainer {id} ikke fundet");

        await _repository.DeleteAsync(id, ct);
        _logger.LogInformation("Trainer {TrainerId} slettet", id);
    }

    public async Task AddAvailabilitySlotAsync(
        Guid trainerId,
        AvailabilitySlot slot,
        CancellationToken ct = default)
    {
        var trainer = await _repository.GetByIdAsync(trainerId, ct)
            ?? throw new KeyNotFoundException($"Trainer {trainerId} ikke fundet");

        slot.Id = Guid.NewGuid();
        trainer.AvailabilitySlots.Add(slot);

        await _repository.UpdateAsync(trainer, ct);
        _logger.LogInformation("AvailabilitySlot tilføjet til Trainer {TrainerId}", trainerId);
    }

    public async Task RemoveAvailabilitySlotAsync(
        Guid trainerId,
        Guid slotId,
        CancellationToken ct = default)
    {
        var trainer = await _repository.GetByIdAsync(trainerId, ct)
            ?? throw new KeyNotFoundException($"Trainer {trainerId} ikke fundet");

        trainer.AvailabilitySlots.RemoveAll(s => s.Id == slotId);
        await _repository.UpdateAsync(trainer, ct);
        _logger.LogInformation("AvailabilitySlot {SlotId} fjernet fra Trainer {TrainerId}", slotId, trainerId);
    }
}