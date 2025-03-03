namespace PracticaMvcNetCoreCarritoCompra.Models
{
    public class CompraFinalizadaView
    {
        public string NombreCubo { get; set; }
        public int PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public int PrecioTotal { get; set; }
        public DateTime FechaPedido { get; set; }
    }
}
