using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace EM.DOMAIN.Servicos.Relatorio
{
	public class DefaultEvent : PdfPageEventHelper
	{
		public override void OnStartPage(PdfWriter writer, Document document)
		{

			base.OnStartPage(writer, document);
			// layout do titulo
			PdfPTable TitleTable = new PdfPTable(new float[] { 3, 7, 3 });
			TitleTable.WidthPercentage = 107;
			TitleTable.DefaultCell.Border = Rectangle.NO_BORDER;

			//logotipo do titulo
			string logoPath = "https://www.grupocope.com.br/imgs/245/215/images/cope-421.png";
			Image logo = Image.GetInstance(logoPath);
			logo.ScaleToFit(100, 100);
			PdfPCell logoCell = new PdfPCell(logo);
			logoCell.Border = Rectangle.NO_BORDER;
			logoCell.HorizontalAlignment = Element.ALIGN_CENTER;
			logoCell.BorderWidthBottom = 1f;
			TitleTable.AddCell(logoCell);

			//texto do título
			Font titleFont = FontFactory.GetFont("Arial", 24, Font.BOLD);
			PdfPCell textCell = new PdfPCell(new Phrase("Relatório de Alunos", titleFont));
			textCell.Padding = 0;
			textCell.VerticalAlignment = Element.ALIGN_MIDDLE;
			textCell.HorizontalAlignment = Element.ALIGN_CENTER;
			textCell.Border = Rectangle.NO_BORDER;
			textCell.BorderWidthBottom = 1f;
			TitleTable.AddCell(textCell);

			//celula em branco por questões de alinhamento
			PdfPCell blanckCell = new PdfPCell();
			blanckCell.Border = Rectangle.NO_BORDER;
			blanckCell.BorderWidthBottom = 1f;
			TitleTable.AddCell(blanckCell);


			TitleTable.SpacingAfter = 10;
			document.Add(TitleTable);


			//marca d'agua
			PdfGState marcaDagua = new()
			{
				FillOpacity = 0.3f
			};
			writer.DirectContentUnder.SetGState(marcaDagua);
			Image backgroundImage = Image.GetInstance(".\\wwwroot\\images\\escolar_manager_logo (3).png");

			//dimensiona a imagem
			backgroundImage.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
			backgroundImage.ScalePercent(80);

			// posiciona a imagem
			float xPosition = (document.PageSize.Width - backgroundImage.ScaledWidth) / 2;
			float yPosition = (document.PageSize.Height - backgroundImage.ScaledHeight) / 2- 50;
			backgroundImage.SetAbsolutePosition(xPosition, yPosition);

			// Adiciona a imagem de fundo
			PdfContentByte canvas = writer.DirectContentUnder;
			canvas.AddImage(backgroundImage);

		}

		public override void OnEndPage(PdfWriter writer, Document document)
		{
			base.OnEndPage(writer, document);

			//Rodapé
			BaseColor corFonte = new(0, 0, 0);
			BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
			Font fonteFooter = new(bf);
			PdfPTable footer = new PdfPTable(1);
			footer.TotalWidth = document.PageSize.Width;
			footer.DefaultCell.Border = PdfPCell.NO_BORDER;

			// texto centralizado no rodapé
			PdfPCell dateCell = new PdfPCell(new Phrase($"Data de Emissão: {DateTime.Now.ToString("dd/MM/yyyy")} ", fonteFooter));
			dateCell.HorizontalAlignment = Element.ALIGN_CENTER;
			dateCell.VerticalAlignment = Element.ALIGN_BOTTOM;
			dateCell.Border = PdfPCell.NO_BORDER;
			footer.AddCell(dateCell);

			// Posição relativa do rodapé
			float footerPosition = document.BottomMargin;
			footer.WriteSelectedRows(0, -1, 0, footerPosition, writer.DirectContent);

		}
	}
}