namespace TicketFlow.Domain.Entities;

public sealed class Evento
{
    private Evento()
    {
    }

    public Evento(
        string nome,
        string descricao,
        DateTime dataInicio,
        DateTime dataFim,
        string local,
        int capacidadeTotal,
        decimal precoIngresso,
        DateTime criadoEm)
    {
        ValidarDados(nome, local, dataInicio, dataFim, capacidadeTotal, precoIngresso);

        Id = Guid.CreateVersion7();
        Nome = nome;
        Descricao = descricao;
        DataInicio = dataInicio;
        DataFim = dataFim;
        Local = local;
        CapacidadeTotal = capacidadeTotal;
        IngressosVendidos = 0;
        PrecoIngresso = precoIngresso;
        Ativo = true;
        CriadoEm = criadoEm;
    }

    public Guid Id { get; private set; }
    public string Nome { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public DateTime DataInicio { get; private set; }
    public DateTime DataFim { get; private set; }
    public string Local { get; private set; } = null!;
    public int CapacidadeTotal { get; private set; }
    public int IngressosVendidos { get; private set; }
    public decimal PrecoIngresso { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime CriadoEm { get; private set; }

    public void AtualizarDados(
        string nome,
        string descricao,
        DateTime dataInicio,
        DateTime dataFim,
        string local,
        int capacidadeTotal,
        decimal precoIngresso,
        bool ativo)
    {
        ValidarDados(nome, local, dataInicio, dataFim, capacidadeTotal, precoIngresso);

        if (capacidadeTotal < IngressosVendidos)
        {
            throw new ArgumentException(
                "CapacidadeTotal não pode ser menor que a quantidade de ingressos já vendidos.",
                nameof(capacidadeTotal));
        }

        Nome = nome;
        Descricao = descricao;
        DataInicio = dataInicio;
        DataFim = dataFim;
        Local = local;
        CapacidadeTotal = capacidadeTotal;
        PrecoIngresso = precoIngresso;
        Ativo = ativo;
    }

    private static void ValidarDados(
        string nome,
        string local,
        DateTime dataInicio,
        DateTime dataFim,
        int capacidadeTotal,
        decimal precoIngresso)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(local);

        if (dataFim <= dataInicio)
        {
            throw new ArgumentException("DataFim deve ser posterior a DataInicio.", nameof(dataFim));
        }

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(capacidadeTotal, 0);
        ArgumentOutOfRangeException.ThrowIfNegative(precoIngresso);
    }
}
