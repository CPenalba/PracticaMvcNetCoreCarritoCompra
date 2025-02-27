using Microsoft.EntityFrameworkCore;
using PracticaMvcNetCoreCarritoCompra.Data;
using PracticaMvcNetCoreCarritoCompra.Models;

namespace PracticaMvcNetCoreCarritoCompra.Repositories
{
    public class RepositoryCubos : IRepositoryCubos
    {
        private CuboContext context;

        public RepositoryCubos(CuboContext context)
        {
            this.context = context;
        }

        public async Task<List<Cubo>> GetCubosAsync()
        {
            var consulta = from cubo in this.context.Cubos select cubo;
            return await consulta.ToListAsync();
        }


        public async Task<List<Cubo>> GetCubosSessionAsync(List<int> ids)
        {
            var consulta = from cubo in this.context.Cubos where ids.Contains(cubo.IdCubo) select cubo;
            if (consulta.Count() == 0)
            {
                return null;
            }
            else
            {
                return await consulta.ToListAsync();
            }
        }

        public async Task<Cubo> FindCuboAsync(int idCubo)
        {
            var consulta = from cubo in this.context.Cubos where cubo.IdCubo == idCubo select cubo;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task InsertCuboAsync(Cubo cubo)
        {
            this.context.Cubos.Add(cubo);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateCuboAsync(Cubo cubo)
        {
            this.context.Cubos.Update(cubo);
            await this.context.SaveChangesAsync();
        }
    }
}
