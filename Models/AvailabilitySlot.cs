using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitLife.PersonalTrainer.API.Models;

public class AvailabilitySlot
{
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid CenterId { get; private set; }

    [BsonElement("dayOfWeek")]
    public DayOfWeek DayOfWeek { get; private set; }

    [BsonElement("startTime")]
    [BsonRepresentation(BsonType.String)]
    public TimeOnly StartTime { get; private set; }

    [BsonElement("endTime")]
    [BsonRepresentation(BsonType.String)]
    public TimeOnly EndTime { get; private set; }

    [BsonElement("isRecurring")]
    public bool IsRecurring { get; private set; }

    private AvailabilitySlot() { }

    public static AvailabilitySlot Create(
        Guid centerId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        bool isRecurring)
    {
        return new AvailabilitySlot
        {
            Id = Guid.NewGuid(),
            CenterId = centerId,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            IsRecurring = isRecurring
        };
    }
}