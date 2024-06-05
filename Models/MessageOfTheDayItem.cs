using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MotdApiDotnet.Models;

public class MessageOfTheDayItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    [BsonElement("message")]
    public string Message { get; set; } = null!;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}