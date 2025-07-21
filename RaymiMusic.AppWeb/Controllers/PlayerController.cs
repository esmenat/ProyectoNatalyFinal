using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaymiMusic.AppWeb.Services;
using RaymiMusic.AppWeb.Models;
using System.Security.Claims;

namespace RaymiMusic.AppWeb.Controllers
{
    [Authorize]
    public class PlayerController : Controller
    {
        private readonly ISongService _songService;
        private readonly IPlanesService _planesService;
        private readonly IWebHostEnvironment _env;
        public PlayerController(ISongService songService, IPlanesService planesService, IWebHostEnvironment env)
        {
            _songService = songService;
            _planesService = planesService;
            _env = env;
        }

        // GET: /Player/Play/{id}
        public async Task<IActionResult> Play(Guid id)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return NotFound();
            }

            var plan = await _planesService.ObtenerPlanUsuarioAsync(Guid.Parse(userId));
           
                if (plan.Nombre == "Free")
                {
                    // Ruta física a la carpeta 'anuncios'
                    var anunciosPath = Path.Combine(_env.WebRootPath, "anuncios");

                    if (Directory.Exists(anunciosPath))
                    {
                        var archivos = Directory.GetFiles(anunciosPath);
                        if (archivos.Length > 0)
                        {
                            var random = new Random();
                            var anuncioElegido = Path.GetFileName(archivos[random.Next(archivos.Length)]);

                            // Pasamos el anuncio a la vista mediante ViewBag
                            ViewBag.Anuncio = "/anuncios/" + anuncioElegido;
                        }
                    }
                }


            


            var song = await _songService.GetByIdAsync(id);
            if (song == null) return NotFound();

            return View(song);
        }
    }
}
