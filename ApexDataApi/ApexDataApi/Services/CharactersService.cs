using ApexDataApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApexDataApi.Services;

/// <summary>
/// A Service containing CRUD methods and other utilities related to
/// Character objects, used for both the API and Front End
/// </summary>
public class CharactersService
{
    private readonly IMongoCollection<Character> _charactersCollection;
    private readonly IMongoCollection<Player> _playersCollection;
    
    /// <summary>
    /// MongoDB connection
    /// </summary>
    /// <param name="apexPlayerDatabaseSettings"></param>
    public CharactersService(
        IOptions<ApexPlayerDatabaseSettings> apexPlayerDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            apexPlayerDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            apexPlayerDatabaseSettings.Value.DatabaseName);

        _charactersCollection = mongoDatabase.GetCollection<Character>(
            apexPlayerDatabaseSettings.Value.CharactersCollectionName);
        _playersCollection = mongoDatabase.GetCollection<Player>(
            apexPlayerDatabaseSettings.Value.PlayersCollectionName);
    }

    #region GET
    /// <summary>
    /// Finds a single character using the CharacterName variable
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<Character?> GetAsync(string name) =>
        await _charactersCollection.Find(x => x.CharacterName == name).FirstOrDefaultAsync();

    /// <summary>
    /// Finds a single character by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Character?> GetAsyncId(string id) =>
        await _charactersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    /// <summary>
    /// Loops through a list of all players searching for a specific character
    /// Sums the playtime of that character across all players, updates playtime variable
    /// </summary>
    /// <param name="character"></param>
    public async void CalculatePlaytimeAsync(Character character)
    {
        List<Player> players = await _playersCollection.Find(_ => true).ToListAsync();
        foreach (var player in players)
        {
            if (player.Character1 == character.Id)
            {
                character.Playtime += player.Character1Playtime;
            }
            else if (player.Character2 == character.Id)
            {
                character.Playtime += player.Character2Playtime;
            }
        }
    }

    // Creating a list of all character IDs
    //public async Task<Array<string>> GetCharacterIds()
    //{
    //    Array<string> names;
    //    List<Character> result = await _charactersCollection.Find(_ => true).ToListAsync();
    //    foreach (Character character in result)
    //    {

    //    }
    //}

    /// <summary>
    /// Returns a list of all characters, calculating playtime across all players. 
    /// List is unsorted
    /// </summary>
    /// <returns></returns>
    public async Task<List<Character>> GetCharacterList()
    {
        List<Character> result = await _charactersCollection.Find(_ => true).ToListAsync();
        foreach (Character character in result)
        {
            CalculatePlaytimeAsync(character);
        }
        return result;
    }

    /// <summary>
    /// Returns a list of all characters with playtime across all players. 
    /// List is sorted in order of playtime descending
    /// </summary>
    /// <returns></returns>
    public async Task<List<Character>> GetCharacterListRanked()
    {
        List<Character> result = await _charactersCollection.Find(_ => true).ToListAsync();
        foreach (Character character in result)
        {
            CalculatePlaytimeAsync(character);
        }
        result.Sort();
        result.Reverse();
        return result;
    }
    #endregion GET

    #region POST
    /// <summary>
    /// Creates a new character and adds it to the database. Used for the front end
    /// </summary>
    /// <param name="newCharacter"></param>
    /// <returns></returns>
    public async Task CreateAsync(Character newCharacter) =>
        await _charactersCollection.InsertOneAsync(newCharacter);

    /// <summary>
    /// Creates two new characters and adds them to the database. Used for the API
    /// </summary>
    /// <param name="name1"></param>
    /// <param name="name2"></param>
    /// <returns></returns>
    public async Task CreateListAsync(string name1, string name2)
    {
        Character character1 = new Character(name1);
        //character1.Id = name1.Substring(0, 4).ToLower();
        Character character2 = new Character(name2);
        //character2.Id = name2.Substring(0, 4).ToLower();
        List<Character> characterList = new List<Character>() { character1, character2 };
        await _charactersCollection.InsertManyAsync(characterList);
    }
    #endregion POST

    #region PUT
    /// <summary>
    /// Updates a single character. Used for the front end
    /// </summary>
    /// <param name="updatedCharacter"></param>
    /// <returns></returns>
    public async Task UpdateCharacterAsync(Character updatedCharacter)
    {
        await _charactersCollection.ReplaceOneAsync(x => x.Id == updatedCharacter.Id, updatedCharacter);
    }
    #endregion PUT

    #region DELETE
    /// <summary>
    /// Deletes a single character by name, used for the API
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task RemoveAsync(string name) =>
        await _charactersCollection.DeleteOneAsync(x => x.CharacterName.ToLower() == name.ToLower());

    /// <summary>
    /// Deletes a single character, used for the front end
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public async Task RemoveAsync(Character character) =>
        await _charactersCollection.DeleteOneAsync(x => x.Id == character.Id);
    #endregion DELETE
}
