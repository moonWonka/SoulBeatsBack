namespace BackendSoulBeats.Domain.Application.V1.Model.Respository
{
    /// <summary>
    /// Modelo que representa las preferencias de género musical de un usuario.
    /// </summary>
    public class UserGenrePreferenceModel
    {
        public long Id { get; set; }
        public string FirebaseUid { get; set; }
        public int GenreId { get; set; }
        public string GenreName { get; set; } // Para joins con tabla Genres
        public int PreferenceLevel { get; set; } // 1-5
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}