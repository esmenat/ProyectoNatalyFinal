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
            try
            {
                HttpResponseMessage response = await _http.GetAsync("https://localhost:1753/api/Generos");

                if (response.IsSuccessStatusCode)
                {
                    var generos = await response.Content.ReadFromJsonAsync<IEnumerable<Genero>>();
                    Console.WriteLine($"Generos obtenidos: {generos?.Count()}");
                    return generos ?? new List<Genero>();
                }
                else
                {
                    Console.WriteLine($"Error al obtener géneros: {response.StatusCode}");
                    return new List<Genero>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al obtener géneros: {ex.Message}");
                return new List<Genero>();
            }
        }

    }
}


