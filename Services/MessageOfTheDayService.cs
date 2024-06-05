using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MotdApiDotnet.Models;
using MotdApiDotnet.Settings;

namespace MotdApiDotnet.Services;

public class MessageOfTheDayService
{
    /// <summary>
    /// The collection of MOTD items in the database.
    /// </summary>
    private readonly IMongoCollection<MessageOfTheDayItem> motdCollection;

    //
    //  Constructor
    //

    public MessageOfTheDayService(IOptions<MotdDatabaseSettings> databaseSettings)
    {
        // Get the MongoDB collection containing all MOTDs
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        motdCollection = mongoDatabase.GetCollection<MessageOfTheDayItem>(databaseSettings.Value.MotdCollectionName);
    }

    //
    //  Public Methods
    //

    public async Task CreateAsync(MessageOfTheDayItem newMotd) => await motdCollection.InsertOneAsync(newMotd);

    public async Task<MessageOfTheDayItem?> GetLatestAsync() => await motdCollection.Find(_ => true).SortByDescending(motd => motd.CreatedAt).FirstOrDefaultAsync();

    public async Task<MessageOfTheDayItem?> GetAsync(string id) => await motdCollection.Find(motd => motd.Id == id).FirstOrDefaultAsync();

    public async Task<List<MessageOfTheDayItem>> ListPageAsync(int pageSize, string? previousLastId = null)
    {
        var query = motdCollection.Find(_ => true);

        // If we have an ID to start from, filter our results to only be results before the given ID
        if (previousLastId != null)
        {
            var filter = Builders<MessageOfTheDayItem>.Filter.Lt("_id", previousLastId);
            query = motdCollection.Find(filter);
        }

        return await query.Limit(pageSize).ToListAsync();
    }

    public async Task UpdateAsync(string id, MessageOfTheDayItem updatedMotd) => await motdCollection.ReplaceOneAsync(motd => motd.Id == id, updatedMotd);

    public async Task RemoveAsync(string id) => await motdCollection.DeleteOneAsync(motd => motd.Id == id);

}