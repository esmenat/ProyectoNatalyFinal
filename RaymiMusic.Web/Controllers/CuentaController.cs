using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaymiMusic.Api.Data;
using RaymiMusic.Modelos.DTOs.Cuenta;

namespace RaymiMusic.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaController : ControllerBase
    {

        private readonly AppDbContext _context;

        public CuentaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == request.Correo);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.HashContrasena))
            {
                return Unauthorized("Correo o contraseña incorrectos.");
            }
            // Aquí podrías generar un token JWT o establecer una sesión
           var loginResponse = new LoginResponse
           {
               UsuarioId = usuario.Id,
               Correo = usuario.Correo,
               Rol = usuario.Rol
           };
            return Ok(loginResponse);
        }
    }
}
