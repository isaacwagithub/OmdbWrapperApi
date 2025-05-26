using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using OmdbWrapperApi.DTOs;
using Microsoft.Extensions.Caching.Memory;


namespace OmdbWrapperApi.Services
{
    public class OmdbMovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private static readonly List<MovieDto> _customMovies = new();
        private readonly List<MovieDto> _cachedMovies = new();
        private readonly string _apiKey = "8aedc429"; 

        public OmdbMovieService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        private MovieDto MapOmdbResponseToDto(OmdbApiResponse response)
        {
            return new MovieDto
            {
                Title = response.Title,
                Year = response.Year,
                Rated = response.Rated,
                Released = response.Released,
                Runtime = response.Runtime,
                Genre = response.Genre,
                Director = response.Director,
                Plot = response.Plot,
                Poster = response.Poster,
                ImdbID = response.ImdbID,
                Type = response.Type
            };
        }


        public async Task<MovieDto?> GetByIdAsync(string id)
        {
            // Check cache
            if (_cache.TryGetValue(id, out MovieDto cachedMovie))
            {
                return cachedMovie;
            }

            // Call OMDb API
            var response = await _httpClient.GetFromJsonAsync<OmdbApiResponse>($"?i={id}&apikey={_apiKey}");
            if (response == null || response.Response == "False")
                return null;

            var movie = MapOmdbResponseToDto(response);

            // Set in cache with expiration (optional)
            _cache.Set(id, movie, TimeSpan.FromHours(1));

            return movie;
        }

        public async Task<MovieDto?> GetByTitleAsync(string title)
        {
            // 1. Check in cache for any movie where Title matches (case-insensitive)
            if (_cache.TryGetValue(title, out MovieDto cachedMovie))
            {
                return cachedMovie;
            }
            var url = $"http://www.omdbapi.com/?t={title}&apikey={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var movie = JsonSerializer.Deserialize<MovieDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return movie;
        }

        public async Task<List<MovieDto>?> GetByYearAsync(string year)
        {
            // 1. Check in-memory cache first
            if (_cachedMoviesByYear.TryGetValue(year, out var cachedMovies))
            {
                return cachedMovies;
            }
            var response = await _httpClient.GetFromJsonAsync<OmdbSearchResponse>($"?s={year}&apikey={_apiKey}");
            if (response == null || response.Response == "False" || response.Search == null)
                return null;

            // Filter results by year exactly
            var movies = response.Search
                .Where(m => m.Year == year)
                .Select(m => new MovieDto
                {
                    ImdbID = m.ImdbID,
                    Title = m.Title,
                    Year = m.Year,
                    Type = m.Type,
                    Poster = m.Poster
                })
                .ToList();

            // 3. Cache all results
            _cachedMovies.AddRange(movies);

            return movies;
        }

        public Task<bool> UpdateCachedEntryAsync(MovieDto updatedMovie)
        {
            var index = _cachedMovies.FindIndex(m => m.ImdbID == updatedMovie.ImdbID);
            if (index == -1)
            {
                return Task.FromResult(false);
            }

            _cachedMovies[index] = updatedMovie;
            return Task.FromResult(true);
        }
        
        public Task<bool> DeleteCachedEntryAsync(string imdbId)
        {
            var movie = _cachedMovies.FirstOrDefault(m => m.ImdbID == imdbId);
            if (movie == null)
            {
                return Task.FromResult(false);
            }

            _cachedMovies.Remove(movie);
            return Task.FromResult(true);
        }



        public Task<MovieDto> AddCustomMovieAsync(MovieDto movie)
        {
            // Generate a fake IMDb ID if not provided
            movie.ImdbID ??= $"custom-{Guid.NewGuid().ToString()[..8]}";

            _customMovies.Add(movie);
            return Task.FromResult(movie);
        }

        public Task<List<MovieDto>> GetAllCachedMoviesAsync()
        {
            return Task.FromResult(_customMovies.ToList());
        }

    }
}
