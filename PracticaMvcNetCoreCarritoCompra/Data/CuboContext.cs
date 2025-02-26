using Microsoft.EntityFrameworkCore;
using PracticaMvcNetCoreCarritoCompra.Models;

namespace PracticaMvcNetCoreCarritoCompra.Data
{
    public class CuboContext: DbContext
    {
        public CuboContext(DbContextOptions<CuboContext> options): base(options) { }

        public DbSet<Cubo> cubos { get; set; }
    }
}
