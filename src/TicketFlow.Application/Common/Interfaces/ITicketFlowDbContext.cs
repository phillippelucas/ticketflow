using Microsoft.EntityFrameworkCore;
using TicketFlow.Domain.Entities;

namespace TicketFlow.Application.Common.Interfaces;

public interface ITicketFlowDbContext
{
    DbSet<Evento> Eventos { get; }
    DbSet<Lote> Lotes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
