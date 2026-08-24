namespace TicketFlow.Application.Eventos.Dtos;

public sealed record EventoDto(
    Guid Id,
    string Nome,
    string Descricao,
    DateTime DataInicio,
    DateTime DataFim,
    string Local,
    int CapacidadeTotal,
    int IngressosVendidos,
    decimal PrecoIngresso,
    bool Ativo,
    DateTime CriadoEm);
