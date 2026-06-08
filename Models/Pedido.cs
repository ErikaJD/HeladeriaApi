using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeladeriaAPI.Models;

[Table("pedidos")]
public class Pedido
{
    [Key]
    [Column("id_pedido")]
    public int Id_Pedido { get; set; }

    [Column("id_cliente")]
    public int Id_Cliente { get; set; }

    [Column("fecha")]
    public DateTime Fecha { get; set; }

    [Column("total")]
    public decimal Total { get; set; }
}