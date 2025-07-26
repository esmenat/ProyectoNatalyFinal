using RaymiMusic.Modelos;

namespace RaymiMusic.AppWeb.Services
{
    public interface IGenerosService
    {
        Task <IEnumerable<Genero>> GetAllGenerosAsync();
        Task<Genero?> GetGeneroByIdAsync(Guid id);
    }
}
