using TicketFlow.Application.Eventos.Dtos;

namespace TicketFlow.Application.Eventos;

public interface IEventoService
{
    Task<EventoDto> CriarAsync(CriarEventoDto dto, CancellationToken cancellationToken);
    Task<EventoDto?> ObterAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<EventoDto>> ListarAsync(CancellationToken cancellationToken);
    Task<EventoDto?> AtualizarAsync(Guid id, AtualizarEventoDto dto, CancellationToken cancellationToken);
    Task<bool> DeletarAsync(Guid id, CancellationToken cancellationToken);
}
