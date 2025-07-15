using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using RaymiMusic.Modelos;

namespace RaymiMusic.MVC.Pages.ClienteVistas
{
    public class PlayListsVistaModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public PlayListsVistaModel(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public List<ListaPublica> ListasPublicas { get; set; } = new();

        public async Task OnGetAsync()
        {
            var apiUrl = _config["ApiBaseUrl"] + "/listaspublicas"; // URL de la API para obtener listas públicas

            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                ListasPublicas = JsonSerializer.Deserialize<List<ListaPublica>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
    }
}
