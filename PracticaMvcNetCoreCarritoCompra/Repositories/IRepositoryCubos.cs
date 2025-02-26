using PracticaMvcNetCoreCarritoCompra.Models;

namespace PracticaMvcNetCoreCarritoCompra.Repositories
{
    public interface IRepositoryCubos
    {
        Task<List<Cubo>> GetCubosAsync();

        Task<Cubo> FindCuboAsync(int idCubo);

        Task InsertCuboAsync(Cubo cubo);
    }
}
