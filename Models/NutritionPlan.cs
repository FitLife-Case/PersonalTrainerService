using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitLife.PersonalTrainer.API.Models;

public enum PlanStatusNutrition
{
    Active,
    Paused,
    Completed
}

public class NutritionPlan
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

    [BsonElement("dailyCalorieTarget")]
    public int DailyCalorieTarget { get; set; }

    [BsonElement("startDate")]
    [BsonRepresentation(BsonType.String)]
    public DateOnly StartDate { get; set; }

    [BsonElement("endDate")]
    [BsonRepresentation(BsonType.String)]
    public DateOnly EndDate { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public PlanStatusNutrition Status { get; set; }

    [BsonElement("meals")]
    public List<Meal> Meals { get; set; } = new();
}