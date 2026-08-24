using System.Net;
using System.Net.Http.Json;
using TicketFlow.Application.Eventos.Dtos;
using TicketFlow.IntegrationTests.Fixtures;

namespace TicketFlow.IntegrationTests.Eventos;

public class EventosEndpointsTests(ApiFixture fixture) : IClassFixture<ApiFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task CriarEObterEvento_ComDadosValidos_RetornaEventoCriadoEPersistido()
    {
        // Arrange
        var dto = new CriarEventoDto(
            "Show de Integração",
            "Teste de fluxo completo",
            new DateTime(2026, 8, 1, 20, 0, 0),
            new DateTime(2026, 8, 1, 23, 0, 0),
            "Arena Central",
            1000,
            80m);

        // Act
        var createResponse = await _client.PostAsJsonAsync("/api/eventos", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var criado = await createResponse.Content.ReadFromJsonAsync<EventoDto>();
        Assert.NotNull(criado);
        Assert.NotEqual(Guid.Empty, criado.Id);
        Assert.Equal(dto.Nome, criado.Nome);
        Assert.Contains($"/api/eventos/{criado.Id}", createResponse.Headers.Location?.ToString());

        // Act
        var getResponse = await _client.GetAsync($"/api/eventos/{criado.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var obtido = await getResponse.Content.ReadFromJsonAsync<EventoDto>();
        Assert.NotNull(obtido);
        Assert.Equal(criado.Id, obtido.Id);
        Assert.Equal(dto.Nome, obtido.Nome);
        Assert.Equal(dto.CapacidadeTotal, obtido.CapacidadeTotal);
    }

    [Fact]
    public async Task CriarEvento_ComDadosInvalidos_RetornaValidationProblem()
    {
        // Arrange
        var dto = new CriarEventoDto(
            string.Empty,
            "Descrição",
            new DateTime(2026, 8, 1, 20, 0, 0),
            new DateTime(2026, 8, 1, 23, 0, 0),
            "Arena Central",
            1000,
            80m);

        // Act
        var response = await _client.PostAsJsonAsync("/api/eventos", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ObterEvento_ComIdInexistente_RetornaNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/eventos/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
