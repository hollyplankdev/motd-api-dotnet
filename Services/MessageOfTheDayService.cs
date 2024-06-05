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
}