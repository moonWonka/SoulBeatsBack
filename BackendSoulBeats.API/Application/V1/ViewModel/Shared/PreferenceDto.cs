namespace BackendSoulBeats.API.Application.V1.ViewModel.Shared
{
    /// <summary>
    /// DTO para preferencias de géneros musicales.
    /// </summary>
    public class GenrePreferenceDto
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public int PreferenceLevel { get; set; } // 1-5
    }

    /// <summary>
    /// DTO para preferencias de artistas musicales.
    /// </summary>
    public class ArtistPreferenceDto
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public int PreferenceLevel { get; set; } // 1-5
    }
}
