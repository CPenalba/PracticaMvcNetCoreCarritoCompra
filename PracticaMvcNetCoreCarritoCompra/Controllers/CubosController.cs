﻿using Microsoft.AspNetCore.Mvc;
using PracticaMvcNetCoreCarritoCompra.Extensions;
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
        public async Task<IActionResult> Index(int? idCubo)
        {
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

        public async Task<IActionResult> CarritoCompra(int? idCubo, int? idEliminar)
        {
            List<int> idsCubos = HttpContext.Session.GetObject<List<int>>("IDSCUBOS") ?? new List<int>();

            if (idCubo != null)
            {
                if (!idsCubos.Contains(idCubo.Value))
                {
                    idsCubos.Add(idCubo.Value);
                }
            }

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
            else
            {
                HttpContext.Session.SetObject("IDSCUBOS", idsCubos);
            }
            List<Cubo> cubos = await this.repo.GetCubosSessionAsync(idsCubos);
            return View(cubos);
        }

        [HttpPost]
        public IActionResult ActualizarCantidad(int idCubo, int cantidad)
        {
            // Obtener la lista de cubos con sus cantidades de la sesión
            var cubosConCantidad = HttpContext.Session.GetObject<List<CuboConCantidad>>("CUBOS_CON_CANTIDAD") ?? new List<CuboConCantidad>();

            // Buscar si ya existe el cubo en la lista
            var cuboExistente = cubosConCantidad.FirstOrDefault(c => c.IdCubo == idCubo);

            if (cuboExistente != null)
            {
                // Si el cubo ya existe, actualizamos su cantidad
                cuboExistente.Cantidad = cantidad;
            }
            else
            {
                // Si el cubo no existe, lo agregamos con la cantidad seleccionada
                cubosConCantidad.Add(new CuboConCantidad { IdCubo = idCubo, Cantidad = cantidad });
            }

            // Guardar la lista actualizada en la sesión
            HttpContext.Session.SetObject("CUBOS_CON_CANTIDAD", cubosConCantidad);

            return Ok();
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