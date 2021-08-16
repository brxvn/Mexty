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

namespace Mexty.MVVM.Model {
    class BarCodes {
        private readonly string _mainPath = @"C:\Mexty\CodigoBarras\";
        public BarCodes() {
            Directory.CreateDirectory(_mainPath);
            ManipulatePdf(_mainPath);
        }

        private void ManipulatePdf(string dest) {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream($"{dest}Codigo-Productos.pdf", FileMode.Create, FileAccess.Write)));

            Document doc = new Document(pdfDoc, PageSize.A4);
            Table table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth().SetTextAlignment(TextAlignment.CENTER);

            Paragraph titulo = new Paragraph().Add("Codigos de Productos").SetTextAlignment(TextAlignment.CENTER);
            doc.Add(titulo);

            var data = QuerysProductos.GetTablesFromProductos();

            foreach (var item in data) {
                Paragraph p = new Paragraph().Add($"{item.TipoProducto} {item.NombreProducto}");
                var i = item.IdProducto;
                table.AddCell(CreateBarcode(string.Format("{0:d8}", i), pdfDoc, p)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                if ((string.Format("{0:d8}", i) == i.ToString())) {

                }
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
