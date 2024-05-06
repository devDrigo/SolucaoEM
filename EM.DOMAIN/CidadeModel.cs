using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EM.DOMAIN
{
	public class CidadeModel : IEntidade
	{
		public int Id_cidade { get; set; }

		public string? Nome { get; set; }

		public string? UF { get; set; }
	}
}