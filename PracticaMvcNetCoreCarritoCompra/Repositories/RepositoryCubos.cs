using Microsoft.EntityFrameworkCore;
using PracticaMvcNetCoreCarritoCompra.Data;
using PracticaMvcNetCoreCarritoCompra.Models;

namespace PracticaMvcNetCoreCarritoCompra.Repositories
{
    public class RepositoryCubos: IRepositoryCubos
    {
        private CuboContext context;

        public RepositoryCubos(CuboContext context)
        {
            this.context = context;
        }

        public async Task<List<Cubo>> GetCubosAsync()
        {
            var consulta = from cubo in this.context.cubos select cubo;
            return await consulta.ToListAsync();
        }

        public async Task<Cubo> FindCuboAsync(int idCubo)
        {
            var consulta = from cubo in this.context.cubos where cubo.IdCubo == idCubo select cubo;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task InsertCuboAsync(Cubo cubo)
        {
            this.context.cubos.Add(cubo);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateCuboAsync(Cubo cubo)
        {
            this.context.cubos.Update(cubo);
            await this.context.SaveChangesAsync();
        }
    }
}
