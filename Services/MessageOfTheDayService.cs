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
        await CreateAsync("There's a great deal of history that you should know, but I'm afraid that... I must continue my writing.");
    }

    /// <summary>
    /// Create a new MOTD in the database.
    /// </summary>
    /// <param name="message">The message text of the new MOTD to create.</param>
    /// <param name="newMotd">The entire new MOTD object to put in the DB, or null if params are invalid.</param>
    public async Task<MessageOfTheDayItem?> CreateAsync(string message)
    {
        var motd = new MessageOfTheDayItem()
        {
            Message = message,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await motdCollection.InsertOneAsync(motd);
        return motd;
    }

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
    /// <param name="newMessage">The new message text for the MOTD.</param>
    /// <returns>The updated MOTD or null if the MOTD couldn't be found.</returns>
    public async Task<MessageOfTheDayItem?> UpdateAsync(string id, string newMessage)
    {
        var update = Builders<MessageOfTheDayItem>.Update
            .Set(motd => motd.Message, newMessage)          // Update the message field
            .Set(motd => motd.UpdatedAt, DateTime.UtcNow);  // Make sure to upkeep the updatedAt field

        var result = await motdCollection.UpdateOneAsync(motd => motd.Id == id, update);
        if (!result.IsAcknowledged || result.ModifiedCount <= 0) return null;

        return await GetAsync(id);
    }

    /// <summary>
    /// Removes an existing MOTD.
    /// </summary>
    /// <param name="id">The ID of the MOTD to delete.</param>
    /// <returns>true if removed, false otherwise.</returns>
    public async Task<bool> RemoveAsync(string id)
    {
        var result = await motdCollection.DeleteOneAsync(motd => motd.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

}