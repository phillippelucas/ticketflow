# ADR-002: Fixar FluentAssertions em 7.2.2 devido à mudança de licenciamento na v8

## Status

Accepted

## Context

Ao adicionar testes unitários para `EventoService` na Fase 2, o pedido original era usar **xUnit + FluentAssertions + NSubstitute**. Ao consultar o NuGet pela versão mais recente do pacote `FluentAssertions`, a versão estável mais alta disponível era **8.10.0**.

A partir da versão 8.0 (lançada em janeiro de 2025), o FluentAssertions passou a ser licenciado comercialmente pela Xceed para uso não-OSS/não-pessoal: uso comercial acima de determinados limiares exige uma licença paga. As versões **7.x e anteriores permanecem sob a licença Apache 2.0**, livres para qualquer uso, para sempre — não há retroatividade da mudança de licença sobre versões já publicadas.

TicketFlow é um projeto com finalidade comercial (não uma biblioteca open source), então adicionar `FluentAssertions` 8.x sem avaliação criaria exposição de licenciamento não intencional: uma dependência de teste (frequentemente instalada sem revisão detalhada de licença) passaria a exigir contrato comercial para uso contínuo em produção/CI.

Alternativas consideradas:

1. **Usar FluentAssertions 8.x** (última versão) — introduz obrigação de licença comercial paga sem que isso tenha sido uma decisão explícita do time.
2. **Fixar em FluentAssertions 7.2.2** (última versão sob Apache 2.0) — mantém a API e a experiência de asserções fluentes pedida, sem custo de licenciamento.
3. **Trocar de biblioteca** (ex.: Shouldly, ou `Assert` nativo do xUnit) — evita o problema de licença por completo, mas diverge do que foi pedido explicitamente e exigiria reescrever os testes já criados.

## Decision

O pacote `FluentAssertions` é fixado em **7.2.2** (`<PackageReference Include="FluentAssertions" Version="7.2.2" />`) em `TicketFlow.UnitTests.csproj`, a última versão publicada sob licença Apache 2.0.

Nenhum outro projeto do repositório referencia `FluentAssertions` além de `TicketFlow.UnitTests`. Os testes de integração (`TicketFlow.IntegrationTests`) usam `Assert` nativo do xUnit, evitando ampliar a superfície de dependência dessa biblioteca.

Antes de qualquer atualização futura de `FluentAssertions` (manual ou via ferramenta de atualização de dependências), verificar explicitamente se a versão alvo é `8.0.0` ou superior — nesse caso, a atualização exige avaliação de licenciamento (compra de licença Xceed ou permanência em 7.x) antes de ser aceita.

## Consequences

### Positive

- Zero exposição a licenciamento comercial não avaliado.
- API de asserções idêntica à esperada (`.Should().Be(...)`, `.Should().ContainKey(...)`, etc.) — nenhuma reescrita de teste foi necessária.
- Decisão documentada e localizada em um único `PackageReference`, fácil de auditar.

### Negative

- Ficamos presos a uma versão de 2024/2025 do FluentAssertions, sem receber correções de bugs ou novos recursos lançados em 8.x.
- Ferramentas automáticas de atualização de dependências (Dependabot, `dotnet outdated`, etc.) vão sinalizar 7.2.2 como desatualizado indefinidamente, exigindo triagem manual recorrente para não aceitar a atualização por engano.

### Mitigations

- Este ADR serve como justificativa documentada para ignorar alertas de "pacote desatualizado" especificamente para `FluentAssertions` — a skill `outdated` do dotnet-claude-kit já trata FluentAssertions como uma das "commercial-license traps" conhecidas e deve ser consultada antes de qualquer bump de versão.
- Se o projeto decidir adquirir a licença comercial da Xceed no futuro, revisar este ADR e marcá-lo como `Superseded by ADR-NNN` ao migrar para 8.x.
- Alternativamente, se o custo de manter a dependência pinada crescer, considerar a migração para uma biblioteca de asserções sem custo de licença (ex.: Shouldly) como um ADR futuro.
