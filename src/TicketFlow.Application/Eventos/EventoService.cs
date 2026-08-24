using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TicketFlow.Application.Common.Interfaces;
using TicketFlow.Application.Eventos.Dtos;
using TicketFlow.Domain.Entities;

namespace TicketFlow.Application.Eventos;

public sealed class EventoService(ITicketFlowDbContext db, TimeProvider timeProvider) : IEventoService
{
    private static readonly Expression<Func<Evento, EventoDto>> ProjectToDto = e => new EventoDto(
        e.Id,
        e.Nome,
        e.Descricao,
        e.DataInicio,
        e.DataFim,
        e.Local,
        e.CapacidadeTotal,
        e.IngressosVendidos,
        e.PrecoIngresso,
        e.Ativo,
        e.CriadoEm);

    public async Task<EventoDto> CriarAsync(CriarEventoDto dto, CancellationToken cancellationToken)
    {
        var evento = new Evento(
            dto.Nome,
            dto.Descricao,
            dto.DataInicio,
            dto.DataFim,
            dto.Local,
            dto.CapacidadeTotal,
            dto.PrecoIngresso,
            timeProvider.GetUtcNow().UtcDateTime);

        db.Eventos.Add(evento);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(evento);
    }

    public Task<EventoDto?> ObterAsync(Guid id, CancellationToken cancellationToken) =>
        db.Eventos
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(ProjectToDto)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<EventoDto>> ListarAsync(CancellationToken cancellationToken) =>
        await db.Eventos
            .AsNoTracking()
            .OrderBy(e => e.DataInicio)
            .Select(ProjectToDto)
            .ToListAsync(cancellationToken);

    public async Task<EventoDto?> AtualizarAsync(Guid id, AtualizarEventoDto dto, CancellationToken cancellationToken)
    {
        var evento = await db.Eventos.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (evento is null)
        {
            return null;
        }

        evento.AtualizarDados(
            dto.Nome,
            dto.Descricao,
            dto.DataInicio,
            dto.DataFim,
            dto.Local,
            dto.CapacidadeTotal,
            dto.PrecoIngresso,
            dto.Ativo);

        await db.SaveChangesAsync(cancellationToken);

        return ToDto(evento);
    }

    public async Task<bool> DeletarAsync(Guid id, CancellationToken cancellationToken)
    {
        var evento = await db.Eventos.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (evento is null)
        {
            return false;
        }

        db.Eventos.Remove(evento);
        await db.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static EventoDto ToDto(Evento e) => new(
        e.Id,
        e.Nome,
        e.Descricao,
        e.DataInicio,
        e.DataFim,
        e.Local,
        e.CapacidadeTotal,
        e.IngressosVendidos,
        e.PrecoIngresso,
        e.Ativo,
        e.CriadoEm);
}
