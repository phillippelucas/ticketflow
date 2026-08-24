using Microsoft.EntityFrameworkCore;
using TicketFlow.Application.Common.Interfaces;
using TicketFlow.Domain.Entities;

namespace TicketFlow.Infrastructure.Persistence;

public class TicketFlowDbContext(DbContextOptions<TicketFlowDbContext> options)
    : DbContext(options), ITicketFlowDbContext
{
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Lote> Lotes => Set<Lote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketFlowDbContext).Assembly);
    }
}
