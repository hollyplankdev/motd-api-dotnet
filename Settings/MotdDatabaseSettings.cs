namespace MotdApiDotnet.Settings;

public class MotdDatabaseSettings
{
    /// <summary>
    /// The string for the MongoDB driver to use to connect to the MongoDB instance.
    /// </summary>
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// The name of the MongoDB database to work with.
    /// </summary>
    public string DatabaseName { get; set; } = null!;

    /// <summary>
    /// The name of the collection containing `MessageOfTheDayItem`s in the MongoDB instance.
    /// </summary>
    public string MotdCollectionName { get; set; } = null!;
}