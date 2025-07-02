namespace BackendSoulBeats.Domain.Application.V1.Model.Respository
{
    /// <summary>
    /// Modelo que representa las preferencias de artista musical de un usuario.
    /// </summary>
    public class UserArtistPreferenceModel
    {
        public long Id { get; set; }
        public string FirebaseUid { get; set; }
        public int ArtistId { get; set; }
        public string ArtistName { get; set; } // Para joins con tabla Artists
        public int PreferenceLevel { get; set; } // 1-5
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}