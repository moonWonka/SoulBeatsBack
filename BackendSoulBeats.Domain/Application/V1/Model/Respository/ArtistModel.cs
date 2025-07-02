namespace BackendSoulBeats.Domain.Application.V1.Model.Respository
{
    /// <summary>
    /// Modelo que representa un artista musical en la base de datos.
    /// </summary>
    public class ArtistModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SpotifyId { get; set; }
        public string ImageUrl { get; set; }
        public int? GenreId { get; set; }
        public string GenreName { get; set; } // Para joins con tabla Genres
        public int Popularity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}