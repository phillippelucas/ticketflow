# ADR-001: Usar xUnit v2 em vez de xUnit v3 para os projetos de teste

## Status

Accepted

## Context

O `CLAUDE.md` do projeto e a skill `testing` do dotnet-claude-kit documentam xUnit v3 como padrão para novos projetos .NET 10, incluindo o exemplo de `IAsyncLifetime` com `WebApplicationFactory` (a `ValueTask InitializeAsync/DisposeAsync` de v3 evita conflito de assinatura com o `IAsyncDisposable.DisposeAsync()` que `WebApplicationFactory<T>` já implementa).

Os projetos `TicketFlow.UnitTests` e `TicketFlow.IntegrationTests` foram gerados pelo scaffold inicial (`dotnet new xunit`) com o pacote clássico `xunit` 2.9.3, não `xunit.v3`. Ao preparar a Fase 2 (testes unitários e de integração), avaliamos migrar para v3 antes de escrever os testes novos.

Alternativas consideradas:

1. **Migrar para xUnit v3** (`xunit.v3` + Microsoft Testing Platform). O SDK .NET 10 instalado localmente (10.0.400) não possui o template `xunit3` — `dotnet new list xunit` só retorna o template clássico, que continua gerando `xunit` 2.9.3. A documentação oficial de xUnit v3 (`xunit.net/docs/getting-started/v3`) descreve um projeto baseado em `OutputType=Exe` e no pacote `xunit.v3.mtp-v2` (Microsoft Testing Platform v2), atualmente só disponível em versão preview (`4.0.0-pre.108`), com mudanças de runner (`dotnet test` passa a rodar via Microsoft.Testing.Platform em vez de VSTest) que afetam diretamente os passos de CI (coleta de cobertura via `--collect:"XPlat Code Coverage"`, geração de `.trx`).
2. **Permanecer em xUnit v2** (pacote `xunit` 2.9.3, já instalado e testado no scaffold), usando um workaround conhecido para conciliar `IAsyncLifetime` (v2) com `WebApplicationFactory<T>`.

## Decision

Permanecemos em **xUnit v2** (`xunit` 2.9.3 + `xunit.runner.visualstudio` 3.1.4) para `TicketFlow.UnitTests` e `TicketFlow.IntegrationTests`.

Para conciliar `IAsyncLifetime` de xUnit v2 (`Task InitializeAsync()` / `Task DisposeAsync()`) com `WebApplicationFactory<Program>` (que já implementa `IAsyncDisposable.DisposeAsync() : ValueTask`), a fixture de integração usa implementação explícita de interface:

```csharp
public class ApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    public async Task InitializeAsync() { /* ... */ }

    // Override público que WebApplicationFactory já expõe (ValueTask)
    public override async ValueTask DisposeAsync() { /* ... */ }

    // Implementação explícita satisfaz IAsyncLifetime.DisposeAsync (Task) delegando
    // para o override público acima — evita conflito de assinatura entre as duas interfaces.
    async Task IAsyncLifetime.DisposeAsync() => await DisposeAsync();
}
```

Quando migrar para v3 no futuro: revisitar quando o template `xunit3` (ou o pacote `xunit.v3` com Microsoft Testing Platform) estiver estável e disponível localmente sem depender de pacotes preview, e quando o impacto no pipeline de CI (mudança de runner) puder ser validado isoladamente antes de aplicar aos projetos de teste existentes.

## Consequences

### Positive

- Zero risco de regressão: pacote já usado pelo scaffold original, sem dependências preview.
- `dotnet test` continua funcionando via VSTest normalmente, sem mudanças no runner nem no workflow de CI (`ci.yml`).
- Compatibilidade imediata com `Microsoft.AspNetCore.Mvc.Testing`, `coverlet.collector` e `ReportGenerator`, todos validados com o runner VSTest clássico.

### Negative

- Diverge do que `CLAUDE.md` e a skill `testing` documentam como padrão (xUnit v3).
- `IAsyncLifetime.DisposeAsync()` em `ApiFixture` exige o workaround de implementação explícita de interface — código um pouco menos direto do que o padrão v3 documentado na skill.
- Futuras contribuições que sigam a skill `testing` ao pé da letra podem tentar usar a sintaxe de `IAsyncLifetime` de v3 (`ValueTask` direto) e precisar ajustar para o padrão v2 usado aqui.

### Mitigations

- Este ADR documenta o motivo do desvio, para que a skill `testing` (que assume v3) não seja seguida cegamente neste repositório sem checar este documento antes.
- O workaround de `ApiFixture` está comentado inline explicando o porquê, para reduzir a chance de alguém "corrigir" a assinatura e quebrar a compilação.
- Reavaliar esta decisão quando o pacote `xunit.v3` (ou equivalente) atingir uma versão estável sem dependência do Microsoft Testing Platform em preview, abrindo uma issue/ADR de superseding neste momento.
