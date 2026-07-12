
using System;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using TestApiProj.Models;
using TestApiProj.Services;
namespace TestApiProj.DataAccess
{
    public class PdfInvoiceGenerator: IInvoice
    {
        public async Task<FileStreamResult> GenerateInvoicePdf(Invoice invoice)
        {
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                // Create a PdfWriter instance
                PdfWriter writer = new PdfWriter(memoryStream);
                writer.SetCloseStream(false); // Prevent the writer from closing the stream

                // Create a PdfDocument instance
                PdfDocument pdfDoc = new PdfDocument(writer);

                // Create a Document instance to add content
                Document document = new Document(pdfDoc);

                // Create fonts
                PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // Add a title to the document
                document.Add(new Paragraph($"{invoice.CustomerName} Invoice")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(24)
                    .SetFont(boldFont));

                // Add a line separator
                document.Add(new LineSeparator(new SolidLine()));

                // Add customer details
                document.Add(new Paragraph($"Customer Name: {invoice.CustomerName}")
                    .SetFontSize(12)
                    .SetFont(boldFont));
                document.Add(new Paragraph($"Customer Email: {invoice.CustomerEmail}")
                    .SetFontSize(12)
                    .SetFont(regularFont));

                // Add a line separator
                document.Add(new LineSeparator(new SolidLine()));

                // Add item details in a table
                Table table = new Table(5);
                table.AddCell(new Cell().Add(new Paragraph("Item").SetFont(boldFont)));
                table.AddCell(new Cell().Add(new Paragraph("Price").SetFont(boldFont)));
                table.AddCell(new Cell().Add(new Paragraph("Quantity").SetFont(boldFont)));
                table.AddCell(new Cell().Add(new Paragraph("Discount").SetFont(boldFont)));
                table.AddCell(new Cell().Add(new Paragraph("Tax").SetFont(boldFont)));

                table.AddCell(new Cell().Add(new Paragraph(invoice.ItemName).SetFont(regularFont)));
                table.AddCell(new Cell().Add(new Paragraph($"${invoice.ItemPrice}").SetFont(regularFont)));
                table.AddCell(new Cell().Add(new Paragraph(invoice.Quantity.ToString()).SetFont(regularFont)));
                table.AddCell(new Cell().Add(new Paragraph($"{invoice.Discount}%").SetFont(regularFont)));
                table.AddCell(new Cell().Add(new Paragraph($"{invoice.Itemtax}%").SetFont(regularFont)));

                document.Add(table);

                // Calculate and display the total amount
                decimal subtotal = invoice.ItemPrice * invoice.Quantity;
                decimal discountAmount = subtotal * (invoice.Discount / 100);
                decimal finalTotal = subtotal - discountAmount;
                decimal TaxAmount = finalTotal * (invoice.Itemtax / 100);
                finalTotal = TaxAmount + finalTotal;

                document.Add(new Paragraph($"Subtotal: ${subtotal}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(12)
                    .SetFont(regularFont));
                document.Add(new Paragraph($"Discount: -${discountAmount}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(12)
                    .SetFont(regularFont));
                document.Add(new Paragraph($"TaxGST: -${TaxAmount}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(12)
                    .SetFont(regularFont));
                document.Add(new Paragraph($"Total Amount: ${finalTotal}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(12)
                    .SetFont(boldFont));

                // Close the document
                document.Close();

                // Flush the memory stream
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin); // Move to the beginning of the stream

                // Return the PDF file as a FileStreamResult
                return new FileStreamResult(memoryStream, "application/pdf")
                {
                    FileDownloadName = $"{invoice.CustomerName} Invoice.pdf" // The filename for the download
                };
            }
            catch (Exception ex)
            {
                memoryStream.Dispose();
                throw new Exception("Error generating PDF invoice", ex);
            }
        }
    }
}
