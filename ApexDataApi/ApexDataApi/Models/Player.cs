using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApexDataApi.Models;

/// <summary>
/// A Class representing an Apex Legends player
/// </summary>
public class Player : IComparable
{
    #region CONSTRUCTORS

    /// <summary>
    /// A default, parameterless constructor for MVC purposes
    /// </summary>
    public Player() { }

    /// <summary>
    /// An overloaded constructor allowing PlayerName and Rank fields to be set
    /// Used to insert a player with the API
    /// </summary>
    /// <param name="name"></param>
    /// <param name="rank"></param>
    /// <param name="avatar"></param>
    public Player(string name, int rank, string avatar)
    {
        PlayerName = name;
        Rank = rank;
        Avatar = avatar;
    }
    #endregion CONSTRUCTORS

    /// <summary>
    /// The CompareTo function for the IComparable interface
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int CompareTo(object? obj)
    {
        Player? compare = obj as Player;
        return Rank.CompareTo(compare?.Rank);
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string PlayerName { get; set; } = null!;

    [BsonElement("rank")]
    public int Rank { get; set; }

    [BsonElement("avatar")]
    public string Avatar { get; set; }

    [BsonElement("topranked")]
    public bool Topranked { get; set; }
    
    [BsonElement("character1")]
    public string Character1 { get; set; } = null!;

    [BsonElement("character1playtime")]
    public int Character1Playtime { get; set; }

    [BsonElement("character2")]
    public string Character2 { get; set; } = null!;

    [BsonElement("character2playtime")]
    public int Character2Playtime { get; set; }
}