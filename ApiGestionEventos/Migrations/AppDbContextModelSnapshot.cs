using System;
using ApiGestionEventos.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ApiGestionEventos.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "10.0.14");

            modelBuilder.Entity("ApiGestionEventos.Modelos.Evento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Sqlite:Autoincrement", true);

                    b.Property<int>("CupoDisponible").HasColumnType("INTEGER");
                    b.Property<DateTime>("Fecha").HasColumnType("TEXT");
                    b.Property<string>("Lugar").IsRequired().HasMaxLength(120).HasColumnType("TEXT");
                    b.Property<string>("Nombre").IsRequired().HasMaxLength(100).HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.ToTable("Eventos");
                });

            modelBuilder.Entity("ApiGestionEventos.Modelos.Participante", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Sqlite:Autoincrement", true);

                    b.Property<string>("CorreoElectronico").IsRequired().HasMaxLength(150).HasColumnType("TEXT");
                    b.Property<int>("EventoId").HasColumnType("INTEGER");
                    b.Property<string>("NombreCompleto").IsRequired().HasMaxLength(120).HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("EventoId");
                    b.ToTable("Participantes");
                });

            modelBuilder.Entity("ApiGestionEventos.Modelos.Participante", b =>
                {
                    b.HasOne("ApiGestionEventos.Modelos.Evento", "Evento")
                        .WithMany("Participantes")
                        .HasForeignKey("EventoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Evento");
                });

            modelBuilder.Entity("ApiGestionEventos.Modelos.Evento", b =>
                {
                    b.Navigation("Participantes");
                });

#pragma warning restore 612, 618
        }
    }
}
