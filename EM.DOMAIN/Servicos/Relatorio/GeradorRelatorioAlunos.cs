using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EM.DOMAIN.Servicos.Relatorio
{
    public class GeradorRelatorioAluno
    {
        public byte[] GerarRelatorio(List<Aluno> alunos, int? Id_cidade, int? Sexo, string? Ordem, bool? linhasZebradas, bool horizontal)
        {

            switch (Ordem)
            {
                case "Nome":
                    alunos = alunos.OrderBy(a => a.Nome).ToList();
                    break;
                case "DataNascimento":
                    alunos = alunos.OrderBy(a => a.DataNascimento).ToList();
                    break;
                case "Cidade":
                    alunos = alunos.OrderBy(a => a.Cidade.Nome).ToList();
                    break;
                case "UF":
                    alunos = alunos.OrderBy(a => a.Cidade.UF).ToList();
                    break;
            }


            using (MemoryStream ms = new MemoryStream())
            {


				Document document = horizontal ? document = new(PageSize.A4.Rotate(), 25, 25, 25, 30) : document = new(PageSize.A4, 25, 25, 25, 30);
				PdfWriter writer = PdfWriter.GetInstance(document, ms);
				writer.PageEvent = new HeaderEvent();
				document.Open();

				


				PdfPTable table = new PdfPTable(new float[] { 7, 10, 3, 7, 5, 6, 2 }) { WidthPercentage = 105 };
				table.DefaultCell.FixedHeight = 30;
				table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				table.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;


				// filtra e mostra os filtros utilizados dentro de um retangulo com borda arredondado(se algum filtro for utilizado)
				PdfContentByte cb = writer.DirectContent;
				cb.SetRGBColorStroke(0, 0, 0);
				cb.SetLineWidth(1f);

				float larguraRetangulo = 140, alturaRetangulo = 80, margemDireita = 20, margemSuperior = 20;
				float posicaoX = document.PageSize.Width - larguraRetangulo - margemDireita;
				float posicaoY = document.PageSize.Height - alturaRetangulo - margemSuperior;

				if (Id_cidade.HasValue || Sexo.HasValue)
				{
					cb.RoundRectangle(posicaoX, posicaoY, larguraRetangulo, alturaRetangulo, 10);
					cb.Stroke();

					cb.BeginText();
					cb.SetRGBColorFill(0, 0, 0);
					cb.SetFontAndSize(BaseFont.CreateFont(), 10f);

					float posX = posicaoX + (larguraRetangulo - cb.GetEffectiveStringWidth("Filtros utilizados:", false)) / 3;
					float posY = posicaoY;
					cb.ShowTextAligned(Element.ALIGN_LEFT, "Filtros utilizados:", posX, posicaoY + alturaRetangulo - 5 - 15, 0);

					if (Id_cidade.HasValue)
					{
						alunos = alunos.Where(a => a.Cidade!.Id_cidade == Id_cidade).ToList();
						string filterCidade = $"• Cidade: ID-{Id_cidade}";
						cb.ShowTextAligned(Element.ALIGN_LEFT, filterCidade, posX, posicaoY + alturaRetangulo - 10 - 10 - 20, 0);
						posY -= 15;
					}
					if (Sexo.HasValue)
					{
						alunos = alunos.Where(a => (int)a.Sexo! == Sexo.Value).ToList();
						string filterSexo = $"• Sexo: {(Sexo.Value == 1 ? "Masculino" : "Feminino")}";
						cb.ShowTextAligned(Element.ALIGN_LEFT, filterSexo, posX, posY + alturaRetangulo - 10 - 10 - 20, 0);
						posY -= 15;
					}

					cb.EndText();
				}


				// Função para criar células do cabeçalho do jeito que quero
				PdfPCell CreateHeaderCell(string text) =>
					new PdfPCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE)))
					{
                        HorizontalAlignment = Element.ALIGN_CENTER,
				        BackgroundColor = new BaseColor(0, 100, 0)

					};

				// Adicione as células do cabeçalho à tabela
				table.AddCell(CreateHeaderCell("Matrícula"));
				table.AddCell(CreateHeaderCell("Nome"));
				table.AddCell(CreateHeaderCell("Sexo"));
				table.AddCell(CreateHeaderCell("CPF"));
				table.AddCell(CreateHeaderCell("Idade"));
				table.AddCell(CreateHeaderCell("Cidade"));
				table.AddCell(CreateHeaderCell("UF"));



				//conteúdo da tabela
				int rowIndex = 0;
				foreach (Aluno aluno in alunos)
				{
					if (linhasZebradas.HasValue && linhasZebradas.Value && rowIndex % 2 == 1)
					{
						// Altere a cor de fundo para cinza claro para linhas ímpares
						table.DefaultCell.BackgroundColor = new BaseColor(0, 0, 0,40);
					}
					else
					{
						// Mantenha a cor de fundo branca para linhas pares
						table.DefaultCell.BackgroundColor = null;
					}

					table.AddCell(aluno.Matricula.ToString());
					table.AddCell(aluno.Nome);
					table.AddCell(aluno.Sexo.ToString());
					table.AddCell(aluno.CPF);
					table.AddCell(CalcularIdade(aluno.DataNascimento));
					table.AddCell(aluno.Cidade!.Nome);
					table.AddCell(aluno.Cidade.UF);

					rowIndex++;
				}


				table.SpacingBefore = 10;
				table.HeaderRows = 1;
				document.Add(table);
				document.Close();
                return ms.ToArray();

			}
        }

        // Método para calcular a idade baseado na data de nascimento
        private string CalcularIdade(DateTime? dataNascimento)
        {
            if (!dataNascimento.HasValue)
            {
                return "----";
            }

            DateTime agora = DateTime.Now;
            int anos = agora.Year - dataNascimento.Value.Year;
            int meses = agora.Month - dataNascimento.Value.Month;
            int dias = agora.Day - dataNascimento.Value.Day;

            if (dias < 0)
            {
                meses--;
                dias += DateTime.DaysInMonth(agora.Year, agora.Month);
            }

            if (meses < 0)
            {
                anos--;
                meses += 12;
            }

            return $"{anos}a {meses}m {dias}d";
        }
    }
}