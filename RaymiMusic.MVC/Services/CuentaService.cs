using RaymiMusic.Modelos;
using RaymiMusic.Modelos.DTOs.Cuenta;

namespace RaymiMusic.MVC.Services
{
    public class CuentaService : ICuentaService
    {
        private readonly HttpClient _http;
        public CuentaService(IHttpClientFactory httpFactory)
            => _http = httpFactory.CreateClient("RaymiMusicApi");


        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
                var resp = await _http.PostAsJsonAsync("api/Cuenta/login", loginRequest);

                // Si la respuesta no es exitosa, verificamos el código de estado
                if (!resp.IsSuccessStatusCode)
                {
                    // Si el código de estado es 401, lanzamos un mensaje específico
                    if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new ApplicationException("Correo o contraseña incorrectos.");
                    }

                    // Si no es un error 401, lanza una excepción genérica
                    resp.EnsureSuccessStatusCode();
                }

                // Si la respuesta es exitosa, procesamos la respuesta
                return await resp.Content.ReadFromJsonAsync<LoginResponse>()
                       ?? throw new ApplicationException("Error al procesar la respuesta de login.");
           
        }
    }
}

