using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;
using RaymiMusic.Modelos;

namespace RaymiMusic.MVC.Pages.Cuenta
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Correo { get; set; } = null!;

        [BindProperty]
        public string Contrasena { get; set; } = null!;

        [BindProperty]
        public string TipoCuenta { get; set; }  // "Usuario" o "Artista"

        [BindProperty]
        public string Rol { get; set; }  // "Admin" o "Client"

        public string? ErrorMensaje { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _context.Usuarios.AnyAsync(u => u.Correo == Correo))
            {
                ErrorMensaje = "Ya existe una cuenta con este correo.";
                return Page();
            }

  
            string rolAsignado = Rol;

            string tipoCuenta = TipoCuenta == "Artista" ? "Artista" : "Free";

            var nuevoUsuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Correo = Correo,
                HashContrasena = BCrypt.Net.BCrypt.HashPassword(Contrasena),
                Rol = rolAsignado,  // Aquí se asigna el rol seleccionado
                PlanSuscripcionId = await _context.Planes
                    .Where(p => p.Nombre == "Free")
                    .Select(p => p.Id)
                    .FirstAsync()
            };

            _context.Usuarios.Add(nuevoUsuario);

            if (tipoCuenta == "Artista")
            {
                _context.Artistas.Add(new Artista
                {
                    Id = Guid.NewGuid(),
                    NombreArtistico = Correo
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Cuenta/Login");
        }
    }
}
