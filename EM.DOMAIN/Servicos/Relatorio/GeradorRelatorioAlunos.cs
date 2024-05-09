using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EM.DOMAIN.Servicos.Relatorio
{
    public class GeradorRelatorioAluno
    {
        public byte[] GerarRelatorio(List<Aluno> alunos, int? Id_cidade, int? Sexo, string? Ordem)
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
				Document document = new Document(PageSize.A4, 25, 25, 25, 30);
				PdfWriter writer = PdfWriter.GetInstance(document, ms);
				writer.PageEvent = new HeaderEvent();
				document.Open();

                
                //linha de divisão
                Chunk linebreak = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 107f, BaseColor.BLACK, Element.ALIGN_CENTER, -1));

                document.Add(linebreak);

                // filtra e mostra os filtros utilizados(se utilizado)
                if (Id_cidade.HasValue || Sexo.HasValue)
                {
                    Font filterFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);
                    document.Add(new Paragraph($"Filtros utilizados:"));
                    Paragraph filtros = new Paragraph($"Filtros utilizados:");
                    if (Id_cidade.HasValue)
                    {
						alunos = alunos.Where(a => a.Cidade!.Id_cidade == Id_cidade).ToList();
						Paragraph filterCidade = new Paragraph($"• Cidade: ID-{Id_cidade}");
                        filterCidade.Alignment = Element.ALIGN_LEFT;
                        document.Add(filterCidade);
                    }
                    if (Sexo.HasValue)
                    {
						alunos = alunos.Where(a => (int)a.Sexo! == Sexo.Value).ToList();
						Paragraph filterSexo = new Paragraph($"• Sexo: {(Sexo.Value == 1 ? "Masculino" : "Feminino")}", filterFont);
                        filterSexo.Alignment = Element.ALIGN_LEFT;
                        document.Add(filterSexo);
                    }
                }

                document.Add(new Paragraph("\n"));

				PdfPTable table = new PdfPTable(new float[] { 7, 10, 3, 6, 5, 6, 2 }) { WidthPercentage = 105 };
				table.DefaultCell.FixedHeight = 30;
				table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				table.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

				// Função para criar células do cabeçalho com fonte branca
				PdfPCell CreateHeaderCell(string text) =>
					new PdfPCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE)))
					{
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
				foreach (Aluno aluno in alunos)
                {
                    table.AddCell(aluno.Matricula.ToString());
                    table.AddCell(aluno.Nome);
                    table.AddCell(aluno.Sexo.ToString());
                    table.AddCell(aluno.CPF);
                    table.AddCell(CalcularIdade(aluno.DataNascimento));
                    table.AddCell(aluno.Cidade!.Nome);
                    table.AddCell(aluno.Cidade.UF);
                }

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