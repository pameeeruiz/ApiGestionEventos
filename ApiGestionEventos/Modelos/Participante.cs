using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiGestionEventos.Modelos;

public class Participante
{
    public int Id { get; set; }

    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string NombreCompleto { get; set; } = "";

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string CorreoElectronico { get; set; } = "";

    [Range(1, int.MaxValue)]
    public int EventoId { get; set; }

    [JsonIgnore]
    public Evento? Evento { get; set; }
}
