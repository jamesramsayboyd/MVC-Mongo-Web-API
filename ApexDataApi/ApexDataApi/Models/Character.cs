using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApexDataApi.Models;

/// <summary>
/// A Class representing a character used in the game Apex Legends
/// </summary>
public class Character : IComparable
{
    #region CONSTRUCTORS
    /// <summary>
    /// A default, parameterless constructor for MVC purposes
    /// </summary>
    public Character() { }

    /// <summary>
    /// A constructor allowing only the character's name to be set
    /// ID is automatically set to first four characters of character name
    /// Image is set to default
    /// </summary>
    /// <param name="characterName"></param>
    public Character(string characterName)
    {
        CharacterName = characterName;
        Id = CharacterName.Substring(0, 4).ToLower();
        Image = "default.png";
    }
    #endregion CONSTRUCTORS

    /// <summary>
    /// CompareTo function required for the IComparable interface
    /// Character objects are sorted by the Playtime variable
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int CompareTo(object obj)
    {
        Character? compare = obj as Character;
        return Playtime.CompareTo(compare?.Playtime);
    }

    /// <summary>
    /// A unique ID for each character. IDs should be manually set to be
    /// the first four characters of the CharacterName
    /// </summary>
    [BsonElement("id")]
    public string Id { get; set; }

    /// <summary>
    /// The Character's name (i.e. 'Bangalore', 'Horizon')
    /// </summary>
    [BsonElement("name")]
    public string CharacterName { get; set; } = null!;

    /// <summary>
    /// A variable to contain the character's playtime. This isn't stored on 
    /// the character object itself, but is dynamically calculated when needed
    /// by looping through the list of all players and counting their playtime 
    /// stats per character
    /// </summary>
    [BsonElement("playtime")]
    public int Playtime { get; set; }

    /// <summary>
    /// An image representing the character
    /// </summary>
    [BsonElement("image")]
    public string Image { get; set; } = null!;
}
