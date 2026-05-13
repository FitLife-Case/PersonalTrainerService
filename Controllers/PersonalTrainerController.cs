using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitLife.PersonalTrainer.API.Models;
using FitLife.PersonalTrainer.API.Services;

namespace FitLife.PersonalTrainer.API.Controllers;

[ApiController]

public class PersonalTrainerController : ControllerBase
{
    private readonly ITrainerService _trainerService;
    private readonly ITrainingPlanService _trainingPlanService;
    private readonly INutritionPlanService _nutritionPlanService;
    private readonly ILogger<PersonalTrainerController> _logger;

    public PersonalTrainerController(
        ITrainerService trainerService,
        ITrainingPlanService trainingPlanService,
        INutritionPlanService nutritionPlanService,
        ILogger<PersonalTrainerController> logger)
    {
        _trainerService = trainerService;
        _trainingPlanService = trainingPlanService;
        _nutritionPlanService = nutritionPlanService;
        _logger = logger;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // TRAINER ENDPOINTS
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Hent alle trænere</summary>
    [HttpGet("api/trainers")]
    [ProducesResponseType(typeof(List<Trainer>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTrainers(CancellationToken ct)
    {
        var trainers = await _trainerService.GetAllAsync(ct);
        return Ok(trainers);
    }

    /// <summary>Hent træner på ID</summary>
    [HttpGet("api/trainers/{id}")]
    [ProducesResponseType(typeof(Trainer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrainerById(Guid id, CancellationToken ct)
    {
        var trainer = await _trainerService.GetByIdAsync(id, ct);
        if (trainer is null)
            return NotFound(new { message = $"Trainer {id} ikke fundet" });
        return Ok(trainer);
    }

    /// <summary>Hent alle1 trænere på et bestemt center</summary>
    [HttpGet("api/trainers/center/{centerId}")]
    [ProducesResponseType(typeof(List<Trainer>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrainersByCenter(Guid centerId, CancellationToken ct)
    {
        var trainers = await _trainerService.GetByCenterIdAsync(centerId, ct);
        return Ok(trainers);
    }

    /// <summary>Opret ny træner</summary>
    [HttpPost("api/trainers")]
    [ProducesResponseType(typeof(Trainer), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTrainer([FromBody] Trainer trainer, CancellationToken ct)
    {
        var created = await _trainerService.CreateAsync(trainer, ct);
        return CreatedAtAction(nameof(GetTrainerById), new { id = created.Id }, created);
    }

    /// <summary>Opdater træner</summary>
    [HttpPut("api/trainers/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTrainer(Guid id, [FromBody] Trainer trainer, CancellationToken ct)
    {
        await _trainerService.UpdateAsync(id, trainer, ct);
        return Ok();
    }

    /// <summary>Slet træner</summary>
    [HttpDelete("api/trainers/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTrainer(Guid id, CancellationToken ct)
    {
        await _trainerService.DeleteAsync(id, ct);
        return NoContent();
    }

    /// <summary>Tilføj ledig tid til træner</summary>
    [HttpPost("api/trainers/{trainerId}/availability")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddAvailabilitySlot(
        Guid trainerId,
        [FromBody] AvailabilitySlot slot,
        CancellationToken ct)
    {
        await _trainerService.AddAvailabilitySlotAsync(trainerId, slot, ct);
        return Ok();
    }

    /// <summary>Fjern ledig tid fra træner</summary>
    [HttpDelete("api/trainers/{trainerId}/availability/{slotId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveAvailabilitySlot(
        Guid trainerId,
        Guid slotId,
        CancellationToken ct)
    {
        await _trainerService.RemoveAvailabilitySlotAsync(trainerId, slotId, ct);
        return NoContent();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // TRAINING PLAN ENDPOINTS
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Hent træningsplan på ID</summary>
    [HttpGet("api/trainingplans/{id}")]
    [ProducesResponseType(typeof(TrainingPlan), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrainingPlanById(Guid id, CancellationToken ct)
    {
        var plan = await _trainingPlanService.GetByIdAsync(id, ct);
        if (plan is null)
            return NotFound(new { message = $"TrainingPlan {id} ikke fundet" });
        return Ok(plan);
    }

    /// <summary>Hent alle træningsplaner for et medlem</summary>
    [HttpGet("api/trainingplans/member/{memberId}")]
    [ProducesResponseType(typeof(List<TrainingPlan>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrainingPlansByMember(Guid memberId, CancellationToken ct)
    {
        var plans = await _trainingPlanService.GetByMemberIdAsync(memberId, ct);
        return Ok(plans);
    }

    /// <summary>Hent alle træningsplaner for en træner</summary>
    [HttpGet("api/trainingplans/trainer/{trainerId}")]
    [ProducesResponseType(typeof(List<TrainingPlan>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrainingPlansByTrainer(Guid trainerId, CancellationToken ct)
    {
        var plans = await _trainingPlanService.GetByTrainerIdAsync(trainerId, ct);
        return Ok(plans);
    }

    /// <summary>Opret ny træningsplan</summary>
    [HttpPost("api/trainingplans")]
    [ProducesResponseType(typeof(TrainingPlan), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTrainingPlan([FromBody] TrainingPlan plan, CancellationToken ct)
    {
        var created = await _trainingPlanService.CreateAsync(plan, ct);
        return CreatedAtAction(nameof(GetTrainingPlanById), new { id = created.Id }, created);
    }

    /// <summary>Opdater træningsplan</summary>
    [HttpPut("api/trainingplans/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTrainingPlan(Guid id, [FromBody] TrainingPlan plan, CancellationToken ct)
    {
        await _trainingPlanService.UpdateAsync(id, plan, ct);
        return Ok();
    }

    /// <summary>Slet træningsplan</summary>
    [HttpDelete("api/trainingplans/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTrainingPlan(Guid id, CancellationToken ct)
    {
        await _trainingPlanService.DeleteAsync(id, ct);
        return NoContent();
    }

    /// <summary>Opdater status på træningsplan</summary>
    [HttpPatch("api/trainingplans/{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTrainingPlanStatus(
        Guid id,
        [FromBody] PlanStatusTraining status,
        CancellationToken ct)
    {
        await _trainingPlanService.UpdateStatusAsync(id, status, ct);
        return Ok();
    }

    /// <summary>Tilføj øvelse til træningsplan</summary>
    [HttpPost("api/trainingplans/{planId}/exercises")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddExercise(
        Guid planId,
        [FromBody] Exercise exercise,
        CancellationToken ct)
    {
        await _trainingPlanService.AddExerciseAsync(planId, exercise, ct);
        return Ok();
    }

    /// <summary>Fjern øvelse fra træningsplan</summary>
    [HttpDelete("api/trainingplans/{planId}/exercises/{exerciseId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveExercise(
        Guid planId,
        Guid exerciseId,
        CancellationToken ct)
    {
        await _trainingPlanService.RemoveExerciseAsync(planId, exerciseId, ct);
        return NoContent();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // NUTRITION PLAN ENDPOINTS
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Hent kostplan på ID</summary>
    [HttpGet("api/nutritionplans/{id}")]
    [ProducesResponseType(typeof(NutritionPlan), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNutritionPlanById(Guid id, CancellationToken ct)
    {
        var plan = await _nutritionPlanService.GetByIdAsync(id, ct);
        if (plan is null)
            return NotFound(new { message = $"NutritionPlan {id} ikke fundet" });
        return Ok(plan);
    }

    /// <summary>Hent alle kostplaner for et medlem</summary>
    [HttpGet("api/nutritionplans/member/{memberId}")]
    [ProducesResponseType(typeof(List<NutritionPlan>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNutritionPlansByMember(Guid memberId, CancellationToken ct)
    {
        var plans = await _nutritionPlanService.GetByMemberIdAsync(memberId, ct);
        return Ok(plans);
    }

    /// <summary>Hent alle kostplaner for en træner</summary>
    [HttpGet("api/nutritionplans/trainer/{trainerId}")]
    [ProducesResponseType(typeof(List<NutritionPlan>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNutritionPlansByTrainer(Guid trainerId, CancellationToken ct)
    {
        var plans = await _nutritionPlanService.GetByTrainerIdAsync(trainerId, ct);
        return Ok(plans);
    }

    /// <summary>Opret ny kostplan</summary>
    [HttpPost("api/nutritionplans")]
    [ProducesResponseType(typeof(NutritionPlan), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateNutritionPlan([FromBody] NutritionPlan plan, CancellationToken ct)
    {
        var created = await _nutritionPlanService.CreateAsync(plan, ct);
        return CreatedAtAction(nameof(GetNutritionPlanById), new { id = created.Id }, created);
    }

    /// <summary>Opdater kostplan</summary>
    [HttpPut("api/nutritionplans/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateNutritionPlan(Guid id, [FromBody] NutritionPlan plan, CancellationToken ct)
    {
        await _nutritionPlanService.UpdateAsync(id, plan, ct);
        return Ok();
    }

    /// <summary>Slet kostplan</summary>
    [HttpDelete("api/nutritionplans/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteNutritionPlan(Guid id, CancellationToken ct)
    {
        await _nutritionPlanService.DeleteAsync(id, ct);
        return NoContent();
    }

    /// <summary>Opdater status på kostplan</summary>
    [HttpPatch("api/nutritionplans/{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateNutritionPlanStatus(
        Guid id,
        [FromBody] PlanStatusNutrition status,
        CancellationToken ct)
    {
        await _nutritionPlanService.UpdateStatusAsync(id, status, ct);
        return Ok();
    }

    /// <summary>Tilføj måltid til kostplan</summary>
    [HttpPost("api/nutritionplans/{planId}/meals")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddMeal(
        Guid planId,
        [FromBody] Meal meal,
        CancellationToken ct)
    {
        await _nutritionPlanService.AddMealAsync(planId, meal, ct);
        return Ok();
    }

    /// <summary>Fjern måltid fra kostplan</summary>
    [HttpDelete("api/nutritionplans/{planId}/meals/{mealId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMeal(
        Guid planId,
        Guid mealId,
        CancellationToken ct)
    {
        await _nutritionPlanService.RemoveMealAsync(planId, mealId, ct);
        return NoContent();
    }
}
