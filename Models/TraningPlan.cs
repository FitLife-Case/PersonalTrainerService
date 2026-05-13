using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitLife.PersonalTrainer.API.Models;

public enum PlanStatusTraining
{
    Active,
    Paused,
    Completed
}

public class TrainingPlan
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("trainerId")]
    [BsonRepresentation(BsonType.String)]
    public Guid TrainerId { get; set; }

    [BsonElement("memberId")]
    [BsonRepresentation(BsonType.String)]
    public Guid MemberId { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("startDate")]
    [BsonRepresentation(BsonType.String)]
    public DateOnly StartDate { get; set; }

    [BsonElement("endDate")]
    [BsonRepresentation(BsonType.String)]
    public DateOnly EndDate { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public PlanStatusTraining Status { get; set; }

    [BsonElement("exercises")]
    public List<Exercise> Exercises { get; set; } = new();
}