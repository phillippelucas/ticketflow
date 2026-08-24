using Microsoft.AspNetCore.Http.HttpResults;
using TicketFlow.Api.Extensions;
using TicketFlow.Application.Common.Models;
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
            .Produces<EventoDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", AtualizarEvento)
            .WithName("AtualizarEvento")
            .Produces<EventoDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        group.MapDelete("/{id:guid}", DeletarEvento)
            .WithName("DeletarEvento")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<EventoDto>, ProblemHttpResult>> ObterEvento(
        Guid id,
        IEventoService eventoService,
        CancellationToken cancellationToken)
    {
        var result = await eventoService.ObterAsync(id, cancellationToken);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : NotFoundProblem(result);
    }

    private static async Task<Ok<IReadOnlyList<EventoDto>>> ListarEventos(
        IEventoService eventoService,
        CancellationToken cancellationToken)
    {
        var eventos = await eventoService.ListarAsync(cancellationToken);
        return TypedResults.Ok(eventos);
    }

    private static async Task<Results<Created<EventoDto>, ValidationProblem>> CriarEvento(
        CriarEventoDto dto,
        IEventoService eventoService,
        CancellationToken cancellationToken)
    {
        var result = await eventoService.CriarAsync(dto, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Created($"/api/eventos/{result.Value.Id}", result.Value)
            : TypedResults.ValidationProblem(result.ValidationErrors!);
    }

    private static async Task<Results<Ok<EventoDto>, ProblemHttpResult, ValidationProblem>> AtualizarEvento(
        Guid id,
        AtualizarEventoDto dto,
        IEventoService eventoService,
        CancellationToken cancellationToken)
    {
        var result = await eventoService.AtualizarAsync(id, dto, cancellationToken);
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        return result.ErrorType == ResultErrorType.NotFound
            ? NotFoundProblem(result)
            : TypedResults.ValidationProblem(result.ValidationErrors!);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> DeletarEvento(
        Guid id,
        IEventoService eventoService,
        CancellationToken cancellationToken)
    {
        var result = await eventoService.DeletarAsync(id, cancellationToken);
        return result.IsSuccess ? TypedResults.NoContent() : NotFoundProblem(result);
    }

    private static ProblemHttpResult NotFoundProblem(Result result) =>
        TypedResults.Problem(
            title: "Recurso não encontrado.",
            detail: result.Error,
            statusCode: StatusCodes.Status404NotFound);
}
