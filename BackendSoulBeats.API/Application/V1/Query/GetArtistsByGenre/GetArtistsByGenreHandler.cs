using System.Net;
using BackendSoulBeats.Domain.Application.V1.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendSoulBeats.API.Application.V1.Query.GetArtistsByGenre
{
    public class GetArtistsByGenreHandler : IRequestHandler<GetArtistsByGenreRequest, GetArtistsByGenreResponse>
    {
        private readonly ISoulBeatsRepository _repository;
        private readonly ILogger<GetArtistsByGenreHandler> _logger;

        public GetArtistsByGenreHandler(ISoulBeatsRepository repository, ILogger<GetArtistsByGenreHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<GetArtistsByGenreResponse> Handle(GetArtistsByGenreRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("🔍 Obteniendo artistas del género: {GenreId}", request.GenreId);

            try
            {
                if (request.GenreId <= 0)
                {
                    _logger.LogWarning("⚠️ GenreId inválido: {GenreId}", request.GenreId);
                    return new GetArtistsByGenreResponse
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        UserFriendly = "ID de género inválido"
                    };
                }

                var artists = await _repository.GetArtistsByGenreAsync(request.GenreId);

                var response = new GetArtistsByGenreResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    UserFriendly = "Artistas obtenidos exitosamente",
                    Artists = artists.Select(a => new ArtistDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        ImageUrl = a.ImageUrl ?? string.Empty,
                        GenreId = a.GenreId ?? 0,
                        GenreName = a.GenreName ?? string.Empty,
                        Popularity = a.Popularity
                    }).ToList()
                };

                _logger.LogDebug("✅ Artistas obtenidos para género {GenreId}: {Count}", request.GenreId, response.Artists.Count);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener artistas del género: {GenreId}", request.GenreId);

                return new GetArtistsByGenreResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    UserFriendly = "Error interno del servidor"
                };
            }
        }
    }
}