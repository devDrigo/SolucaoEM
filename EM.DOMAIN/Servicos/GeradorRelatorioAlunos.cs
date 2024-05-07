using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using EM.DOMAIN;

namespace EM.DOMAIN.Servicos
{
	public class GeradorRelatorioAluno
	{
		public byte[] GerarRelatorio(List<Aluno> alunos, int? Id_cidade, int? Sexo, string? Ordem)
		{
			// Filtra a lista de alunos com base nos parâmetros do formulário
			if (Id_cidade.HasValue)
			{
				alunos = alunos.Where(a => a.Cidade!.Id_cidade == Id_cidade).ToList();
			}
			if (Sexo.HasValue)
			{
				alunos = alunos.Where(a => (int)a.Sexo! == Sexo.Value).ToList();
			}

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
				Document document = new Document();
				PdfWriter writer = PdfWriter.GetInstance(document, ms);
				document.Open();

				BaseColor grey = new BaseColor(210, 210, 210);

				//retângulo cinza que uso como plano de fundo
				PdfContentByte canvas = writer.DirectContentUnder;
				canvas.SaveState();
				canvas.SetColorFill(grey);
				canvas.Rectangle(0, 0, document.PageSize.Width, document.PageSize.Height);
				canvas.Fill();
				canvas.RestoreState();

				// layout do cabeçalho
				PdfPTable layoutTable = new PdfPTable(new float[] { 3, 6 });
				layoutTable.WidthPercentage = 100;

				//logotipo
				string logoPath = "https://www.grupocope.com.br/imgs/245/215/images/cope-421.png";
				Image logo = Image.GetInstance(logoPath);
				logo.ScaleToFit(100, 100);
				PdfPCell logoCell = new PdfPCell(logo);
				logoCell.Border = Rectangle.NO_BORDER;
				layoutTable.AddCell(logoCell);

				//título
				Font titleFont = FontFactory.GetFont("Arial", 24, Font.BOLD);
				Paragraph title = new Paragraph("Relatório de Alunos", titleFont);
				title.Alignment = Element.ALIGN_LEFT;
				title.Alignment = Element.ALIGN_BOTTOM;
				title.SpacingAfter= 20;
				PdfPCell titleCell = new PdfPCell();
				titleCell.AddElement(title);
				titleCell.Border = Rectangle.NO_BORDER;
				layoutTable.AddCell(titleCell);

				document.Add(layoutTable);


				//linha de divisão
				Chunk linebreak = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 112f, BaseColor.BLACK, Element.ALIGN_CENTER, -1));

				document.Add(linebreak);

				// Adiciona os filtros utilizados se algum filtro foi usado
				if (Id_cidade.HasValue || Sexo.HasValue)
				{
					Font filterFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);
					document.Add(new Paragraph($"Filtros utilizados:"));
					Paragraph filtros = new Paragraph($"Filtros utilizados:");
					if (Id_cidade.HasValue)
					{
						Paragraph filterCidade = new Paragraph($"• Cidade: ID-{Id_cidade}");
						filterCidade.Alignment = Element.ALIGN_LEFT;
						document.Add(filterCidade);
					}
					if (Sexo.HasValue)
					{
						Paragraph filterSexo = new Paragraph($"• Sexo: {(Sexo.Value == 1 ? "Masculino" : "Feminino")}", filterFont);
						filterSexo.Alignment = Element.ALIGN_LEFT;
						document.Add(filterSexo);
					}
				}

				document.Add(new Paragraph("\n"));

				// PARÂMETROS DA TABELA
				PdfPTable table = new PdfPTable(new float[] { 7, 10, 4, 5, 5, 6, 2 });
				table.WidthPercentage = 110;
				table.WidthPercentage = 110;
				table.DefaultCell.FixedHeight = 30;
				table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				table.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

				//cabeçalho da tabela
				table.AddCell(new Phrase("Matrícula"));
				table.AddCell(new Phrase("Nome"));
				table.AddCell(new Phrase("Sexo"));
				table.AddCell(new Phrase("Data de Nascimento"));
				table.AddCell(new Phrase("Idade"));
				table.AddCell(new Phrase("Cidade"));
				table.AddCell(new Phrase("UF"));

			
				//conteúdo da tabela
				foreach (Aluno aluno in alunos)
				{
					table.AddCell(aluno.Matricula.ToString());
					table.AddCell(aluno.Nome);
					table.AddCell(aluno.Sexo.ToString());
					table.AddCell(aluno.DataNascimento.HasValue ? aluno.DataNascimento.Value.ToString("dd/MM/yyyy") : "");
					table.AddCell(CalcularIdade(aluno.DataNascimento));
					table.AddCell(aluno.Cidade!.Nome);
					table.AddCell(aluno.Cidade.UF);
				}

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