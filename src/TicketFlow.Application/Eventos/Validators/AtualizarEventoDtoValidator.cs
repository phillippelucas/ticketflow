using FluentValidation;
using TicketFlow.Application.Eventos.Dtos;

namespace TicketFlow.Application.Eventos.Validators;

public sealed class AtualizarEventoDtoValidator : AbstractValidator<AtualizarEventoDto>
{
    public AtualizarEventoDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.CapacidadeTotal)
            .GreaterThan(0).WithMessage("CapacidadeTotal deve ser maior que zero.");

        RuleFor(x => x.PrecoIngresso)
            .GreaterThanOrEqualTo(0).WithMessage("PrecoIngresso não pode ser negativo.");

        RuleFor(x => x.DataInicio)
            .LessThan(x => x.DataFim).WithMessage("DataInicio deve ser anterior a DataFim.");
    }
}
