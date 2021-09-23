using iText.Barcodes;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mexty.MVVM.Model.DatabaseQuerys;
using System.Windows;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using HorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;

namespace Mexty.MVVM.Model {
    class BarCodes {
        private readonly string _mainPath = @"C:\Mexty\CodigoBarras\";

        //private static readonly string[] _tipoProductoText = { "Paleta Agua", "Paleta Leche", "Paleta Fruta", "Helado", "Agua", "Extras", "Otros" };

        public BarCodes() {
            Directory.CreateDirectory(_mainPath);
            ManipulatePdf(_mainPath);
            MessageBox.Show($"Se ha generado el pdf de los códigos de barras en la ruta {_mainPath}");
        }

        private void ManipulatePdf(string dest) {
            Paragraph titulo;
            Paragraph p;
            int i;
            Table table;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream($"{dest}Codigo-Productos.pdf", FileMode.Create, FileAccess.Write)));

            Document doc = new Document(pdfDoc, PageSize.A4);
            table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth().SetTextAlignment(TextAlignment.CENTER);

            titulo = new Paragraph().Add("Códigos de Productos").SetTextAlignment(TextAlignment.CENTER);
            doc.Add(titulo);

            var data = QuerysProductos.GetTablesFromProductos(true);

            titulo = new Paragraph().Add("Paleta Agua").SetTextAlignment(TextAlignment.CENTER);
            doc.Add(titulo);
            foreach (var item in data) {
                if (item.TipoProducto == "Paleta Agua") {
                    p = new Paragraph().Add($"{item.NombreProducto}");
                    i = item.IdProducto;
                    table.AddCell(CreateBarcode(string.Format("{0:d8}", i), pdfDoc, p)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                }
                else continue;
            }
            doc.Add(table);

            table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth().SetTextAlignment(TextAlignment.CENTER);
            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            titulo = new Paragraph().Add("Paleta Leche").SetTextAlignment(TextAlignment.CENTER);
            doc.Add(titulo);
            foreach (var item in data) {
                if (item.TipoProducto == "Paleta Leche") {
                    p = new Paragraph().Add($"{item.NombreProducto}");
                    i = item.IdProducto;
                    table.AddCell(CreateBarcode(string.Format("{0:d8}", i), pdfDoc, p)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                }
                else continue;
            }
            doc.Add(table);

            table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth().SetTextAlignment(TextAlignment.CENTER);
            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            titulo = new Paragraph().Add("Paleta Fruta").SetTextAlignment(TextAlignment.CENTER);
            doc.Add(titulo);
            foreach (var item in data) {
                if (item.TipoProducto == "Paleta Fruta") {
                    p = new Paragraph().Add($"{item.NombreProducto}");
                    i = item.IdProducto;
                    table.AddCell(CreateBarcode(string.Format("{0:d8}", i), pdfDoc, p)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                }
                else continue;
            }
            doc.Add(table);

            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth().SetTextAlignment(TextAlignment.CENTER);
            titulo = new Paragraph().Add("Helado").SetTextAlignment(TextAlignment.CENTER);
            doc.Add(titulo);
            foreach (var item in data) {
                if (item.TipoProducto == "Helado") {
                    p = new Paragraph().Add($"{item.NombreProducto}");
                    i = item.IdProducto;
                    table.AddCell(CreateBarcode(string.Format("{0:d8}", i), pdfDoc, p)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                }
                else continue;
            }
            doc.Add(table);

            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth().SetTextAlignment(TextAlignment.CENTER);
            titulo = new Paragraph().Add("Agua").SetTextAlignment(TextAlignment.CENTER);
            doc.Add(titulo);
            foreach (var item in data) {
                if (item.TipoProducto == "Agua") {
                    p = new Paragraph().Add($"{item.NombreProducto}");
                    i = item.IdProducto;
                    table.AddCell(CreateBarcode(string.Format("{0:d8}", i), pdfDoc, p)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                }
                else continue;
            }
            doc.Add(table);

            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth().SetTextAlignment(TextAlignment.CENTER);
            titulo = new Paragraph().Add("Extras").SetTextAlignment(TextAlignment.CENTER);
            doc.Add(titulo);
            foreach (var item in data) {
                if (item.TipoProducto == "Extras") {
                    p = new Paragraph().Add($"{item.NombreProducto}");
                    i = item.IdProducto;
                    table.AddCell(CreateBarcode(string.Format("{0:d8}", i), pdfDoc, p)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                }
                else continue;
            }
            doc.Add(table);

            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth().SetTextAlignment(TextAlignment.CENTER);
            titulo = new Paragraph().Add("Otros").SetTextAlignment(TextAlignment.CENTER);
            doc.Add(titulo);
            foreach (var item in data) {
                if (item.TipoProducto == "Otros") {
                    p = new Paragraph().Add($"{item.NombreProducto}");
                    i = item.IdProducto;
                    table.AddCell(CreateBarcode(string.Format("{0:d8}", i), pdfDoc, p)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                }
                else continue;
            }
            doc.Add(table);


            doc.Close();
        }

        private static Cell CreateBarcode(string code, PdfDocument pdfDoc, Paragraph nombre) {
            nombre.SetFontSize(10);

            Barcode39 barcode = new Barcode39(pdfDoc);

            barcode.SetCode(code);

            // Create barcode object to put it to the cell as image
            PdfFormXObject barcodeObject = barcode.CreateFormXObject(null, null, pdfDoc);
            Cell cell = new Cell().Add(new Image(barcodeObject));
            cell.Add(nombre);
            cell.SetPaddingTop(10);
            cell.SetPaddingRight(10);
            cell.SetPaddingBottom(10);
            cell.SetPaddingLeft(10);

            return cell;


        }


    }
}
