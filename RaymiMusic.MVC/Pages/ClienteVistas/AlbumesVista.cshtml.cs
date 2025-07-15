using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using RaymiMusic.Modelos;

namespace RaymiMusic.MVC.Pages.ClienteVistas
{
    public class AlbumesVistaModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AlbumesVistaModel(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public List<Album> Albumes { get; set; } = new();
        public List<string> AlbumImagenes { get; set; } = new(); 

        public async Task OnGetAsync()
        {
            var apiUrl = _config["ApiBaseUrl"] + "/albumes";  

            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Albumes = JsonSerializer.Deserialize<List<Album>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

               
                foreach (var album in Albumes)
                {
                    string imagenUrl = "https://example.com/imagenes/" + album.Id + ".jpg";
                    AlbumImagenes.Add(imagenUrl);
                }
            }
        }
    }
}
