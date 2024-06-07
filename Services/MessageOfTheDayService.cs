using Microsoft.Extensions.Options;
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

    public async Task PopulateDefaultsAsync()
    {
        // If there is at LEAST one MOTD, we don't need to populate. EXIT EARLY!
        var motd = await GetLatestAsync();
        if (motd != null) return;

        // OTHERWISE - populate the db with a default MOTD!
        await CreateAsync(new MessageOfTheDayItem() { Message = "There's a great deal of history that you should know, but I'm afraid that... I must continue my writing." });
    }

    /// <summary>
    /// Create a new MOTD in the database.
    /// </summary>
    /// <param name="newMotd">The entire new MOTD object to put in the DB.</param>
    public async Task CreateAsync(MessageOfTheDayItem newMotd) => await motdCollection.InsertOneAsync(newMotd);

    /// <summary>
    /// Get the most recently created MOTD.
    /// </summary>
    /// <returns>The latest MOTD if there is one, null otherwise.</returns>
    public async Task<MessageOfTheDayItem?> GetLatestAsync() => await motdCollection.Find(_ => true).SortByDescending(motd => motd.CreatedAt).FirstOrDefaultAsync();

    /// <summary>
    /// Get an existing MOTD.
    /// </summary>
    /// <param name="id">The ID of the MOTD to get.</param>
    /// <returns>The MOTD with the matching id, null if there isn't one.</returns>
    public async Task<MessageOfTheDayItem?> GetAsync(string id) => await motdCollection.Find(motd => motd.Id == id).FirstOrDefaultAsync();

    /// <summary>
    /// Get a list of existing MOTDs, sorted in descending order by creation date. Results are paginated.
    /// </summary>
    /// <param name="pageSize">The number of items to return per-page. While paginating, this value should stay the same.</param>
    /// <param name="previousLastId">OPTIONAL - The value of `lastId` from a previous call. Use this while paginating to progress through pages of results.</param>
    /// <returns>A single page of listed results.</returns>
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

    /// <summary>
    /// Update the message text of an existing MOTD.
    /// </summary>
    /// <param name="id">The ID of the MOTD to update.</param>
    /// <param name="updatedMotd">The entire updated MOTD object to replace in the DB.</param>
    public async Task UpdateAsync(string id, MessageOfTheDayItem updatedMotd) => await motdCollection.ReplaceOneAsync(motd => motd.Id == id, updatedMotd);

    /// <summary>
    /// Removes an existing MOTD.
    /// </summary>
    /// <param name="id">The ID of the MOTD to delete.</param>
    public async Task RemoveAsync(string id) => await motdCollection.DeleteOneAsync(motd => motd.Id == id);

}