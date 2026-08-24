using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketFlow.Domain.Entities;

namespace TicketFlow.Infrastructure.Persistence.Configurations;

public class EventoConfiguration : IEntityTypeConfiguration<Evento>
{
    public void Configure(EntityTypeBuilder<Evento> builder)
    {
        builder.ToTable("Eventos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Descricao)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.Local)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.PrecoIngresso)
            .HasPrecision(18, 2);

        builder.Property(e => e.CriadoEm)
            .IsRequired();
    }
}
