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
using Border = iText.Layout.Borders.Border;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Drawing;
using Rectangle = iText.Kernel.Geom.Rectangle;
using Image = iText.Layout.Element.Image;

namespace Mexty.MVVM.Model {
    public class Reports {

        private static readonly ILog Log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly string _date = DateTime.Now.ToString("dd-MM-yy");
        private readonly string _dateNow = DateTime.Now.ToString("G");

        private readonly int _fontSize = 12;

        private string sucursal = "";
        private string direccion = "";
        private readonly string _mainPath = @"C:\Mexty\Reportes\";
        private readonly string _inventarioPath = @"C:\Mexty\Reportes\Inventario\";
        private readonly string _SucursalesInventarioPath = @"C:\Mexty\Reportes\Sucursales\";
        private readonly string usuarioActivo = DatabaseInit.GetUsername();
        private int idImprimir = 0;
        private string dirImprimir = null;
        private string nombreImprimir = null;

        private Font consola = new("Courier New", 8);
        PrintDocument pd = new();

        public Reports() {
            Directory.CreateDirectory(_mainPath);
            Directory.CreateDirectory(_inventarioPath);
            Directory.CreateDirectory(_SucursalesInventarioPath);
        }
        private Document CreateDocument(string nombreDocumento, Paragraph metaData, string titulo, string path) {

            Cell cell;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream($"{path}{nombreDocumento}.pdf", FileMode.Create, FileAccess.Write)));
            Document document = new Document(pdfDocument, PageSize.A4);
            document.SetMargins(20, 20, 20, 20);


            Table header = new Table(2)
                .UseAllAvailableWidth()
                .SetFixedLayout();

