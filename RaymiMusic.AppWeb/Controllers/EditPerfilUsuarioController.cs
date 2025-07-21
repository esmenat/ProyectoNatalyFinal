using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;
using RaymiMusic.Modelos;
using RaymiMusic.Modelos.ViewModels;

namespace RaymiMusic.AppWeb.Controllers
{
    public class EditPerfilUsuarioController : Controller
    {
        private readonly AppDbContext _ctx;
        public EditPerfilUsuarioController(AppDbContext ctx) => _ctx = ctx;

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return NotFound();

            var usuario = await _ctx.Usuarios.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
            if (usuario == null)
                return NotFound();

            // Mapear el usuario a tu ViewModel
            var vm = new UsuarioPerfilVM
            {
                Id = usuario.Id,
                Correo = usuario.Correo
            };

            return View("Index", vm); // Enviar el ViewModel correcto a la vista
        }

        private string Hash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes); // ✅ MISMO QUE REGISTER Y LOGIN
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UsuarioPerfilVM vm)
        {
            if (id != vm.Id)
                return BadRequest();

            var existingUser = await _ctx.Usuarios.FindAsync(id);
            if (existingUser == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View("Index", vm);

            // Solo actualiza la contraseña si el campo fue rellenado
            if (!string.IsNullOrWhiteSpace(vm.HashContrasena))
            {
                existingUser.HashContrasena = Hash(vm.HashContrasena);
            }
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
