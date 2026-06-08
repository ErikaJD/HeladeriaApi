using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeladeriaAPI.Models;

[Table("detalle_pedidos")]
public class DetallePedido
{
    [Key]
    [Column("id_detalle")]
    public int Id_Detalle { get; set; }

    [Column("id_pedido")]
    public int Id_Pedido { get; set; }

    [Column("id_producto")]
    public int Id_Producto { get; set; }

    [Column("cantidad")]
    public int Cantidad { get; set; }

    [Column("subtotal")]
    public decimal Subtotal { get; set; }
}