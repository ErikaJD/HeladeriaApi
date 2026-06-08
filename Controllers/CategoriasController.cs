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

    // GET: api/Categorias
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
    {
        try
        {
            var categorias = await _context.Categorias.ToListAsync();
            return Ok(categorias);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.ToString());
        }
    }

    // GET: api/Categorias/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Categoria>> GetCategoria(int id)
    {
        try
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound("Categoría no encontrada.");

            return Ok(categoria);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.ToString());
        }
    }

    // POST: api/Categorias
    [HttpPost]
    public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
    {
        try
        {
            categoria.Id_Categoria = 0;

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return Ok(categoria);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.ToString());
        }
    }

    // PUT: api/Categorias/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
    {
        try
        {
            if (id != categoria.Id_Categoria)
                return BadRequest("El ID no coincide.");

            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.ToString());
        }
    }

    // DELETE: api/Categorias/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategoria(int id)
    {
        try
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound("Categoría no encontrada.");

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.ToString());
        }
    }
}