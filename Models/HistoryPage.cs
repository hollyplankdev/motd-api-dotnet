using MongoDB.Bson.Serialization.Attributes;

namespace MotdApiDotnet.Models;

public class HistoryPage<TValue>
{
    [BsonElement("lastId")]
    public string? LastId { get; set; }

    [BsonElement("items")]
    public List<TValue> Items { get; set; } = [];
}