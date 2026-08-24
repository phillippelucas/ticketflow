using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MySql;
using TicketFlow.Infrastructure.Persistence;

namespace TicketFlow.IntegrationTests.Fixtures;

public class ApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer = new MySqlBuilder("mysql:8.4")
        .WithDatabase("ticketflow_test")
        .WithUsername("ticketflow")
        .WithPassword("ticketflow")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<TicketFlowDbContext>>();
            services.AddDbContext<TicketFlowDbContext>(options =>
                options.UseMySql(
                    _mySqlContainer.GetConnectionString(),
                    ServerVersion.AutoDetect(_mySqlContainer.GetConnectionString())));
        });
    }

    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TicketFlowDbContext>();
        await db.Database.MigrateAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await _mySqlContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    // xUnit v2's IAsyncLifetime.DisposeAsync returns Task, while WebApplicationFactory's
    // IAsyncDisposable.DisposeAsync returns ValueTask — bridge via explicit implementation.
    async Task IAsyncLifetime.DisposeAsync() => await DisposeAsync();
}
