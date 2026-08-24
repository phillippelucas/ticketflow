using System.Linq.Expressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TicketFlow.Application.Common.Interfaces;
using TicketFlow.Application.Common.Models;
using TicketFlow.Application.Eventos.Dtos;
using TicketFlow.Domain.Entities;

namespace TicketFlow.Application.Eventos;

public sealed class EventoService(
    ITicketFlowDbContext db,
    TimeProvider timeProvider,
    IValidator<CriarEventoDto> criarValidator,
    IValidator<AtualizarEventoDto> atualizarValidator) : IEventoService
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

    public async Task<Result<EventoDto>> CriarAsync(CriarEventoDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await criarValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Validation<EventoDto>(ToValidationErrors(validationResult));
        }

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

        return Result.Success(ToDto(evento));
    }

    public async Task<Result<EventoDto>> ObterAsync(Guid id, CancellationToken cancellationToken)
    {
        var evento = await db.Eventos
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(ProjectToDto)
            .FirstOrDefaultAsync(cancellationToken);

        return evento is not null
            ? Result.Success(evento)
            : Result.NotFound<EventoDto>($"Evento '{id}' não encontrado.");
    }

    public async Task<IReadOnlyList<EventoDto>> ListarAsync(CancellationToken cancellationToken) =>
        await db.Eventos
            .AsNoTracking()
            .OrderBy(e => e.DataInicio)
            .Select(ProjectToDto)
            .ToListAsync(cancellationToken);

    public async Task<Result<EventoDto>> AtualizarAsync(Guid id, AtualizarEventoDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await atualizarValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Validation<EventoDto>(ToValidationErrors(validationResult));
        }

        var evento = await db.Eventos.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (evento is null)
        {
            return Result.NotFound<EventoDto>($"Evento '{id}' não encontrado.");
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

        return Result.Success(ToDto(evento));
    }

    public async Task<Result> DeletarAsync(Guid id, CancellationToken cancellationToken)
    {
        var evento = await db.Eventos.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (evento is null)
        {
            return Result.NotFound($"Evento '{id}' não encontrado.");
        }

        db.Eventos.Remove(evento);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static Dictionary<string, string[]> ToValidationErrors(FluentValidation.Results.ValidationResult validationResult) =>
        validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

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
