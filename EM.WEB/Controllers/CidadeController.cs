using Microsoft.AspNetCore.Mvc;
using EM.DOMAIN;
using EM.REPOSITORY;

namespace EM.WEB.Controllers
{
    public class CidadeController : Controller
    {
        private readonly IRepositorioCidade<CidadeModel> repositorioCidade;

        public CidadeController(IRepositorioCidade<CidadeModel> repositorioCidade)
        {
            this.repositorioCidade = repositorioCidade;
        }


        public IActionResult TabelaCidade()
        {
            IEnumerable<CidadeModel> listaCidade = repositorioCidade.GetAll(); ;
            return View(listaCidade);
        }

        public IActionResult CadastroCidade(int? id)
        {
            if (id != null)
            {
                var cidade = repositorioCidade.Get(c => c.Id_cidade == id).FirstOrDefault();
                if (cidade == null)
                {
                    return NotFound();
                }

                ViewBag.IsEdicao = true;
                return View(cidade);
            }
            ViewBag.IsEdicao = false;
            return View(new CidadeModel());
        }

        [HttpPost]
        public IActionResult CadastroCidade(CidadeModel cidade)
        {
            if (ModelState.IsValid)
            {
                if (cidade.Id_cidade > 0)
                {
                    repositorioCidade.Update(cidade);
                }
                else
                {
                    repositorioCidade.Add(cidade);
                }
                return RedirectToAction("TabelaCidade");
            }
            return View(cidade);
        }

        [HttpPost]
        public IActionResult RemoveCidade(CidadeModel cidade)
        {
            repositorioCidade.Remove(cidade);
            return RedirectToAction("TabelaCidade");
        }

    }
}