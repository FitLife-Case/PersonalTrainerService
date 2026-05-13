using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitLife.PersonalTrainer.API.Models;

public class Exercise
{
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("exerciseName")]
    public string ExerciseName { get; set; } = string.Empty;

    [BsonElement("muscleGroup")]
    public string MuscleGroup { get; set; } = string.Empty;

    [BsonElement("targetSets")]
    public int TargetSets { get; set; }

    [BsonElement("targetReps")]
    public int TargetReps { get; set; }

    [BsonElement("targetWeightKg")]
    public decimal TargetWeightKg { get; set; }

    [BsonElement("order")]
    public int Order { get; set; }

    [BsonElement("notes")]
    public string Notes { get; set; } = string.Empty;
}