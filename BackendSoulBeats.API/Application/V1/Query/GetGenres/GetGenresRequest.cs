using MediatR;

namespace BackendSoulBeats.API.Application.V1.Query.GetGenres
{
    /// <summary>
    /// Solicitud para obtener todos los géneros musicales disponibles.
    /// </summary>
    public class GetGenresRequest : IRequest<GetGenresResponse>
    {
        // No requiere parámetros, devuelve todos los géneros activos
    }
}