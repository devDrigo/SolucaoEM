using EM.WEB.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EM.DOMAIN;
using EM.REPOSITORY;
using System.Reflection;
using EM.DOMAIN.Servicos.Relatorio;


namespace EM.WEB.Controllers
{
    public class AlunoController : Controller
	{
		private readonly IRepositorioAluno<Aluno> repositorioAluno;
		private readonly IRepositorioCidade<CidadeModel> repositorioCidade;
		private readonly GeradorRelatorioAluno geradorRelatorio;
		private readonly IWebHostEnvironment _env;

		public AlunoController(IRepositorioAluno<Aluno> repositorioAluno, IRepositorioCidade<CidadeModel> repositorioCidade, GeradorRelatorioAluno geradorRelatorio, IWebHostEnvironment env)
		{
			this.repositorioAluno = repositorioAluno;
			this.repositorioCidade = repositorioCidade;
			this.geradorRelatorio = geradorRelatorio;
			_env = env;
		}


		public IActionResult GerarRelatorio()
		{
			// Obtenha a lista de alunos. Você pode obter isso do seu repositório.
			List<Aluno> alunos = repositorioAluno.GetAll().ToList();

			// Gere o relatório e obtenha o PDF como um array de bytes.
			// Aqui estamos passando null para CidadeId e Sexo, já que não temos esses valores neste contexto.
			byte[] pdfBytes = geradorRelatorio.GerarRelatorio(alunos, null, null, null);

			// Retorne o array de bytes como um FileContentResult.
			return File(pdfBytes, "application/pdf");
		}


		[HttpPost]
		public IActionResult GerarRelatorio(int? Id_cidade, int? Sexo, string? Ordem)
		{
			// Obtenha a lista de alunos do seu repositório e converta para IQueryable
			IQueryable<Aluno> alunos = repositorioAluno.GetAll().AsQueryable();

			// Converta a consulta para uma lista para execução
			List<Aluno> alunosList = alunos.ToList();

			// Gere o relatório e obtenha o PDF como um array de bytes.
			byte[] pdfBytes = geradorRelatorio.GerarRelatorio(alunosList, Id_cidade, Sexo, Ordem);

			// Retorne o array de bytes como um FileContentResult.
			return File(pdfBytes, "application/pdf");
		}



		public IActionResult TabelaAluno()
		{
			IEnumerable<Aluno> listaAlunos = repositorioAluno.GetAll();
			return View(listaAlunos);
		}


		public IActionResult CadastroAluno(long? matricula)
		{
			ViewBag.Cidades = repositorioCidade.GetAll().ToList();

			if (matricula != null)
			{
				var aluno = repositorioAluno.Get(a => a.Matricula == matricula).FirstOrDefault();
				if (aluno == null)
				{
					return NotFound();
				}
				ViewBag.IsEdicao = true;
				return View(aluno);
			}

			ViewBag.IsEdicao = false;
			return View(new Aluno());
		}


		[HttpPost]
		public IActionResult CadastroAluno(Aluno aluno)
		{
			if (ModelState.IsValid)
			{
				if (aluno.Matricula > 0)
				{
					repositorioAluno.Update(aluno);
				}
				else
				{
					repositorioAluno.Add(aluno);
				}
				return RedirectToAction("TabelaAluno");
			}

			ViewBag.IsEdicao = aluno.Matricula > 0;
			ViewBag.Cidades = repositorioCidade.GetAll().ToList();
			return View(aluno);
		}




		[HttpPost]
		public ActionResult Search(string searchTerm, string searchType)
		{
			Console.WriteLine("Search Type: " + searchType); // Adicione esta linha para depuração
			IEnumerable<Aluno> alunos;
			if (searchType == "matricula" && long.TryParse(searchTerm, out long matricula))
			{
				alunos = new List<Aluno>(repositorioAluno.GetByMatricula(matricula));
			}
			else if (searchType == "nome")
			{
				alunos = repositorioAluno.GetByContendoNoNome(searchTerm);
			}
			else
			{
				alunos = new List<Aluno>();
			}
			return View("TabelaAluno", alunos);
		}


		[HttpPost]
		public IActionResult RemoveAluno(Aluno aluno)
		{
			repositorioAluno.Remove(aluno);
			return RedirectToAction("Index", "Home");
		}
		
		public IActionResult RelatorioAluno()
		{
            ViewBag.Cidades = repositorioCidade.GetAll().ToList();
            return View();
		}




		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
