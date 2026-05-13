using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitLife.PersonalTrainer.API.Models;

public enum MealType
{
    Breakfast,
    Lunch,
    Dinner,
    Snack
}

public class Meal
{
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("mealType")]
    [BsonRepresentation(BsonType.String)]
    public MealType MealType { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("calories")]
    public int Calories { get; set; }

    [BsonElement("proteinGrams")]
    public decimal ProteinGrams { get; set; }

    [BsonElement("carbsGrams")]
    public decimal CarbsGrams { get; set; }

    [BsonElement("fatGrams")]
    public decimal FatGrams { get; set; }
}