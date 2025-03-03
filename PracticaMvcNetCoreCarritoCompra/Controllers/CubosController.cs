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
        private IRepositoryCompras repoCompras;
        private IMemoryCache memoryCache;

        public CubosController(IRepositoryCubos repo, IRepositoryCompras repoCompras, IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.repoCompras = repoCompras;
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
            List<CuboCantidad> cantidades = HttpContext.Session.GetObject<List<CuboCantidad>>("CANTIDADES");

            if (idsCubos == null || !idsCubos.Any())
            {
                ViewData["MENSAJE"] = "No existen cubos almacenados en Session.";
                ViewData["PRECIO_TOTAL"] = 0;
                return View();
            }

            if (idEliminar != null)
            {
                idsCubos.Remove(idEliminar.Value);
                if (cantidades != null)
                {
                    cantidades.RemoveAll(x => x.IdCubo == idEliminar.Value);
                }

                if (idsCubos.Count == 0)
                {
                    HttpContext.Session.Remove("IDSCUBOS");
                    HttpContext.Session.Remove("CANTIDADES");
                }
                else
                {
                    HttpContext.Session.SetObject("IDSCUBOS", idsCubos);
                    if (cantidades != null)
                    {
                        HttpContext.Session.SetObject("CANTIDADES", cantidades);
                    }
                }
            }

            List<Cubo> cubos = await this.repo.GetCubosSessionAsync(idsCubos);
            int precioTotal = 0;

            if (cubos != null)
            {
                foreach (var cubo in cubos)
                {
                    int cantidad = 1;

                    if (cantidades != null)
                    {
                        CuboCantidad cuboCantidad = cantidades.Find(x => x.IdCubo == cubo.IdCubo);
                        if (cuboCantidad != null)
                        {
                            cantidad = cuboCantidad.Cantidad;
                        }
                    }
                    ViewData[$"CANTIDAD_{cubo.IdCubo}"] = cantidad;
                    precioTotal += cubo.Precio * cantidad;
                }
            }
            ViewData["PRECIO_TOTAL"] = precioTotal;
            return View(cubos);
        }

        public async Task<IActionResult> CambiarCantidad(int idCubo, int cantidad)
        {
            List<CuboCantidad> cantidades;

            if (HttpContext.Session.GetObject<List<CuboCantidad>>("CANTIDADES") == null)
            {
                cantidades = new List<CuboCantidad>();
            }
            else
            {
                cantidades = HttpContext.Session.GetObject<List<CuboCantidad>>("CANTIDADES");
            }
            CuboCantidad cuboCantidad = cantidades.Find(x => x.IdCubo == idCubo);
            if (cuboCantidad != null)
            {
                cuboCantidad.Cantidad = cantidad;
            }
            else
            {
                cantidades.Add(new CuboCantidad { IdCubo = idCubo, Cantidad = cantidad });
            }
            HttpContext.Session.SetObject("CANTIDADES", cantidades);

            return RedirectToAction("CarritoCompra");
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

        public async Task<IActionResult> CompraFinalizada()
        {
            List<int> idsCubos = HttpContext.Session.GetObject<List<int>>("IDSCUBOS");
            List<CuboCantidad> cantidades = HttpContext.Session.GetObject<List<CuboCantidad>>("CANTIDADES");

            if (idsCubos == null || !idsCubos.Any())
            {
                ViewData["MENSAJE"] = "No existen cubos en el carrito.";
                return View(new List<CompraFinalizadaView>());
            }

            List<Cubo> cubos = await this.repo.GetCubosSessionAsync(idsCubos);
            List<CompraFinalizadaView> compraDetalles = new List<CompraFinalizadaView>();
            int precioFinal = 0;

            foreach (var cubo in cubos)
            {
                int cantidad = 1;
                if (cantidades != null)
                {
                    CuboCantidad cuboCantidad = cantidades.Find(x => x.IdCubo == cubo.IdCubo);
                    if (cuboCantidad != null)
                    {
                        cantidad = cuboCantidad.Cantidad;
                    }
                }

                CompraFinalizadaView detalle = new CompraFinalizadaView
                {
                    NombreCubo = cubo.Nombre,
                    PrecioUnitario = cubo.Precio,
                    Cantidad = cantidad,
                    PrecioTotal = cubo.Precio * cantidad,
                    FechaPedido = DateTime.Now
                };

                compraDetalles.Add(detalle);
                precioFinal += detalle.PrecioTotal;
            }

            List<Compra> compras = compraDetalles.Select(d => new Compra
            {
                IdCubo = cubos.First(c => c.Nombre == d.NombreCubo).IdCubo,
                Cantidad = d.Cantidad,
                Precio = d.PrecioUnitario,
                FechaPedido = d.FechaPedido
            }).ToList();

            await this.repoCompras.InsertarComprasAsync(compras);
            HttpContext.Session.Remove("IDSCUBOS");
            HttpContext.Session.Remove("CANTIDADES");
            ViewData["PRECIO_FINAL"] = precioFinal;
            return View(compraDetalles);
        }

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