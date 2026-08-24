namespace TicketFlow.Domain.Entities;

public sealed class Lote
{
    private Lote()
    {
    }

    public Lote(
        Guid eventoId,
        string nome,
        int quantidade,
        decimal preco,
        DateTime dataInicio,
        DateTime dataFim)
    {
        if (eventoId == Guid.Empty)
        {
            throw new ArgumentException("EventoId não pode ser vazio.", nameof(eventoId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(quantidade, 0);
        ArgumentOutOfRangeException.ThrowIfNegative(preco);

        if (dataFim <= dataInicio)
        {
            throw new ArgumentException("DataFim deve ser posterior a DataInicio.", nameof(dataFim));
        }

        Id = Guid.CreateVersion7();
        EventoId = eventoId;
        Nome = nome;
        Quantidade = quantidade;
        Vendidos = 0;
        Preco = preco;
        DataInicio = dataInicio;
        DataFim = dataFim;
    }

    public Guid Id { get; private set; }
    public Guid EventoId { get; private set; }
    public string Nome { get; private set; } = null!;
    public int Quantidade { get; private set; }
    public int Vendidos { get; private set; }
    public decimal Preco { get; private set; }
    public DateTime DataInicio { get; private set; }
    public DateTime DataFim { get; private set; }
}
