using Common.Logging;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Mexty.MVVM.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Mexty.MVVM.Model {
    public class Reports {

        private static readonly ILog Log =
           LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private string _date = DateTime.UtcNow.ToString("dd-MM-yy");

        public void PDFReportInventario() {

            var ListaSucursales = Database.GetTablesFromSucursales();

            var nombres = ListaSucursales[0].NombreTienda;

            var data = Database.GetItemsFromInventario();

            // TODO
            Directory.CreateDirectory(@"C:\Mexty\Reportes\");
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream($@"C:\Mexty\Reportes\Reporte-{_date}.pdf", FileMode.Create, FileAccess.Write)));
            Document document = new Document(pdfDocument, PageSize.A7);
            document.SetMargins(5,5,5,5);
            // Load image from disk
            ImageData imageData = ImageDataFactory.Create(@"C:\Mexty\Brand\LogoReportes.png");
            // Create layout image object and provide parameters. Page number = 1
            Image image = new Image(imageData).ScaleAbsolute(100, 30);
            image.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
            // This adds the image to the page
            document.Add(image);

            string titulo = "Reporte de inventario de Sucursal";

            document.Add(new Paragraph(titulo).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(8));


            string[] columnas = { "ID", "Tipo de Producto", "Nombre", "Piezas", "Cantidad" };
            float[] tamaños = { 1, 3, 3, 2, 2 };

            Table table = new Table(UnitValue.CreatePercentArray(tamaños));
            table.SetWidth(UnitValue.CreatePercentValue(100));

            foreach (string columa in columnas) {
                table.AddHeaderCell(new Cell().Add(new Paragraph(columa).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(8)));
            }

            foreach (var item in data) {
                table.AddCell(new Cell().Add(new Paragraph(item.IdProducto.ToString()).SetFontSize(8)));
                table.AddCell(new Cell().Add(new Paragraph(item.TipoProducto.ToString()).SetFontSize(8)));
                table.AddCell(new Cell().Add(new Paragraph(item.NombreProducto.ToString()).SetFontSize(8)));
                table.AddCell(new Cell().Add(new Paragraph(item.Piezas.ToString()).SetFontSize(8)));
                table.AddCell(new Cell().Add(new Paragraph(item.Cantidad.ToString()).SetFontSize(8)));
            }

            document.Add(table);
            document.Close();

            var msg = $@"Se ha creado Reporte-{_date}.pdf en la ruta C:\Mexty\Reportes\";
            MessageBox.Show(msg, "Reporte Creado");

            Log.Debug("Reporte de inventario creado");


        }
    }
}
