using RaymiMusic.Modelos;

namespace RaymiMusic.AppWeb.Services
{
    public class GenerosService : IGenerosService
    {
        private readonly HttpClient _http;

        public GenerosService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<Genero>> GetAllGenerosAsync()
        {
            var generos = await _http.GetFromJsonAsync<Genero[]>($"api/Generos");
            if (generos == null)
            {
                return [];
            }
            return generos;
        }


        public async Task<Genero?> GetGeneroByIdAsync(Guid id)
        {
            var genero = await _http.GetFromJsonAsync<Genero>($"api/Generos/{id}");
            return genero;
        }
    }
}


