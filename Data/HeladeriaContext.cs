
using HeladeriaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HeladeriaAPI.Data;

public class HeladeriaContext : DbContext
{
    public HeladeriaContext(
        DbContextOptions<HeladeriaContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallePedidos => Set<DetallePedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>()
            .ToTable("clientes");

        modelBuilder.Entity<Categoria>()
            .ToTable("categorias");

        modelBuilder.Entity<Producto>()
            .ToTable("productos");

        modelBuilder.Entity<Pedido>()
            .ToTable("pedidos");

        modelBuilder.Entity<DetallePedido>()
            .ToTable("detalle_pedidos");
    }
}