using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using TicketFlow.Application.Common.Interfaces;
using TicketFlow.Application.Common.Models;
using TicketFlow.Application.Eventos;
using TicketFlow.Application.Eventos.Dtos;
using TicketFlow.Application.Eventos.Validators;
using TicketFlow.Domain.Entities;

namespace TicketFlow.UnitTests.Eventos;

public class EventoServiceTests
{
    private readonly ITicketFlowDbContext _db = Substitute.For<ITicketFlowDbContext>();
    private readonly DbSet<Evento> _eventosDbSet = Substitute.For<DbSet<Evento>>();
    private readonly FakeTimeProvider _timeProvider = new(new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero));
    private readonly EventoService _sut;

    public EventoServiceTests()
    {
        _db.Eventos.Returns(_eventosDbSet);
        _sut = new EventoService(_db, _timeProvider, new CriarEventoDtoValidator(), new AtualizarEventoDtoValidator());
    }

    [Fact]
    public async Task CriarAsync_ComDadosValidos_RetornaSucessoEPersisteEvento()
    {
        // Arrange
        var dto = new CriarEventoDto(
            "Show de Rock",
            "Um grande show",
            new DateTime(2026, 6, 1, 20, 0, 0),
            new DateTime(2026, 6, 1, 23, 0, 0),
            "Estádio Municipal",
            5000,
            150m);

        // Act
        var result = await _sut.CriarAsync(dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Nome.Should().Be(dto.Nome);
        result.Value.Ativo.Should().BeTrue();
        result.Value.IngressosVendidos.Should().Be(0);
        result.Value.CriadoEm.Should().Be(_timeProvider.GetUtcNow().UtcDateTime);

        _eventosDbSet.Received(1).Add(Arg.Is<Evento>(e => e.Nome == dto.Nome));
        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("", 100, 50, "Nome")]
    [InlineData("Show Válido", 0, 50, "CapacidadeTotal")]
    [InlineData("Show Válido", -1, 50, "CapacidadeTotal")]
    [InlineData("Show Válido", 100, -1, "PrecoIngresso")]
    public async Task CriarAsync_ComDadosInvalidos_RetornaFalhaDeValidacaoENaoPersiste(
        string nome, int capacidadeTotal, decimal precoIngresso, string campoComErro)
    {
        // Arrange
        var dto = new CriarEventoDto(
            nome,
            "Descrição",
            new DateTime(2026, 6, 1, 20, 0, 0),
            new DateTime(2026, 6, 1, 23, 0, 0),
            "Local",
            capacidadeTotal,
            precoIngresso);

        // Act
        var result = await _sut.CriarAsync(dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ResultErrorType.Validation);
        result.ValidationErrors.Should().ContainKey(campoComErro);

        _eventosDbSet.DidNotReceive().Add(Arg.Any<Evento>());
        await _db.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CriarAsync_ComDataFimAnteriorADataInicio_RetornaFalhaDeValidacaoENaoPersiste()
    {
        // Arrange
        var dto = new CriarEventoDto(
            "Show Válido",
            "Descrição",
            new DateTime(2026, 6, 1, 23, 0, 0),
            new DateTime(2026, 6, 1, 20, 0, 0),
            "Local",
            100,
            50m);

        // Act
        var result = await _sut.CriarAsync(dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ResultErrorType.Validation);
        result.ValidationErrors.Should().ContainKey(nameof(CriarEventoDto.DataInicio));

        _eventosDbSet.DidNotReceive().Add(Arg.Any<Evento>());
        await _db.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
