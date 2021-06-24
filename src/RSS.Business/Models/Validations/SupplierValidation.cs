using FluentValidation;
using RSS.Business.Models.Enums;
using RSS.Business.Utils;

namespace RSS.Business.Models.Validations
{
    public class SupplierValidation : AbstractValidator<Supplier>
    {
        public SupplierValidation()
        {
            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(2, 100)
                .WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");
            When(s => s.SupplierType == SupplierType.PessoaFisica, () => 
            {
                RuleFor(s => s.IdentificationDocument.Length).Equal(CpfValidation._CPF_LENGTH)
                    .WithMessage("O campo Documento precisa ter {ComparisonValue} caracteres e foi fornecido {PropertyValue}");
                RuleFor(s => CpfValidation.Validate(s.IdentificationDocument)).Equal(true)
                    .WithMessage("O documento fornecido é inválido");
            });

            When(s => s.SupplierType == SupplierType.PessoaJuridica, () =>
            {
                RuleFor(s => s.IdentificationDocument.Length).Equal(CnpjValidation._CNPJ_LENGTH)
                   .WithMessage("O campo Documento precisa ter {ComparisonValue} caracteres e foi fornecido {PropertyValue}");
                RuleFor(s => CnpjValidation.Validate(s.IdentificationDocument)).Equal(true)
                    .WithMessage("O documento fornecido é inválido");
            });
        }
    }
}
