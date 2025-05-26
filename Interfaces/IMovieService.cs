using System.Collections.Generic;
using System.Threading.Tasks;
using OmdbWrapperApi.DTOs;

namespace OmdbWrapperApi.Services
{
    public interface IMovieService
    {
        Task<MovieDto?> GetByIdAsync(string imdbId);
        Task<MovieDto?> GetByTitleAsync(string title);
        Task<List<MovieDto>?> GetByYearAsync(string year);
        Task<MovieDto> AddCustomMovieAsync(MovieDto movie);
        Task<List<MovieDto>> GetAllCachedMoviesAsync();
        Task<bool> UpdateCachedEntryAsync(MovieDto updatedMovie);
    }
}
