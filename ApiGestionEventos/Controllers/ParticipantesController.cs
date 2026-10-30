using ApiGestionEventos.Data;
using ApiGestionEventos.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiGestionEventos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipantesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ParticipantesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var personas = await _db.Participantes
            .AsNoTracking()
            .OrderBy(p => p.NombreCompleto)
            .ToListAsync();

        return Ok(personas);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var persona = await _db.Participantes.FindAsync(id);

        if (persona is null)
            return NotFound("No existe este participante.");

        return Ok(persona);
    }

    [HttpPost]
    public async Task<IActionResult> Inscribir([FromBody] Participante datos)
    {
        if (string.IsNullOrWhiteSpace(datos.NombreCompleto) ||
            string.IsNullOrWhiteSpace(datos.CorreoElectronico))
        {
            return BadRequest("Debes proporcionar nombre y correo.");
        }

        var evento = await _db.Eventos.FindAsync(datos.EventoId);

        if (evento is null)
            return NotFound("El evento seleccionado no existe.");

        if (evento.CupoDisponible <= 0)
            return Conflict("Se agotaron los lugares disponibles.");

        string correo = datos.CorreoElectronico.Trim().ToLowerInvariant();

        bool registrado = await _db.Participantes
            .AnyAsync(p =>
                p.EventoId == datos.EventoId &&
                p.CorreoElectronico.ToLower() == correo);

        if (registrado)
            return Conflict("Este correo ya tiene una inscripcion.");

        datos.Id = 0;
        datos.NombreCompleto = datos.NombreCompleto.Trim();
        datos.CorreoElectronico = correo;

        _db.Participantes.Add(datos);
        evento.CupoDisponible -= 1;

        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { id = datos.Id },
            datos
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] Participante cambios)
    {
        if (cambios.Id != 0 && cambios.Id != id)
            return BadRequest("El ID enviado es incorrecto.");

        if (string.IsNullOrWhiteSpace(cambios.NombreCompleto) ||
            string.IsNullOrWhiteSpace(cambios.CorreoElectronico))
        {
            return BadRequest("Faltan datos del participante.");
        }

        var persona = await _db.Participantes.FindAsync(id);

        if (persona is null)
            return NotFound("No se encontro al participante.");

        var destino = await _db.Eventos.FindAsync(cambios.EventoId);

        if (destino is null)
            return NotFound("El evento de destino no existe.");

        string correo = cambios.CorreoElectronico.Trim().ToLowerInvariant();

        bool duplicado = await _db.Participantes
            .AnyAsync(p =>
                p.Id != id &&
                p.EventoId == cambios.EventoId &&
                p.CorreoElectronico.ToLower() == correo);

        if (duplicado)
            return Conflict("El correo ya esta inscrito en ese evento.");

        if (persona.EventoId != cambios.EventoId)
        {
            if (destino.CupoDisponible <= 0)
                return Conflict("No hay espacio en el nuevo evento.");

            var origen = await _db.Eventos.FindAsync(persona.EventoId);

            if (origen != null)
                origen.CupoDisponible += 1;

            destino.CupoDisponible -= 1;
        }

        persona.NombreCompleto = cambios.NombreCompleto.Trim();
        persona.CorreoElectronico = correo;
        persona.EventoId = cambios.EventoId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancelar(int id)
    {
        var persona = await _db.Participantes.FindAsync(id);

        if (persona is null)
            return NotFound("No existe la inscripcion solicitada.");

        var evento = await _db.Eventos.FindAsync(persona.EventoId);

        if (evento != null)
            evento.CupoDisponible += 1;

        _db.Participantes.Remove(persona);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
