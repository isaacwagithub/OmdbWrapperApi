using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmdbWrapperApi.Models
{
    [Table("CachedMovies")]
    public class CachedMovie
    {
        [Key]
        [Required]
        [MaxLength(50)]
        public string ImdbID { get; set; } = null!;

        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Year { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Rated { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Released { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Runtime { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Genre { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Director { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Plot { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Poster { get; set; } = string.Empty;
    }
}
