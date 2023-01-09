using Microsoft.AspNetCore.Mvc;
using ApexDataApi.Services;
using ApexDataApi.Models;

namespace ApexDataApi.Controllers
{
    /// <summary>
    /// A controller for character actions, used for the Front End
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    //[Route("frontend/[controller]")]
    [Route("[controller]")]
    public class CharactersController : Controller
    {        
        private readonly CharactersService _charactersService;

        /// <summary>
        /// Connection to the Service containing all methods
        /// </summary>
        /// <param name="charactersService"></param>
        public CharactersController(CharactersService charactersService)
        {
            _charactersService = charactersService;
        }

        #region CREATE
        /// <summary>
        /// Returns the Create Character page
        /// </summary>
        /// <returns></returns>
        [HttpGet("Create")]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Adds the character to the database, redirects the user to index
        /// </summary>
        /// <param name="character">The character being created</param>
        /// <returns></returns>
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Character character)
        {
            // automatically sets ID to first four characters of CharacterName
            character.Id = character.CharacterName.Substring(0, 4).ToLower();
            // Character image set to default
            character.Image = "default.png";
            await _charactersService.CreateAsync(character);
            return RedirectToAction("IndexAdmin");
        }
        #endregion CREATE

        #region READ
        /// <summary>
        /// Shows list of all characters with CRUD options for admin
        /// </summary>
        /// <returns></returns>
        [HttpGet("Admin"), Route("index")]
        public async Task<IActionResult> IndexAdmin()
        {
            return View(await _charactersService.GetCharacterList());
        }

        /// <summary>
        /// Shows list of all characters ordered by Playtime descending, used 
        /// as the character leaderboard page
        /// </summary>
        /// <returns></returns>
        [HttpGet("Index"), Route("index")]
        public async Task<IActionResult> Index()
        {
            return View(await _charactersService.GetCharacterListRanked());
        }

        /// <summary>
        /// Shows a single character's details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("CharacterDetails"), Route("details")]
        public async Task<IActionResult> Details(string id)
        {
            var character = await _charactersService.GetAsyncId(id);

            if (character is null)
            {
                return NotFound();
            }

            return View(character);
        }
        #endregion READ

        #region UPDATE
        /// <summary>
        /// Returns the Edit Character page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Edit"), Route("edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var character = await _charactersService.GetAsyncId(id);
            if (character is null)
                return NotFound();

            return View(character);
        }

        /// <summary>
        /// Updates the character's details and redirects to the index
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        [HttpPost("Edit"), Route("details")]
        public async Task<ActionResult> Edit(Character character)
        {
            await _charactersService.UpdateCharacterAsync(character);
            return RedirectToAction("indexAdmin");
        }
        #endregion UPDATE

        #region DELETE
        /// <summary>
        /// Loads the Delete Character page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var character = await _charactersService.GetAsyncId(id);

            if (character is null)
            {
                return NotFound();
            }

            return View(character);
        }

        /// <summary>
        /// Deletes the character from the database, redirects to the index
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        public async Task<ActionResult> Delete(Character character)
        {
            await _charactersService.RemoveAsync(character);
            return RedirectToAction("IndexAdmin");
        }
        #endregion DELETE        
    }
}
