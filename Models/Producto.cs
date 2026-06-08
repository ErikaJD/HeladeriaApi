using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeladeriaAPI.Models;

[Table("productos")]
public class Producto
{
    [Key]
    [Column("id_producto")]
    public int Id_Producto { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("precio")]
    public decimal Precio { get; set; }

    [Column("stock")]
    public int Stock { get; set; }

    [Column("id_categoria")]
    public int Id_Categoria { get; set; }
}