using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace EM.DOMAIN.Servicos.Relatorio
{
	public class HeaderEvent : PdfPageEventHelper
	{
		public override void OnStartPage(PdfWriter writer, Document document)
		{

			base.OnStartPage(writer, document);
			// layout do titulo
			PdfPTable layoutTable = new PdfPTable(new float[] { 3, 6 });
			layoutTable.WidthPercentage = 100;
			layoutTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			layoutTable.DefaultCell.Border = Rectangle.NO_BORDER;

			//logotipo do titulo
			string logoPath = "https://www.grupocope.com.br/imgs/245/215/images/cope-421.png";
			Image logo = Image.GetInstance(logoPath);
			logo.ScaleToFit(100, 100);
			PdfPCell logoCell = new PdfPCell(logo);
			logoCell.Border = Rectangle.NO_BORDER;

			layoutTable.AddCell(logoCell);

			//texto do título
			Font titleFont = FontFactory.GetFont("Arial", 24, Font.BOLD);
			Paragraph title = new Paragraph("Relatório de Alunos", titleFont);


			PdfPCell titleCell = new PdfPCell();
			titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
			titleCell.VerticalAlignment = Element.ALIGN_MIDDLE;
			titleCell.Border = Rectangle.NO_BORDER;
			titleCell.AddElement(title);

			layoutTable.AddCell(titleCell);

			document.Add(layoutTable);


			PdfGState gs = new()
			{
				FillOpacity = 0.3f
			};
			writer.DirectContentUnder.SetGState(gs);
			Image backgroundImage = Image.GetInstance("C:\\Users\\Escolar Manager\\Downloads\\SolucaoEM-main\\SolucaoEM-main\\EM.WEB\\wwwroot\\images\\escolar_manager_logo (3).png");
			backgroundImage.SetAbsolutePosition(0, 120);
			backgroundImage.ScaleToFit(document.PageSize.Width, document.PageSize.Height);

			// Adiciona a imagem de fundo
			PdfContentByte canvas = writer.DirectContentUnder;
			canvas.AddImage(backgroundImage);

		}
	}
}
