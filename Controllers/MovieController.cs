using Microsoft.AspNetCore.Mvc;
using OmdbWrapperApi.Services;
using OmdbWrapperApi.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OmdbWrapperApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("id/{imdbId}")]
        public async Task<IActionResult> GetById(string imdbId)
        {
            var movie = await _movieService.GetByIdAsync(imdbId);
            if (movie == null)
                return NotFound();

            return Ok(movie);
        }

        [HttpGet("title/{title}")]
        public async Task<IActionResult> GetByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Title is required.");

            var movie = await _movieService.GetByTitleAsync(title);
            if (movie == null || string.IsNullOrEmpty(movie.ImdbID))
                return NotFound($"Movie with title '{title}' not found.");

            return Ok(movie);
        }

        [HttpGet("year/{year}")]
        public async Task<IActionResult> GetByYear(string year)
        {
            if (string.IsNullOrWhiteSpace(year))
                return BadRequest("Year is required.");

            var movies = await _movieService.GetByYearAsync(year);
            if (movies == null || movies.Count == 0)
                return NotFound($"No movies found for year '{year}'.");

            return Ok(movies);
        }

        [HttpPost("custom")]
        public async Task<IActionResult> CreateCustomMovie([FromBody] MovieDto movie)
        {
            if (movie == null || string.IsNullOrWhiteSpace(movie.Title))
                return BadRequest("Movie data must include at least a title.");

            var added = await _movieService.AddCustomMovieAsync(movie);
            return CreatedAtAction(nameof(GetById), new { imdbId = added.ImdbID }, added);
        }

        [HttpGet("cached")]
        public async Task<IActionResult> GetAllCachedMovies()
        {
            var movies = await _movieService.GetAllCachedMoviesAsync();
            return Ok(movies);
        }

        [HttpPut("cached/{id}")]
        public async Task<IActionResult> UpdateCachedEntry(string id, [FromBody] MovieDto updatedMovie)
        {
            if (id != updatedMovie.ImdbID)
            {
                return BadRequest("ID mismatch");
            }

            var success = await _movieService.UpdateCachedEntryAsync(updatedMovie);
            if (!success)
            {
                return NotFound();
            }

            return Ok(updatedMovie);
        }

        [HttpDelete("cached/{id}")]
        public async Task<IActionResult> DeleteCachedEntry(string id)
        {
            var success = await _movieService.DeleteCachedEntryAsync(id);
            if (!success)
            {
                return NotFound($"Movie with IMDb ID '{id}' not found in cache.");
            }

            return Ok($"Movie with IMDb ID '{id}' removed from cache.");
        }

    }
}
