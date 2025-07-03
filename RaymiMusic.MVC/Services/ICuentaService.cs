using RaymiMusic.Modelos.DTOs.Cuenta;

namespace RaymiMusic.MVC.Services
{
    public interface ICuentaService
    {
        public Task<LoginResponse> Login(LoginRequest loginRequest);

    }
}
