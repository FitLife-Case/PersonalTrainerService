using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitLife.PersonalTrainer.API.Models;

public class Trainer
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("userAccountId")]
    [BsonRepresentation(BsonType.String)]
    public Guid UserAccountId { get; set; }

    [BsonElement("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("lastName")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [BsonElement("yearsOfExperience")]
    public int YearsOfExperience { get; set; }

    [BsonElement("bio")]
    public string Bio { get; set; } = string.Empty;

    [BsonElement("specialties")]
    public List<string> Specialties { get; set; } = new();

    [BsonElement("worksAtCenterIds")]
    [BsonRepresentation(BsonType.String)]
    public List<Guid> WorksAtCenterIds { get; set; } = new();

    [BsonElement("isActive")]
    public bool IsActive { get; set; }

    [BsonElement("availabilitySlots")]
    public List<AvailabilitySlot> AvailabilitySlots { get; set; } = new();
}