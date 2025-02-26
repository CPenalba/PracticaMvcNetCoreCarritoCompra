using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PracticaMvcNetCoreCarritoCompra.Models
{
    [Table("COMPRA")]
    public class Compra
    {
        [Key]
        [Column("id_compra")]
        public int IdCompra { get; set; }
        [Column("id_cubo")]
        public int IdCubo { get; set; }
        [Column("cantidad")]
        public int Cantidad { get; set; }
        [Column("precio")]
        public int Precio { get; set; }
        [Column("fechapedido")]
        public DateTime FechaPedido { get; set; }
    }
}
