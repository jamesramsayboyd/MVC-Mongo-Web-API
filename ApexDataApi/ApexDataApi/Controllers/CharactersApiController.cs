using Microsoft.AspNetCore.Mvc;
using ApexDataApi.Models;
using ApexDataApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace ApexDataApi.Controllers
{
    /// <summary>
    /// A controller for character actions, used for the API
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("Characters")]
    public class CharactersApiController : Controller
    {
        private readonly CharactersService _charactersService;

        /// <summary>
        /// Connection to the Service containing all methods
        /// </summary>
        /// <param name="charactersService"></param>
        public CharactersApiController(CharactersService charactersService)
        {
            _charactersService = charactersService;
        }

        #region SELECT ALL CHARACTERS
        /// <summary>
        /// Returns a list of all characters
        /// </summary>
        /// <returns></returns>
        [HttpGet("AllCharacters")]
        public async Task<List<Character>> GetCharacters() =>
            await _charactersService.GetCharacterList();
        #endregion SELECT ALL CHARACTERS

        #region SORT CHARACTERS
        /// <summary>
        /// Returns a list of all characters sorted by playtime descending
        /// </summary>
        /// <returns></returns>
        [HttpGet("CharactersByPlaytime")]
        public async Task<List<Character>> GetCharactersByPlaytime() =>
            await _charactersService.GetCharacterListRanked();
        #endregion SORT CHARACTERS

        #region INSERT CHARACTERS
        /// <summary>
        /// Inserts two Characters in one operation
        /// </summary>
        /// <param name="name1">The name of the first character to be inserted</param>
        /// <param name="name2">The name of the second character to be inserted</param>
        /// <returns></returns>
        [HttpPost("{name1}/{name2}")]
        public async Task<IActionResult> PostTwo(string name1, string name2)
        {
            await _charactersService.CreateListAsync(name1, name2);

            return CreatedAtAction(nameof(GetCharacters), 0, 0);
        }
        #endregion INSERT CHARACTERS

        #region DELETE CHARACTERS
        /// <summary>
        /// Deletes a specific Character by name
        /// </summary>
        /// <param name="name">Name of the character to delete (case-insensitive)</param>
        /// <returns></returns>
        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            var character = await _charactersService.GetAsync(name);

            if (character is null)
            {
                return NotFound();
            }

            await _charactersService.RemoveAsync(name);

            return NoContent();
        }

        /// <summary>
        /// Deletes multiple Characters in one operation
        /// </summary>
        /// <param name="name1">Name of the first character to delete</param>
        /// <param name="name2">Name of the second character to delete</param>
        /// <returns></returns>
        [HttpDelete("{name1}/{name2}")]
        public async Task<IActionResult> Delete(string name1, string name2)
        {
            var character = await _charactersService.GetAsync(name1);

            if (character is null)
                return NotFound();

            var character2 = await _charactersService.GetAsync(name2);

            if (character2 is null)
                return NotFound();

            await _charactersService.RemoveAsync(name1);
            await _charactersService.RemoveAsync(name2);

            return NoContent();
        }
        #endregion DELETE CHARACTERS
    }
}
