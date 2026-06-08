using HeladeriaAPI.Data;
using HeladeriaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeladeriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly HeladeriaContext _context;

    public CategoriasController(HeladeriaContext context)
    {
        _context = context;
    }

    // 1. GET: api/Categorias (Listar todo)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
    {
        try
        {
            return await _context.Categorias.ToListAsync();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener categorías: {ex.Message} || Inner: {ex.InnerException?.Message}");
        }
    }

    // 2. GET: api/Categorias/5 (Obtener una sola)
    [HttpGet("{id}")]
    public async Task<ActionResult<Categoria>> GetCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria == null)
            return NotFound();

        return categoria;
    }

    // 3. POST: api/Categorias (Crear nueva)
    [HttpPost]
    public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
    {
        try
        {
            // Forzamos a que ignore el ID enviado para que Postgres use su SERIAL / IDENTITY autoincremental
            categoria.Id_Categoria = 0;

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return Ok(categoria);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al guardar categoría: {ex.Message} || Inner: {ex.InnerException?.Message}");
        }
    }

    // 4. PUT: api/Categorias/5 (Actualizar existente)
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
    {
        if (id != categoria.Id_Categoria)
            return BadRequest("El ID de la URL no coincide con el ID de la categoría enviada.");

        _context.Entry(categoria).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar: {ex.Message} || Inner: {ex.InnerException?.Message}");
        }

        return NoContent();
    }

    // 5. DELETE: api/Categorias/5 (Eliminar)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria == null)
            return NotFound();

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}