using BackendSoulBeats.API.Application.V1.ViewModel.Common;

namespace BackendSoulBeats.API.Application.V1.Query.GetGenres
{
    /// <summary>
    /// Respuesta con la lista de géneros musicales disponibles.
    /// </summary>
    public class GetGenresResponse : BaseResponse
    {
        /// <summary>
        /// Lista de géneros musicales activos.
        /// </summary>
        public List<GenreDto> Genres { get; set; } = new();

        /// <summary>
        /// Cantidad total de géneros disponibles.
        /// </summary>
        public int TotalCount => Genres?.Count ?? 0;
    }

    /// <summary>
    /// DTO para representar un género musical.
    /// </summary>
    public class GenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public int DisplayOrder { get; set; }
    }
}