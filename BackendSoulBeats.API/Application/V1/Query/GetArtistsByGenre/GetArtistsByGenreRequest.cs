using MediatR;

namespace BackendSoulBeats.API.Application.V1.Query.GetArtistsByGenre
{
    public class GetArtistsByGenreRequest : IRequest<GetArtistsByGenreResponse>
    {
        public int GenreId { get; set; }
    }
}