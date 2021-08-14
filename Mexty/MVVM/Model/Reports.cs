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
using Mexty.MVVM.Model.DatabaseQuerys;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace Mexty.MVVM.Model {
    public class Reports {

        private static readonly ILog Log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly string _date = DateTime.Now.ToString("dd-MM-yy");
        private readonly string _dateNow = DateTime.Now.ToString("G");

        private string sucursal = "";
        private string direccion = "";
        private readonly string _mainPath = @"C:\Mexty\Reportes\";
        private readonly string _inventarioPath = @"C:\Mexty\Reportes\Inventario\";
        private readonly string _SucursalesInventarioPath = @"C:\Mexty\Reportes\Sucursales\";
        private readonly string usuarioActivo = DatabaseInit.GetUsername();

        public Reports() {
            Directory.CreateDirectory(_mainPath);
            Directory.CreateDirectory(_inventarioPath);
            Directory.CreateDirectory(_SucursalesInventarioPath);
        }
        private Document CreateDocument(string nombreDocumento, Paragraph metaData, string titulo, string path) {
            
            Cell cell;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream($"{path}{nombreDocumento}.pdf", FileMode.Create, FileAccess.Write)));
            Document document = new Document(pdfDocument, PageSize.A7);
            document.SetMargins(5, 5, 5, 5);
            

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

            metaData
               .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.RIGHT)
               .SetTextAlignment(TextAlignment.RIGHT)
               .SetFontSize(6);

            cell = new();
            cell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
            cell.Add(metaData);
            header.AddCell(cell);

            document.Add(header);

            document.Add(new Paragraph(titulo)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(8));

            return document;
        }

        public void ReporteInventario() {
            int idTienda = DatabaseInit.GetIdTienda();

            var ListaSucursales = QuerysSucursales.GetTablesFromSucursales();
            var data = QuerysInventario.GetItemsFromInventario();

            foreach (Sucursal tienda in ListaSucursales) {
                if (tienda.IdTienda == idTienda) {
                    sucursal = tienda.NombreTienda;
                    direccion = tienda.Dirección;
                }
            }

            string path = $"{_inventarioPath}";
            string nombreReporte = $"Reporte-{sucursal}-{_date}";
            string tituloReporte = $"Reporte de Inventario de {sucursal}";

            var texto = $"{sucursal} - {direccion} \n" + $"{_dateNow} \n" + $"{usuarioActivo}";

            Paragraph p = new Paragraph().Add(texto);
            

            var document = CreateDocument(nombreReporte, p, tituloReporte, path);

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
                table.AddCell(new Cell().Add(new Paragraph(item.Cantidad.ToString()).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER)));
            }

            document.Add(table);
            document.Close();

            var msg = $"Se ha creado {nombreReporte}.pdf en la ruta {path}";
            MessageBox.Show(msg, "Reporte Creado");

            Log.Debug("Reporte de inventario creado");
        }

        public void ReportXSucursal(int idTienda, string nombreTienda, string direccion) {
            var data = QuerysInventario.GetItemsFromInventarioById(idTienda);
            

            string path = $"{_SucursalesInventarioPath}";
            string nombreReporte = $"Reporte-{nombreTienda}-{_date}";
            string tituloReporte = $"Reporte de Inventario de {nombreTienda}";
            string texto = $"{nombreTienda} - {direccion} \n" + $"{_dateNow} \n" + $"{usuarioActivo}";
            Paragraph p = new Paragraph().Add(texto);

            var document = CreateDocument(nombreReporte, p, tituloReporte, path);

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
                table.AddCell(new Cell().Add(new Paragraph(item.Cantidad.ToString()).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER)));
            }

            document.Add(table);
            document.Close();

            var msg = $"Se ha creado {nombreReporte}.pdf en la ruta {path}";
            MessageBox.Show(msg, "Reporte de sucursal Creado");

            Log.Debug("Reporte de inventario por sucursal creado");

        }
    }
}
