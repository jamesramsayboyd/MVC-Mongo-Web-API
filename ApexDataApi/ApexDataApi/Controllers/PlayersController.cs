using ApexDataApi.Models;
using ApexDataApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApexDataApi.Controllers
{
    /// <summary>
    /// A controller for player actions, used for the Front End
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    //[Route("frontend/[controller]")]
    [Route("[controller]")]
    public class PlayersController : Controller
    {
        private readonly PlayersService _playersService;
        private readonly CharactersService _charactersService;

        /// <summary>
        /// Connection to the Service containing all methods
        /// </summary>
        /// <param name="playersService"></param>
        /// <param name="charactersService"></param>
        public PlayersController(PlayersService playersService, CharactersService charactersService)
        {
            _playersService = playersService;
            _charactersService = charactersService;
        }

        #region CREATE
        ///// <summary>
        ///// Returns the Create Player page
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("Create"), Route("create")]
        //public async ActionResult Create()
        //{
        //    return View();
        //}

        [HttpGet("Create"), Route("create")]
        public async Task<ActionResult> Create()
        {
            List<Character> result = await _charactersService.GetCharacterList();
            ViewBag.CharacterNames = result;
            //ViewBag.Message = await _charactersService.GetCharacterList();
            return View();
        }

        /// <summary>
        /// Adds the player to the database, redirects the user to index
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        [HttpPost("Create"), Route("details")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Player player)
        {
            await _playersService.CreateAsync(player);
            return RedirectToAction("IndexAdmin");
        }
        #endregion CREATE

        #region READ
        /// <summary>
        /// Shows list of all players with CRUD options for admin
        /// </summary>
        /// <returns></returns>
        [HttpGet("Admin"), Route("index")]
        public async Task<IActionResult> IndexAdmin()
        {
            return View(await _playersService.GetAsync());
        }

        /// <summary>
        /// Show list of all players ordered by Rank ascending, used 
        /// as the player leaderboard page
        /// </summary>
        /// <returns></returns>
        [HttpGet("Index"), Route("index")]
        public async Task<IActionResult> Index()
        {
            return View(await _playersService.GetRankedListAsync());
        }

        /// <summary>
        /// Shows a single player's details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("PlayerDetails"), Route("details")]
        public async Task<IActionResult> Details(string id)
        {
            var player = await _playersService.GetAsyncId(id);

            if (player is null)
            {
                return NotFound();
            }

            return View(player);
        }

        /// <summary>
        /// Shows a single player's details with CRUD options
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("PlayerDetailsAdmin"), Route("detailsAdmin")]
        public async Task<IActionResult> DetailsAdmin(string id)
        {
            var player = await _playersService.GetAsyncId(id);

            if (player is null)
            {
                return NotFound();
            }

            return View(player);
        }
        #endregion READ

        #region UPDATE
        /// <summary>
        /// Returns the Edit Player page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Edit"), Route("edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var player = await _playersService.GetAsyncId(id);
            if (player is null)
                return NotFound();

            List<Character> result = await _charactersService.GetCharacterList();
            ViewBag.CharacterNames = result;

            return View(player);
        }

        /// <summary>
        /// Updates the player's details and redirects to the index
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        [HttpPost("Edit"), Route("details")]
        public async Task<ActionResult> Edit(Player player)
        {
            await _playersService.UpdatePlayerAsync(player);
            return RedirectToAction("indexAdmin");
        }
        #endregion UPDATE

        #region DELETE
        /// <summary>
        /// Loads the Delete Player page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var player = await _playersService.GetAsyncId(id);

            if (player is null)
            {
                return NotFound();
            }

            return View(player);
        }

        /// <summary>
        /// Deletes the player from the database, redirects to the index
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        public async Task<ActionResult> Delete(Player player)
        {
            await _playersService.RemoveAsync(player);
            return RedirectToAction("IndexAdmin");
        }
        #endregion DELETE
    }
}
