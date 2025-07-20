using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaymiMusic.AppWeb.Models;
using RaymiMusic.AppWeb.Services;

namespace RaymiMusic.AppWeb.Controllers
{
    [Authorize] // Solo usuarios autenticados pueden ver el Home
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISongService _songService;
        private readonly IArtistService _artistService;

        public HomeController(
            ILogger<HomeController> logger,
            ISongService songService,
            IArtistService artistService)
        {
            _logger = logger;
            _songService = songService;
            _artistService = artistService;
        }


        public async Task<IActionResult> Index(string q)
        {
            var canciones = string.IsNullOrWhiteSpace(q)
                ? await _songService.GetAllAsync()
                : await _songService.SearchAsync(q);

            var artistas = await _artistService.GetAllArtistasSearchAsync(q);

            var vm = new HomeIndexVM
            {
                Query = q,
                Songs = canciones,
                Artistas = artistas
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                            ?? HttpContext.TraceIdentifier
            });
        }
    }
}
