using ApiGestionEventos.Data;
using ApiGestionEventos.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiGestionEventos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly AppDbContext _db;

    public EventosController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var eventos = await _db.Eventos
            .AsNoTracking()
            .OrderBy(e => e.Fecha)
            .ToListAsync();

        return Ok(eventos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var encontrado = await _db.Eventos.FindAsync(id);

        if (encontrado is null)
            return NotFound("No se encontro el evento solicitado.");

        return Ok(encontrado);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Evento datos)
    {
        if (string.IsNullOrWhiteSpace(datos.Nombre) ||
            string.IsNullOrWhiteSpace(datos.Lugar))
        {
            return BadRequest("Completa el nombre y el lugar.");
        }

        if (datos.Fecha == default)
            return BadRequest("Ingresa una fecha valida.");

        if (datos.CupoDisponible < 0)
            return BadRequest("El cupo debe ser positivo o cero.");

        datos.Id = 0;
        datos.Nombre = datos.Nombre.Trim();
        datos.Lugar = datos.Lugar.Trim();

        _db.Eventos.Add(datos);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { id = datos.Id },
            datos
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] Evento datos)
    {
        if (datos.Id != 0 && datos.Id != id)
            return BadRequest("Los identificadores no coinciden.");

        if (string.IsNullOrWhiteSpace(datos.Nombre) ||
            string.IsNullOrWhiteSpace(datos.Lugar) ||
            datos.Fecha == default ||
            datos.CupoDisponible < 0)
        {
            return BadRequest("Revisa la informacion enviada.");
        }

        var actual = await _db.Eventos.FindAsync(id);

        if (actual is null)
            return NotFound("No existe el evento que deseas modificar.");

        actual.Nombre = datos.Nombre.Trim();
        actual.Lugar = datos.Lugar.Trim();
        actual.Fecha = datos.Fecha;
        actual.CupoDisponible = datos.CupoDisponible;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Borrar(int id)
    {
        var evento = await _db.Eventos.FindAsync(id);

        if (evento is null)
            return NotFound("El evento indicado no existe.");

        bool tieneInscritos = await _db.Participantes
            .AnyAsync(p => p.EventoId == id);

        if (tieneInscritos)
            return Conflict("Primero debes eliminar las inscripciones de este evento.");

        _db.Eventos.Remove(evento);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
