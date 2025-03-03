using Microsoft.EntityFrameworkCore;
using PracticaMvcNetCoreCarritoCompra.Data;
using PracticaMvcNetCoreCarritoCompra.Models;

namespace PracticaMvcNetCoreCarritoCompra.Repositories
{
    public class RepositoryCompras : IRepositoryCompras
    {
        private CuboContext context;

        public RepositoryCompras(CuboContext context)
        {
            this.context = context;
        }

        public async Task<List<Compra>> GetComprasAsync()
        {
            var consulta = from compra in this.context.Compras select compra;
            return await consulta.ToListAsync();
        }

        public async Task InsertarComprasAsync(List<Compra> compras)
        {
            int maxId = await this.context.Compras.MaxAsync(c => (int?)c.IdCompra) ?? 0;

            foreach (var compra in compras)
            {
                compra.IdCompra = ++maxId;
                this.context.Compras.Add(compra);
            }

            await this.context.SaveChangesAsync();
        }

    }
}
