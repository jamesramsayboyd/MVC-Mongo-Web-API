using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApexDataApi.Models;
using ApexDataApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace ApexDataApi.Controllers
{
    /// <summary>
    /// A controller for player actions, used for the API
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("Players")]
    public class PlayersApiController : Controller
    {
        private readonly PlayersService _playersService;

        /// <summary>
        /// Connection to the Service containing all methods
        /// </summary>
        /// <param name="playersService"></param>
        public PlayersApiController(PlayersService playersService)
        {
            _playersService = playersService;
        }

        #region SELECT PLAYERS/CHARACTERS
        /// <summary>
        /// Selects all players
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("AllPlayers")]
        public async Task<List<Player>> Get() =>
            await _playersService.GetAsync();


        /// <summary>
        /// Selects a single player by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}")]
        public async Task<ActionResult<Player>> Get(string name)
        {
            var player = await _playersService.GetAsync(name);

            if (player is null)
            {
                return NotFound();
            }

            return player;
        }
        #endregion SORT PLAYERS/CHARACTERS

        #region SORT PLAYERS/CHARACTERS
        /// <summary>
        /// Sorts all players by Rank
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("PlayersByRank")]
        public async Task<List<Player>> GetRanked() =>
            await _playersService.GetRankedListAsync();

        #endregion SORT PLAYERS/CHARACTERS

        #region INSERT PLAYERS
        /// <summary>
        /// Inserts a Player
        /// </summary>
        /// <param name="name">The player's name</param>
        /// <param name="rank">The player's rank</param>
        /// <param name="avatar">The filepath of the player's avatar image</param>
        /// <returns></returns>
        [HttpPost("{name}/{rank}/{avatar}")]
        public async Task<IActionResult> Post(string name, int rank, string avatar)
        {
            await _playersService.CreateAsync(name, rank, avatar);

            return CreatedAtAction(nameof(Get), 0, 0);
        }

        #endregion INSERT PLAYERS

        #region UPDATE PLAYER RANKING
        /// <summary>
        /// Updates a Player's ranking
        /// </summary>
        /// <param name="name">The name of the player to be updated</param>
        /// <param name="rank">The new rank</param>
        /// <returns></returns>
        [HttpPut("{name}/{rank}")]
        public async Task<IActionResult> Update(string name, int rank)
        {
            var player = await _playersService.GetAsync(name);

            if (player is null)
                return NotFound();

            await _playersService.UpdateRankAsync(player, rank);

            return CreatedAtAction(nameof(Get), 0, 0);
        }

        /// <summary>
        /// Updates multiple Players' Rankings in one operation
        /// </summary>
        /// <param name="name1">Name of the first player to update</param>
        /// <param name="rank1">The new ranking</param>
        /// <param name="name2">Name of the second player to update</param>
        /// <param name="rank2">The new ranking</param>
        /// <returns></returns>
        [HttpPut("{name1}/{rank1}/{name2}/{rank2}")]
        public async Task<IActionResult> UpdateMultiple(string name1, int rank1, string name2, int rank2)
        {
            var player1 = await _playersService.GetAsync(name1);

            if (player1 is null)
                return NotFound();

            var player2 = await _playersService.GetAsync(name2);

            if (player2 is null)
                return NotFound();

            await _playersService.UpdateMultipleRanksAsync(player1, rank1, player2, rank2);

            return CreatedAtAction(nameof(Get), 0, 0);
        }
        #endregion UPDATE PLAYER RANKING
    }
}