using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketFlow.Domain.Entities;

namespace TicketFlow.Infrastructure.Persistence.Configurations;

public class LoteConfiguration : IEntityTypeConfiguration<Lote>
{
    public void Configure(EntityTypeBuilder<Lote> builder)
    {
        builder.ToTable("Lotes");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Preco)
            .HasPrecision(18, 2);

        builder.HasIndex(l => l.EventoId);

        builder.HasOne<Evento>()
            .WithMany()
            .HasForeignKey(l => l.EventoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
