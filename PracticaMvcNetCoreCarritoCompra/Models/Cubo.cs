using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PracticaMvcNetCoreCarritoCompra.Models
{
    [Table("cubos")]
    public class Cubo
    {
        [Key]
        [Column("id_cubo")]
        public int IdCubo { get; set; }
        [Column("nombre")]
        public string Nombre { get; set; }
        [Column("modelo")]
        public string Modelo { get; set; }
        [Column("marca")]
        public string Marca { get; set; }
        [Column("imagen")]
        public string Imagen { get; set; }
        [Column("precio")]
        public int Precio { get; set; }
    }
}
