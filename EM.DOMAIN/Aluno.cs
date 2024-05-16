using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.DOMAIN.Servicos.Validations;


namespace EM.DOMAIN
{
	public class Aluno : IEntidade
	{
		public long Matricula { get; set; }

		[Required(ErrorMessage = "preencha este campo")]
		[StringLength(100, ErrorMessage = "Nome Deve ter no máximo 100 caracteres!")]
		[MinLength(3, ErrorMessage = "Nome Deve ter no mínimo 3 caracteres!")]
		public string Nome { get; set; } = string.Empty;
		[Required(ErrorMessage = "preencha este campo")]
		public SexoEnum Sexo { get; set; }

		[DataNascimentoValidation]
		public DateTime DataNascimento { get; set; }
		[CpfValidation]
		public string? CPF { get; set; }

		[Required(ErrorMessage = "preencha este campo")]
		public CidadeModel Cidade { get; set; } = new CidadeModel();
	}
}