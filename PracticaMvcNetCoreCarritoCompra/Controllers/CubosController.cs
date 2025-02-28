using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PracticaMvcNetCoreCarritoCompra.Extensions;
using PracticaMvcNetCoreCarritoCompra.Models;
using PracticaMvcNetCoreCarritoCompra.Repositories;
using System.Security.Policy;

namespace PracticaMvcNetCoreCarritoCompra.Controllers
{
    public class CubosController : Controller
    {
        private IRepositoryCubos repo;
        private IMemoryCache memoryCache;

        public CubosController(IRepositoryCubos repo, IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.memoryCache = memoryCache;
        }
        public async Task<IActionResult> Index(int? idCubo, int? idfavorito)
        {

            if (idfavorito != null)
            {
                List<Cubo> cubosFavoritos;
                if (this.memoryCache.Get("FAVORITOS") == null)
                {
                    cubosFavoritos = new List<Cubo>();

                }
                else
                {
                    cubosFavoritos = this.memoryCache.Get<List<Cubo>>("FAVORITOS");
                }
                Cubo c = await this.repo.FindCuboAsync(idfavorito.Value);
                cubosFavoritos.Add(c);
                this.memoryCache.Set("FAVORITOS", cubosFavoritos);
            }

            if (idCubo != null)
            {
                List<int> idsCubos;
                if (HttpContext.Session.GetObject<List<int>>("IDSCUBOS") == null)
                {
                    idsCubos = new List<int>();
                }
                else
                {
                    idsCubos = HttpContext.Session.GetObject<List<int>>("IDSCUBOS");

                }
                idsCubos.Add(idCubo.Value);
                HttpContext.Session.SetObject("IDSCUBOS", idsCubos);
                ViewData["MENSAJE"] = "Cubos almacenados: " + idsCubos.Count;
            }
            List<Cubo> cubos = await this.repo.GetCubosAsync();
            return View(cubos);
        }

        public async Task<IActionResult> CarritoCompra(int? idEliminar)
        {
            List<int> idsCubos = HttpContext.Session.GetObject<List<int>>("IDSCUBOS");

            if (idsCubos == null)
            {
                ViewData["MENSAJE"] = "No existen cubos almacenados en Session.";
                return View();
            }
            else
            {
                if (idEliminar != null)
                {
                    idsCubos.Remove(idEliminar.Value);
                    if (idsCubos.Count == 0)
                    {
                        HttpContext.Session.Remove("IDSCUBOS");
                    }
                    else
                    {
                        HttpContext.Session.SetObject("IDSCUBOS", idsCubos);
                    }
                }
                List<Cubo> cubos = await this.repo.GetCubosSessionAsync(idsCubos);
                return View(cubos);
            }
        }

        public IActionResult CubosFavoritos(int? ideliminar)
        {
            if (ideliminar != null)
            {
                List<Cubo> favoritos = this.memoryCache.Get<List<Cubo>>("FAVORITOS");
                Cubo cDelete = favoritos.Find(z => z.IdCubo == ideliminar.Value);
                favoritos.Remove(cDelete);
                if (favoritos.Count == 0)
                {
                    this.memoryCache.Remove("FAVORITOS");
                }
                else
                {
                    this.memoryCache.Set("FAVORITOS", favoritos);
                }
            }
            return View();
        }

        //[HttpPost]
        //public IActionResult ActualizarCantidad(int idCubo, int cantidad)
        //{
        //    // Obtener la lista de cubos con sus cantidades de la sesión
        //    var cubosConCantidad = HttpContext.Session.GetObject<List<CuboConCantidad>>("CUBOS_CON_CANTIDAD") ?? new List<CuboConCantidad>();

        //    // Buscar si ya existe el cubo en la lista
        //    var cuboExistente = cubosConCantidad.FirstOrDefault(c => c.IdCubo == idCubo);

        //    if (cuboExistente != null)
        //    {
        //        // Si el cubo ya existe, actualizamos su cantidad
        //        cuboExistente.Cantidad = cantidad;
        //    }
        //    else
        //    {
        //        // Si el cubo no existe, lo agregamos con la cantidad seleccionada
        //        cubosConCantidad.Add(new CuboConCantidad { IdCubo = idCubo, Cantidad = cantidad });
        //    }

        //    // Guardar la lista actualizada en la sesión
        //    HttpContext.Session.SetObject("CUBOS_CON_CANTIDAD", cubosConCantidad);

        //    return Ok();
        //}


        public async Task<IActionResult> Details(int idCubo)
        {
            Cubo c = await this.repo.FindCuboAsync(idCubo);
            return View(c);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cubo cubo, IFormFile imagen)
        {
            if (imagen != null && imagen.Length > 0)
            {
                var fileName = Path.GetFileName(imagen.FileName);
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var filePath = Path.Combine(directoryPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }
                cubo.Imagen = fileName;
            }
            await this.repo.InsertCuboAsync(cubo);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int idCubo)
        {
            Cubo c = await this.repo.FindCuboAsync(idCubo);
            return View(c);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Cubo cubo, IFormFile imagen)
        {
            if (imagen != null && imagen.Length > 0)
            {
                var fileName = Path.GetFileName(imagen.FileName);
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                var filePath = Path.Combine(directoryPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }
                cubo.Imagen = fileName;
            }

            await this.repo.UpdateCuboAsync(cubo);
            return RedirectToAction("Index");
        }

    }
}