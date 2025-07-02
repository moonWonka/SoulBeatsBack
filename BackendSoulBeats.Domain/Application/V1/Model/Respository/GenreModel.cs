namespace BackendSoulBeats.Domain.Application.V1.Model.Respository
{
    /// <summary>
    /// Modelo que representa un género musical en la base de datos.
    /// </summary>
    public class GenreModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}