using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore; // Asegúrate de importar esta línea para trabajar con Entity Framework.
using RaymiMusic.AppWeb.Models;
using RaymiMusic.Api.Data;
using RaymiMusic.AppWeb.Services;  // Importa el contexto de la base de datos

public class ExplorarTodoController : Controller
{
    private readonly AppDbContext _context;  // Agregar el DbContext
    private readonly IArtistService _artistasService;
    private readonly ISongService _songsService;
    private readonly IAlbumsService _albumsService;
    private readonly ILogger<ExplorarTodoController> _logger;
    private readonly IGenerosService _generosService; 

    public ExplorarTodoController(AppDbContext context, IArtistService artistasService, ISongService songsService, IAlbumsService albumsService, ILogger<ExplorarTodoController> logger, IGenerosService generosService)
    {
        _context = context;  // Inyectar el contexto de la base de datos
        _artistasService = artistasService;
        _songsService = songsService;
        _albumsService = albumsService;
        _logger = logger;
        _generosService = generosService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            // Obtener géneros directamente desde la base de datos usando el contexto
            var generos = await _context.Generos.ToListAsync(); // Reemplaza "Generos" con el nombre de tu DbSet en el DbContext

            // Si no se encuentran géneros, mostrar un mensaje de advertencia
            if (generos == null || !generos.Any())
            {
                ViewBag.ErrorMessage = "No se encontraron géneros disponibles.";
                return View();
            }

            var artistas = await _artistasService.ObtenerArtistasAsync();
            var songs = await _songsService.GetAllAsync();
            var albums = await _albumsService.GetAlbumsAsync();

            var viewModel = new HomeIndexVM
            {
                Generos = generos,
                Artistas = artistas,
                Songs = songs,
                Albums = albums
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hubo un error al cargar los géneros.");
            ViewBag.ErrorMessage = "Hubo un error al cargar los géneros. Intente más tarde.";
            return View("Error");
        }
    }
    public async Task<IActionResult> CancionesGenero(Guid GeneroId)
    {
        var generos = await _generosService.GetGeneroByIdAsync(GeneroId);
        return View(generos);
    }
}
