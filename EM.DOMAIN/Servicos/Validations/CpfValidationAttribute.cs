using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EM.DOMAIN.Servicos.Validations
{
    public class CpfValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string cpf = value.ToString();

            // Use o método CPFValidado para validar o CPF
            if (!Validation.CPFValidado(cpf))
            {
                return new ValidationResult("CPF inválido.");
            }

            return ValidationResult.Success;
        }
    }
}
