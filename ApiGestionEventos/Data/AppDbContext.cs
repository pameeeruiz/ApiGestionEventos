using ApiGestionEventos.Modelos;
using Microsoft.EntityFrameworkCore;

namespace ApiGestionEventos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opciones)
        : base(opciones)
    {
    }

    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Participante> Participantes => Set<Participante>();

    protected override void OnModelCreating(ModelBuilder modelo)
    {
        base.OnModelCreating(modelo);

        modelo.Entity<Evento>()
            .HasMany(e => e.Participantes)
            .WithOne(p => p.Evento)
            .HasForeignKey(p => p.EventoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
