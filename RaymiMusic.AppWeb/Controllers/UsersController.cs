using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;
using RaymiMusic.Modelos;

namespace RaymiMusic.AppWeb.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _ctx;
        public UsersController(AppDbContext ctx) => _ctx = ctx;

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            var usuarios = await _ctx.Usuarios
                .Include(u => u.PlanSuscripcion)
                .AsNoTracking()
                .ToListAsync();
            return View(usuarios);
        }

        // GET: /Users/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _ctx.Usuarios
                .Include(u => u.PlanSuscripcion)
                .Include(u => u.Perfil)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();
            return View(user);
        }

        // GET: /Users/Create
        public IActionResult Create()
        {
            ViewData["Planes"] = new SelectList(_ctx.Planes, "Id", "Nombre");
            return View();
        }

        // POST: /Users/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Correo,HashContrasena,Rol,PlanSuscripcionId")] Usuario user)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Planes"] = new SelectList(_ctx.Planes, "Id", "Nombre", user.PlanSuscripcionId);
                return View(user);
            }

            user.Id = Guid.NewGuid();
            _ctx.Usuarios.Add(user);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Users/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _ctx.Usuarios.FindAsync(id);
            if (user == null) return NotFound();

            ViewData["Planes"] = new SelectList(_ctx.Planes, "Id", "Nombre", user.PlanSuscripcionId);
            return View(user);
        }
        public async Task<IActionResult> Perfil()
        {
            if (!User.Identity.IsAuthenticated) // Verificar si el usuario está autenticado
            {
                return RedirectToAction("Login", "Account"); // Redirigir al login si no está autenticado
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return NotFound(); // Si no se encuentra el userId, devolver un 404
            }

            // Buscar el usuario correspondiente al userId
            var usuario = await _ctx.Usuarios
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId)); // Asumiendo que 'userId' es el ID del usuario

            if (usuario == null)
            {
                return NotFound(); // Si no se encuentra el usuario, devolver un 404
            }

            return View("PerfilUsuario", usuario); // Pasar el usuario a la vista correcta
        }

        // POST: /Users/Edit/{id}
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Guid id,
            [Bind("Id,Correo,HashContrasena,Rol,PlanSuscripcionId")] Usuario user)
        {
            if (id != user.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewData["Planes"] = new SelectList(_ctx.Planes, "Id", "Nombre", user.PlanSuscripcionId);
                return View(user);
            }

            _ctx.Update(user);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Users/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _ctx.Usuarios
                .Include(u => u.PlanSuscripcion)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: /Users/Delete/{id}
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var user = await _ctx.Usuarios.FindAsync(id);
            if (user != null)
            {
                _ctx.Usuarios.Remove(user);
                await _ctx.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
