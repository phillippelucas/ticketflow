namespace TicketFlow.Application.Eventos.Dtos;

public sealed record AtualizarEventoDto(
    string Nome,
    string Descricao,
    DateTime DataInicio,
    DateTime DataFim,
    string Local,
    int CapacidadeTotal,
    decimal PrecoIngresso,
    bool Ativo);
