using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;
using RaymiMusic.Data;
using RaymiMusic.Modelos;
using RaymiMusic.Modelos.DTOs.Cuenta;
using RaymiMusic.MVC.Services;
using System;

namespace RaymiMusic.MVC.Pages.Cuenta
{
    public class LoginModel : PageModel
    {
        private readonly ICuentaService _cuentaService;

        public LoginModel(ICuentaService cuentaService)
        {
            _cuentaService = cuentaService;
        }

        [BindProperty]
        public string Correo { get; set; } = null!;

        [BindProperty]
        public string Contrasena { get; set; } = null!;

        public string? ErrorMensaje { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            var loginRequest = new LoginRequest
            {
                Correo = Correo,
                Contrasena = Contrasena
            };

            try
            {
                var loginResponse = await _cuentaService.Login(loginRequest);

                HttpContext.Session.SetString("Rol", loginResponse.Rol); 

           
                if (loginResponse.Rol == "Admin")
                {
                    return RedirectToPage("/Usuarios/Index");
                }
                else
                {
                    return RedirectToPage("/ClienteVistas/InicioVista");
                }
            }
            catch (ApplicationException ex)
            {
                ErrorMensaje = ex.Message;
                return Page();
            }
        }

    }
}
