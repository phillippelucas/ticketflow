using TicketFlow.Application.Common.Models;
using TicketFlow.Application.Eventos.Dtos;

namespace TicketFlow.Application.Eventos;

public interface IEventoService
{
    Task<Result<EventoDto>> CriarAsync(CriarEventoDto dto, CancellationToken cancellationToken);
    Task<Result<EventoDto>> ObterAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<EventoDto>> ListarAsync(CancellationToken cancellationToken);
    Task<Result<EventoDto>> AtualizarAsync(Guid id, AtualizarEventoDto dto, CancellationToken cancellationToken);
    Task<Result> DeletarAsync(Guid id, CancellationToken cancellationToken);
}
