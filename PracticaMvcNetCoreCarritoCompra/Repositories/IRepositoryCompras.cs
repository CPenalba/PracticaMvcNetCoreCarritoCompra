using PracticaMvcNetCoreCarritoCompra.Models;

namespace PracticaMvcNetCoreCarritoCompra.Repositories
{
    public interface IRepositoryCompras
    {
        Task<List<Compra>> GetComprasAsync();

        Task InsertarComprasAsync(List<Compra> compras);

    }
}
