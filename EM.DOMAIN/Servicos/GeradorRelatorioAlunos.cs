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
		public string GerarRelatorio(List<Aluno> alunos, int? Id_cidade, int? Sexo)
		{
			string pdfPath = "RelatorioAlunos.pdf";
			// Filtra a lista de alunos com base nos parâmetros do formulário
			if (Id_cidade.HasValue)
			{
				alunos = alunos.Where(a => a.Cidade.Id_cidade == Id_cidade).ToList();
			}
			if (Sexo.HasValue)
			{
				alunos = alunos.Where(a => (int)a.Sexo == Sexo.Value).ToList();
			}


			using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
			{
				Document document = new Document();
				PdfWriter writer = PdfWriter.GetInstance(document, fs);
				document.Open();

				BaseColor grey = new BaseColor(220, 220, 220);

				// Cria um retângulo cinza que cobre toda a página
				PdfContentByte canvas = writer.DirectContentUnder;
				canvas.SaveState();
				canvas.SetColorFill(grey);
				canvas.Rectangle(0, 0, document.PageSize.Width, document.PageSize.Height);
				canvas.Fill();
				canvas.RestoreState();

				// Cria uma tabela para o layout
				PdfPTable layoutTable = new PdfPTable(new float[] { 3, 6 });
				layoutTable.WidthPercentage = 100;

				// Adiciona o logotipo
				string logoPath = "https://is4-ssl.mzstatic.com/image/thumb/Purple112/v4/33/93/a7/3393a763-f5c2-5b3e-2f11-8f3d70663e5c/AppIcon-0-1x_U007emarketing-0-7-0-0-85-220.png/512x512bb.jpg";
				Image logo = Image.GetInstance(logoPath);
				logo.ScaleToFit(75, 75);  // Ajusta o tamanho do logotipo
				PdfPCell logoCell = new PdfPCell(logo);
				logoCell.Border = Rectangle.NO_BORDER;
				layoutTable.AddCell(logoCell);

				// Adiciona o título
				Font titleFont = FontFactory.GetFont("Arial", 24, Font.BOLD);
				Paragraph title = new Paragraph("Relatório de Alunos", titleFont);
				title.Alignment = Element.ALIGN_LEFT;
				PdfPCell titleCell = new PdfPCell();
				titleCell.AddElement(title);
				titleCell.Border = Rectangle.NO_BORDER;
				layoutTable.AddCell(titleCell);

				document.Add(layoutTable);


				// Adiciona uma linha de divisão
				Chunk linebreak = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -1));
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
				// Adiciona algum espaço em branco
				document.Add(new Paragraph("\n"));

				// Cria uma tabela com as colunas necessárias
				PdfPTable table = new PdfPTable(new float[] { 7, 10, 4, 5, 5, 6, 2 });
				table.WidthPercentage = 110;

				// Adiciona os cabeçalhos da tabela
				PdfPCell cell = new PdfPCell(new Phrase("Matrícula"));
				cell.HorizontalAlignment = Element.ALIGN_CENTER;
				cell.VerticalAlignment = Element.ALIGN_MIDDLE;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("Nome"));
				cell.HorizontalAlignment = Element.ALIGN_CENTER;
				cell.VerticalAlignment = Element.ALIGN_MIDDLE;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("Sexo"));
				cell.HorizontalAlignment = Element.ALIGN_CENTER;
				cell.VerticalAlignment = Element.ALIGN_MIDDLE;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("Data de Nascimento"));
				cell.HorizontalAlignment = Element.ALIGN_CENTER;
				cell.VerticalAlignment = Element.ALIGN_MIDDLE;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("Idade"));
				cell.HorizontalAlignment = Element.ALIGN_CENTER;
				cell.VerticalAlignment = Element.ALIGN_MIDDLE;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("Cidade"));
				cell.HorizontalAlignment = Element.ALIGN_CENTER;
				cell.VerticalAlignment = Element.ALIGN_MIDDLE;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("UF"));
				cell.HorizontalAlignment = Element.ALIGN_CENTER;
				cell.VerticalAlignment = Element.ALIGN_MIDDLE;
				table.AddCell(cell);


				// Adiciona o conteúdo da tabela
				foreach (var aluno in alunos)
				{
					table.AddCell(aluno.Matricula.ToString());
					table.AddCell(aluno.Nome);
					table.AddCell(aluno.Sexo == SexoEnum.Masculino ? "Masculino" : "Feminino");
					table.AddCell(aluno.DataNascimento.HasValue ? aluno.DataNascimento.Value.ToString("dd/MM/yyyy") : "");
					table.AddCell(CalcularIdade(aluno.DataNascimento));
					table.AddCell(aluno.Cidade.Nome);
					table.AddCell(aluno.Cidade.UF);
				}

				document.Add(table);

				document.Close();
			}
			return pdfPath;
		}

		// Método para calcular a idade baseado na data de nascimento
		private string CalcularIdade(DateTime? dataNascimento)
		{
			if (!dataNascimento.HasValue)
			{
				return "Data de nascimento não fornecida";
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
