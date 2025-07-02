using BackendSoulBeats.Domain.Application.V1.Repository;
using Microsoft.ApplicationInsights;
using MediatR;
using System.Net;

namespace BackendSoulBeats.API.Application.V1.Query.GetGenres
{
    public class GetGenresHandler : IRequestHandler<GetGenresRequest, GetGenresResponse>
    {
        private readonly ISoulBeatsRepository _soulBeatsRepository;
        private readonly TelemetryClient _telemetryClient;

        public GetGenresHandler(ISoulBeatsRepository soulBeatsRepository, TelemetryClient telemetryClient)
        {
            _soulBeatsRepository = soulBeatsRepository;
            _telemetryClient = telemetryClient;
        }

        public async Task<GetGenresResponse> Handle(GetGenresRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _telemetryClient.TrackEvent("GetGenresRequested", new Dictionary<string, string>
                {
                    {"Handler", "GetGenresHandler"},
                    {"RequestTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}
                });

                var startTime = DateTime.UtcNow;

                // Obtener géneros desde el repositorio
                var genres = await _soulBeatsRepository.GetActiveGenresAsync();

                var genreDtos = genres?.Select(g => new GenreDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    IconUrl = g.IconUrl,
                    DisplayOrder = g.DisplayOrder
                }).OrderBy(g => g.DisplayOrder).ToList() ?? new List<GenreDto>();

                var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

                _telemetryClient.TrackEvent("GetGenresSuccess", new Dictionary<string, string>
                {
                    {"Handler", "GetGenresHandler"},
                    {"GenresCount", genreDtos.Count.ToString()},
                    {"ResponseTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}
                });

                _telemetryClient.TrackMetric("GetGenresDuration", duration);

                return new GetGenresResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Description = "Géneros obtenidos exitosamente",
                    UserFriendly = $"{genreDtos.Count} géneros musicales disponibles",
                    Genres = genreDtos
                };
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex, new Dictionary<string, string>
                {
                    {"Handler", "GetGenresHandler"},
                    {"ErrorMessage", ex.Message}
                });

                return new GetGenresResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Description = $"Error interno: {ex.Message}",
                    UserFriendly = "Error al obtener los géneros musicales"
                };
            }
        }
    }
}