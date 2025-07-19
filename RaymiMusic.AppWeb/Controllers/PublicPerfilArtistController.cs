using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;
using RaymiMusic.AppWeb.Services;
using RaymiMusic.Modelos;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RaymiMusic.AppWeb.Controllers
{
    public class PublicPerfilArtistController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly IFollowService _followService;

        public PublicPerfilArtistController(AppDbContext ctx, IFollowService follow)
        {
            _ctx = ctx;
            _followService = follow;
        }

        // Mostrar perfil de artista
        public async Task<IActionResult> Index(Guid id)
        {
            var artista = await _ctx.Artistas
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artista == null)
            {
                return NotFound();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return NotFound();
            }
            try
            {
                // Verificar si el usuario sigue al artista
                var follow = await _followService.GetFollowByUserAndArtistAsync(Guid.Parse(userId), id);
                ViewBag.IsFollowing = follow != null; // Si está siguiendo, ViewBag.IsFollowing será true
            }
            catch
            {
                ViewBag.isFollowing = false;
            }
            return View(artista);
        }

        // Cambiar el estado de seguimiento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFollow(Guid idArtista)
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

            var artist = await _ctx.Artistas.FirstOrDefaultAsync(a => a.Id == idArtista);
            if (artist == null)
            {
                return NotFound();
            }

            var follow = await _followService.GetFollowByUserAndArtistAsync(Guid.Parse(userId), idArtista);

            if (follow != null)
            {
                // Si ya sigue, dejar de seguir
                await _followService.DeleteFollowAsync(follow.Id);
            }
            else
            {
                // Si no sigue, seguir al artista
                var newFollow = new Follow
                {
                    UsuarioId = Guid.Parse(userId),
                    ArtistaId = idArtista,
                    FechaSeguimiento = DateTime.UtcNow
                };
                await _followService.CreateFollowAsync(newFollow);
            }

            return RedirectToAction("Index", "PublicPerfilArtista", new { id = idArtista });

        }
    }
}
