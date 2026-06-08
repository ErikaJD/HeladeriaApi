using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeladeriaAPI.Models;

[Table("clientes")]
public class Cliente
{
    [Key]
    [Column("id_cliente")]
    public int Id_Cliente { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("telefono")]
    public string? Telefono { get; set; }

    [Column("correo")]
    public string? Correo { get; set; }

    [Column("direccion")]
    public string? Direccion { get; set; }
}