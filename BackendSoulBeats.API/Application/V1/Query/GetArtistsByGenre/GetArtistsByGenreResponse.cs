using BackendSoulBeats.API.Application.V1.ViewModel.Common;

namespace BackendSoulBeats.API.Application.V1.Query.GetArtistsByGenre
{
    public class GetArtistsByGenreResponse : BaseResponse
    {
        public List<ArtistDto> Artists { get; set; } = new();
    }

    public class ArtistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public int Popularity { get; set; }
    }
}