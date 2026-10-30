using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiGestionEventos.Modelos;

public class Evento
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Nombre { get; set; } = "";

    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Lugar { get; set; } = "";

    [Required]
    public DateTime Fecha { get; set; }

    [Range(0, int.MaxValue)]
    public int CupoDisponible { get; set; }

    [JsonIgnore]
    public ICollection<Participante> Participantes { get; set; }
        = new List<Participante>();
}
