using Microsoft.AspNetCore.Mvc;
using PracticaMvcNetCoreCarritoCompra.Models;
using PracticaMvcNetCoreCarritoCompra.Repositories;

namespace PracticaMvcNetCoreCarritoCompra.Controllers
{
    public class CubosController : Controller
    {
        private IRepositoryCubos repo;

        public CubosController(IRepositoryCubos repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            List<Cubo> cubos = await this.repo.GetCubosAsync();
            return View(cubos);
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
