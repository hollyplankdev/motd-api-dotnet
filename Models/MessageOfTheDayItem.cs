using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MotdApiDotnet.Models;

/// <summary>
/// An object representing some message of the day, or MOTD.
/// </summary>
public class MessageOfTheDayItem
{
    /// <summary>
    /// The MongoDB ID that identifies this MOTD.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    /// <summary>
    /// The actual message text contained in this MOTD.
    /// </summary>
    [BsonElement("message")]
    public string Message { get; set; } = null!;

    /// <summary>
    ///  When this MOTD was initially created.
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When this MOTD was last updated.
    /// </summary>
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}