            // Load image from disk
            ImageData imageData = ImageDataFactory.Create(@"C:\Mexty\Brand\LogoReportes.png");
            // Create layout image object and provide parameters. Page number = 1
            Image image = new Image(imageData).ScaleAbsolute(200, 60);
            image.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT);
            // This adds the image to the page

            cell = new();
            cell.Add(image).SetBorder(Border.NO_BORDER);
            header.AddCell(cell);

            metaData
               .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.RIGHT)
               .SetTextAlignment(TextAlignment.RIGHT)
               .SetFontSize(_fontSize);

            cell = new();
            cell.SetBorder(Border.NO_BORDER);
            cell.Add(metaData);
            header.AddCell(cell);

            document.Add(header);

            document.Add(new Paragraph(titulo)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(_fontSize));

            return document;
        }

        public void ReporteInventario() {
            var dataInventario = QuerysInventario.GetItemsFromInventario();

            int idTienda = DatabaseInit.GetIdTienda();

            var ListaSucursales = QuerysSucursales.GetTablesFromSucursales();

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

            string[] columnas = { "ID", "Tipo de Producto", "Nombre", "Cantidad" };
            float[] tamaños = { 1, 3, 3, 1 };

            Table table = new Table(UnitValue.CreatePercentArray(tamaños));
            table.SetWidth(UnitValue.CreatePercentValue(100));

            foreach (string columa in columnas) {
                table.AddHeaderCell(new Cell().Add(new Paragraph(columa).SetTextAlignment(TextAlignment.CENTER).SetFontSize(_fontSize)));
            }

            foreach (var item in dataInventario) {
                table.AddCell(new Cell().Add(new Paragraph(item.IdProducto.ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                table.AddCell(new Cell().Add(new Paragraph(item.TipoProducto.ToString()).SetFontSize(_fontSize)));
                table.AddCell(new Cell().Add(new Paragraph(item.NombreProducto.ToString()).SetFontSize(_fontSize)));
                table.AddCell(new Cell().Add(new Paragraph(item.Cantidad.ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
            }

            document.Add(table);
            document.Close();

            var msg = $"Se ha creado {nombreReporte}.pdf en la ruta {path}";
            MessageBox.Show(msg, "Reporte Creado");

            Log.Debug("Reporte de inventario creado");

            pd.PrinterSettings.PrinterName = "EC-PM-5890X";
            pd.PrintPage += new PrintPageEventHandler(this.ReporteInventarioImprimir);
            pd.Print();
        }

        public void ReportXSucursal(int idTienda, string nombreTienda, string direccion) {
            idImprimir = idTienda;
            nombreImprimir = nombreTienda;
            dirImprimir = direccion;
            var dataInventarioSucursal = QuerysInventario.GetItemsFromInventarioById(idTienda);

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
                table.AddHeaderCell(new Cell().Add(new Paragraph(columa).SetTextAlignment(TextAlignment.CENTER).SetFontSize(_fontSize)));
            }

            foreach (var item in dataInventarioSucursal) {
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

            pd.PrinterSettings.PrinterName = "EC-PM-5890X";
            pd.PrintPage += new PrintPageEventHandler(this.ReporteInventarioSucursalImprimir);
            pd.Print();

        }

        private void ReporteInventarioImprimir(object sender, PrintPageEventArgs ppeArgs) {
            var dataInventario = QuerysInventario.GetItemsFromInventario();

            System.Drawing.Image image = System.Drawing.Image.FromFile(@"C:\Mexty\Brand\LogoTicket.png");
            System.Drawing.Point ulCorner = new System.Drawing.Point(0, 0);

            Graphics g = ppeArgs.Graphics;
            float yPos = 70;
            int count = 0;
            //Read margins from PrintPageEventArgs  
            float leftMargin = 0;
            string type = null;
            string name = null;
            int renglon = 18;

            g.DrawImage(image, ulCorner);

            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos);
            renglon += 18;
            g.DrawString("   REPORTE DE INVENTARIO   ", consola, Brushes.Black, leftMargin, yPos + renglon - 9);
            renglon += 18;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos + renglon - 9);
            renglon += 18;
            g.DrawString($"Sucursal: {sucursal.ToUpper()} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 18;
            g.DrawString($"Dir: {direccion.ToUpper()} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 18;
            g.DrawString($"Usuario: {usuarioActivo} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 18;
            g.DrawString($"Fecha: {_dateNow} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 18;

            g.DrawString("ID Tipo      Nombre    Pzas", consola, Brushes.Black, leftMargin, yPos + renglon + 2);
            renglon += 18;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos + renglon - 8);
            float topMargin = 75 + renglon;
            foreach (var item in dataInventario) {
                type = null;
                name = null;
                if (item.TipoProducto == "Paleta Agua" || item.TipoProducto == "Paleta Leche" || item.TipoProducto == "Paleta Fruta") {
                    type = item.TipoProducto[..9];
                }
                else type = item.TipoProducto;

                name = item.NombreProducto.Length >= 10 ? item.NombreProducto[..10] : item.NombreProducto;
                yPos = topMargin + (count * consola.GetHeight(g));
                g.DrawString(string.Format("{0,2} {1,-9} {2,-10} {3,3}", item.IdProducto, type, name, item.Cantidad), consola, Brushes.Black, leftMargin, yPos);
                count++;
            }
        }

        private void ReporteInventarioSucursalImprimir(object sender, PrintPageEventArgs ppeArgs) {
            var dataInventarioSucursal = QuerysInventario.GetItemsFromInventarioById(idImprimir);

            string path = $"{_SucursalesInventarioPath}";
            string nombreReporte = $"Reporte-{nombreImprimir}-{_date}";
            string tituloReporte = $"Reporte de Inventario de {nombreImprimir}";

            System.Drawing.Image image = System.Drawing.Image.FromFile(@"C:\Mexty\Brand\LogoTicket.png");
            System.Drawing.Point ulCorner = new System.Drawing.Point(0, 0);

            Graphics g = ppeArgs.Graphics;
            float yPos = 70;
            int count = 0;
            //Read margins from PrintPageEventArgs  
            float leftMargin = 0;
            string type = null;
            string name = null;
            int renglon = 18;

            g.DrawImage(image, ulCorner);

            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos);
            renglon += 18;
            g.DrawString("   REPORTE DE INVENTARIO   ", consola, Brushes.Black, leftMargin, yPos + renglon - 9);
            renglon += 18;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos + renglon - 9);
            renglon += 18;
            g.DrawString($"Sucursal: {sucursal.ToUpper()} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 18;
            g.DrawString($"Dir: {dirImprimir.ToUpper()} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 18;
            g.DrawString($"Usuario: {usuarioActivo} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 18;
            g.DrawString($"Fecha: {_dateNow} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 18;

            g.DrawString("ID Tipo      Nombre    Pzas", consola, Brushes.Black, leftMargin, yPos + renglon + 2);
            renglon += 18;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos + renglon - 8);
            float topMargin = 75 + renglon;
            foreach (var item in dataInventarioSucursal) {
                type = null;
                name = null;
                if (item.TipoProducto == "Paleta Agua" || item.TipoProducto == "Paleta Leche" || item.TipoProducto == "Paleta Fruta") {
                    type = item.TipoProducto[..9];
                }
                else type = item.TipoProducto;

                name = item.NombreProducto.Length >= 10 ? item.NombreProducto[..10] : item.NombreProducto;
                yPos = topMargin + (count * consola.GetHeight(g));
                g.DrawString(string.Format("{0,2} {1,-9} {2,-10} {3,3}", item.IdProducto, type, name, item.Cantidad), consola, Brushes.Black, leftMargin, yPos);
                count++;
            }
        }
    }
}
