using Common.Logging;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Font;
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
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace Mexty.MVVM.Model {
    public class Reports {

        private static readonly ILog Log =
           LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly string _date = DateTime.Now.ToString("dd-MM-yy");
        private readonly string _dateNow = DateTime.Now.ToString("G");

        string nombre = "";
        string direccion = "";
        int idTienda = Database.GetIdTienda();
        string usuarioActivo = Database.GetUsername();
        List<Sucursal> ListaSucursales = Database.GetTablesFromSucursales();
        List<ItemInventario> data = Database.GetItemsFromInventario();

        public void PDFReportInventario() {
            Cell cell;
            
            foreach (Sucursal tienda in ListaSucursales) {
                if (tienda.IdTienda == idTienda) {
                    nombre = tienda.NombreTienda;
                    direccion = tienda.Dirección;
                }
            }

            var nombreReporte = $"Reporte-{nombre}-{_date}";
            
            Directory.CreateDirectory(@"C:\Mexty\Reportes\");
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream($@"C:\Mexty\Reportes\{nombreReporte}.pdf", FileMode.Create, FileAccess.Write)));
            Document document = new Document(pdfDocument, PageSize.A7);
            document.SetMargins(5,5,5,5);

            Table header = new Table(2)
                .UseAllAvailableWidth()
                .SetFixedLayout();

            // Load image from disk
            ImageData imageData = ImageDataFactory.Create(@"C:\Mexty\Brand\LogoReportes.png");
            // Create layout image object and provide parameters. Page number = 1
            Image image = new Image(imageData).ScaleAbsolute(100, 30);
            image.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT);
            // This adds the image to the page

            cell = new();
            cell.Add(image).SetBorder(iText.Layout.Borders.Border.NO_BORDER);
            header.AddCell(cell);

            cell = new();
            cell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);

            var texto = $"{nombre} - {direccion} \n" +
                $"{_dateNow} \n" +
                $"{usuarioActivo}";
            
            Paragraph p = new Paragraph()
                .Add(texto)
                .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.RIGHT)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFontSize(6);

            cell.Add(p);
            header.AddCell(cell);

            document.Add(header);

            string titulo = $"Reporte de Inventario de {nombre}";

            document.Add(new Paragraph(titulo)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(8));


            string[] columnas = { "ID", "Tipo de Producto", "Nombre", "Piezas", "Cantidad" };
            float[] tamaños = { 1, 3, 3, 1, 1 };

            Table table = new Table(UnitValue.CreatePercentArray(tamaños));
            table.SetWidth(UnitValue.CreatePercentValue(100));

            foreach (string columa in columnas) {
                table.AddHeaderCell(new Cell().Add(new Paragraph(columa).SetTextAlignment(TextAlignment.CENTER).SetFontSize(8)));
            }

            foreach (var item in data) {
                table.AddCell(new Cell().Add(new Paragraph(item.IdProducto.ToString()).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER)));
                table.AddCell(new Cell().Add(new Paragraph(item.TipoProducto.ToString()).SetFontSize(8)));
                table.AddCell(new Cell().Add(new Paragraph(item.NombreProducto.ToString()).SetFontSize(8)));
                table.AddCell(new Cell().Add(new Paragraph(item.Piezas.ToString()).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER)));
                table.AddCell(new Cell().Add(new Paragraph(item.Cantidad.ToString()).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER)));
            }

            document.Add(table);
            document.Close();

            var msg = $@"Se ha creado {nombreReporte}.pdf en la ruta C:\Mexty\Reportes\";
            MessageBox.Show(msg, "Reporte Creado");

            Log.Debug("Reporte de inventario creado");
        }
    }
}
