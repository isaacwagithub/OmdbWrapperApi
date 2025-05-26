using System.Collections.Generic;

namespace OmdbWrapperApi.DTOs
{
    public class OmdbApiResponse
    {
        public string Title { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string ImdbID { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
        public string Rated { get; set; } = string.Empty;
        public string Released { get; set; } = string.Empty;
        public string Runtime { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Plot { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
    }

    public class OmdbSearchResponse
    {
        public List<OmdbSearchResult> Search { get; set; }
        public string Response { get; set; }
    }

    public class OmdbSearchResult
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string ImdbID { get; set; }
        public string Poster { get; set; }
        public string Type { get; set; }
    }
}
