using Microsoft.AspNetCore.Http.HttpResults;
using TicketFlow.Api.Extensions;
using TicketFlow.Application.Eventos;
using TicketFlow.Application.Eventos.Dtos;

namespace TicketFlow.Api.Endpoints;

public sealed class EventosEndpoints : IEndpointGroup
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/eventos").WithTags("Eventos");

        group.MapGet("/", ListarEventos)
            .WithName("ListarEventos")
            .Produces<IReadOnlyList<EventoDto>>();

        group.MapGet("/{id:guid}", ObterEvento)
            .WithName("ObterEvento")
            .Produces<EventoDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CriarEvento)
            .WithName("CriarEvento")
            .Produces<EventoDto>(StatusCodes.Status201Created);

        group.MapPut("/{id:guid}", AtualizarEvento)
            .WithName("AtualizarEvento")
            .Produces<EventoDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeletarEvento)
            .WithName("DeletarEvento")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<Ok<IReadOnlyList<EventoDto>>> ListarEventos(
        IEventoService eventoService,
        CancellationToken cancellationToken)
    {
        var eventos = await eventoService.ListarAsync(cancellationToken);
        return TypedResults.Ok(eventos);
    }

    private static async Task<Results<Ok<EventoDto>, NotFound>> ObterEvento(
        Guid id,
        IEventoService eventoService,
        CancellationToken cancellationToken)
    {
        var evento = await eventoService.ObterAsync(id, cancellationToken);
        return evento is not null ? TypedResults.Ok(evento) : TypedResults.NotFound();
    }

    private static async Task<Created<EventoDto>> CriarEvento(
        CriarEventoDto dto,
        IEventoService eventoService,
        CancellationToken cancellationToken)
    {
        var evento = await eventoService.CriarAsync(dto, cancellationToken);
        return TypedResults.Created($"/api/eventos/{evento.Id}", evento);
    }

    private static async Task<Results<Ok<EventoDto>, NotFound>> AtualizarEvento(
        Guid id,
        AtualizarEventoDto dto,
        IEventoService eventoService,
        CancellationToken cancellationToken)
    {
        var evento = await eventoService.AtualizarAsync(id, dto, cancellationToken);
        return evento is not null ? TypedResults.Ok(evento) : TypedResults.NotFound();
    }

    private static async Task<Results<NoContent, NotFound>> DeletarEvento(
        Guid id,
        IEventoService eventoService,
        CancellationToken cancellationToken)
    {
        var deletado = await eventoService.DeletarAsync(id, cancellationToken);
        return deletado ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
