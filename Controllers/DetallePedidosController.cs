using HeladeriaAPI.Data;
using HeladeriaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeladeriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetallePedidosController : ControllerBase
{
    private readonly HeladeriaContext _context;

    public DetallePedidosController(HeladeriaContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetallePedido>>> GetDetalles()
    {
        return await _context.DetallePedidos.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DetallePedido>> GetDetalle(int id)
    {
        var detalle = await _context.DetallePedidos.FindAsync(id);

        if (detalle == null)
            return NotFound();

        return detalle;
    }

    [HttpPost]
    public async Task<ActionResult<DetallePedido>> PostDetalle(DetallePedido detalle)
    {
        _context.DetallePedidos.Add(detalle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetDetalle),
            new { id = detalle.Id_Detalle },
            detalle);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutDetalle(
        int id,
        DetallePedido detalle)
    {
        if (id != detalle.Id_Detalle)
            return BadRequest();

        _context.Entry(detalle).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDetalle(int id)
    {
        var detalle = await _context.DetallePedidos.FindAsync(id);

        if (detalle == null)
            return NotFound();

        _context.DetallePedidos.Remove(detalle);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}