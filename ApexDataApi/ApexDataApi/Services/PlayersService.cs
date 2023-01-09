using ApexDataApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApexDataApi.Services;

/// <summary>
/// A Service containing CRUD methods and other utilities related to
/// Player objects, used for both the API and Front End
/// </summary>
public class PlayersService
{
    private readonly IMongoCollection<Player> _playersCollection;

    /// <summary>
    /// MongoDB connection
    /// </summary>
    /// <param name="apexPlayerDatabaseSettings"></param>
    public PlayersService(
        IOptions<ApexPlayerDatabaseSettings> apexPlayerDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            apexPlayerDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            apexPlayerDatabaseSettings.Value.DatabaseName);

        _playersCollection = mongoDatabase.GetCollection<Player>(
            apexPlayerDatabaseSettings.Value.PlayersCollectionName);
    }

    #region GET
    /// <summary>
    /// Returns a list of all players
    /// </summary>
    /// <returns></returns>
    public async Task<List<Player>> GetAsync() =>
        await _playersCollection.Find(_ => true).ToListAsync();

    /// <summary>
    /// Returns a list of all players, ordered by Rank ascending
    /// </summary>
    /// <returns></returns>
    public async Task<List<Player>> GetRankedListAsync()
    {
        List<Player> result = await _playersCollection.Find(_ => true).ToListAsync();
        result.Sort();
        return result;
    }

    /// <summary>
    /// Finds a single player by ID, returns the Player object
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Player?> GetAsyncId(string id) =>
        await _playersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    /// <summary>
    /// Finds a single player by PlayerName, returns the Player object
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<Player?> GetAsync(string name) =>
        await _playersCollection.Find(x => x.PlayerName == name).FirstOrDefaultAsync();

    /// <summary>
    /// Returns the Rank of a single player
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    public async Task<Player?> GetRank(int rank) =>
        await _playersCollection.Find(x => x.Rank == rank).FirstOrDefaultAsync();
    #endregion GET

    #region POST
    /// <summary>
    /// A method to create a player, used for the front end
    /// </summary>
    /// <param name="newPlayer"></param>
    /// <returns></returns>
    public async Task CreateAsync(Player newPlayer) =>
        await _playersCollection.InsertOneAsync(newPlayer);

    /// <summary>
    /// A simple method to create a player, used for the API
    /// Only name, rank and avatar are set, character details are left null for simplicity
    /// </summary>
    /// <param name="name"></param>
    /// <param name="rank"></param>
    /// <param name="avatar"></param>
    /// <returns></returns>
    public async Task CreateAsync(string name, int rank, string avatar)
    {
        Player newPlayer = new Player(name, rank, avatar);
        await _playersCollection.InsertOneAsync(newPlayer);
    }
    #endregion POST

    #region PUT
    /// <summary>
    /// A method to update a player's details, used for the front end
    /// </summary>
    /// <param name="updatedPlayer"></param>
    /// <returns></returns>
    public async Task UpdatePlayerAsync(Player updatedPlayer)
    {
        await _playersCollection.ReplaceOneAsync(x => x.Id == updatedPlayer.Id, updatedPlayer);
    }

    /// <summary>
    /// A method to update a single player's rank, used for the API
    /// The entire list of player's ranks are updated as necessary to compensate
    /// </summary>
    /// <param name="player"></param>
    /// <param name="newRank"></param>
    /// <returns></returns>
    public async Task UpdateRankAsync(Player player, int newRank)
    {
        int oldRank = player.Rank; // Saving player's current rank
        player.Rank = newRank; // Updating player's rank
        if (player.Rank == 1)
        {
            player.Topranked = true; // Setting Topranked field if player reaches rank 1
        }
        List<Player> players = await GetAsync();
        players.Sort();

        // If player's rank is improving, shuffle other players down
        if (newRank < oldRank)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Rank >= newRank)
                {
                    if (players[i].Rank < oldRank)
                    {
                        players[i].Rank += 1;
                        if (players[i].Rank == 1)
                        {
                            players[i].Topranked = true;
                        }
                        else
                        {
                            players[i].Topranked = false;
                        }
                        await _playersCollection.ReplaceOneAsync(x => x.Id == players[i].Id, players[i]);
                    }                    
                }
            }
            await _playersCollection.ReplaceOneAsync(x => x.Id == player.Id, player);
        }
        // If player's rank is worsening, shuffle other players up
        else if (newRank > oldRank)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Rank <= newRank)
                {
                    if (players[i].Rank > 1)
                    {
                        players[i].Rank -= 1;
                        if (players[i].Rank == 1)
                        {
                            players[i].Topranked = true;
                        }
                        else
                        {
                            players[i].Topranked = false;
                        }
                        await _playersCollection.ReplaceOneAsync(x => x.Id == players[i].Id, players[i]);
                    }                    
                }                    
            }
            await _playersCollection.ReplaceOneAsync(x => x.Id == player.Id, player);
        }
    }

    /// <summary>
    /// A method to update two player's ranks at once, used for the API
    /// </summary>
    /// <param name="player1"></param>
    /// <param name="newrank1"></param>
    /// <param name="player2"></param>
    /// <param name="newrank2"></param>
    /// <returns></returns>
    public async Task UpdateMultipleRanksAsync(Player player1, int newrank1, Player player2, int newrank2)
    {
        await UpdateRankAsync(player1, newrank1);
        await UpdateRankAsync(player2, newrank2);
    }
    #endregion PUT

    #region DELETE
    /// <summary>
    /// A method to remove a player by PlayerName variable, used for the API
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task RemoveAsync(string name) =>
        await _playersCollection.DeleteOneAsync(x => x.PlayerName.ToLower() == name.ToLower());

    /// <summary>
    /// A method to remove a player, used for the front end
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public async Task RemoveAsync(Player player) =>
        await _playersCollection.DeleteOneAsync(x => x.Id == player.Id);
    #endregion DELETE
}