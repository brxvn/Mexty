using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using log4net;
using Mexty.MVVM.Model.DatabaseQuerys;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using Border = iText.Layout.Borders.Border;
using Image = iText.Layout.Element.Image;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace Mexty.MVVM.Model {
    public class BaseReportes {
        private static readonly ILog Log =
    LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        protected readonly string _date = DateTime.Now.ToString("d'-'MM'-'y");
        protected readonly string _dateNow = DateTime.Now.ToString("G");

        protected readonly int _fontSize = 12;

        protected string sucursal = "";
        protected string direccion = "";
        protected readonly string _mainPath = @"C:\Mexty\Reportes\";
        protected readonly string _inventarioPath = @"C:\Mexty\Reportes\Inventario\";
        protected readonly string _SucursalesInventarioPath = @"C:\Mexty\Reportes\Sucursales\";
        protected readonly string _sucursalesVentas = @"C:\Mexty\Ventas\Sucursales\";
        protected readonly string _ventasUsuarios = @"C:\Mexty\Ventas\Usuarios\";
        protected readonly string usuarioActivo = DatabaseInit.GetUsername();
        protected int idImprimir = 0;
        protected string dirImprimir = null;
        protected string nombreImprimir = null;

        protected Font consola = new("Courier New", 8, FontStyle.Bold);
        protected PrintDocument pd = new();
        private Document document;

        public BaseReportes() {
            Directory.CreateDirectory(_mainPath);
            Directory.CreateDirectory(_inventarioPath);
            Directory.CreateDirectory(_SucursalesInventarioPath);
            Directory.CreateDirectory($"{_sucursalesVentas}");
        }
        protected Document CreateDocument(string nombreDocumento, Paragraph metaData, string titulo, string path) {
            Cell cell;
            try {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream($"{path}{nombreDocumento}.pdf", FileMode.Create, FileAccess.Write)));
                document = new Document(pdfDocument, PageSize.A4);
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
                document.Add(new Paragraph(titulo).SetTextAlignment(TextAlignment.CENTER).SetFontSize(_fontSize));
                Log.Debug("Documento PDF para reportes creado.");

            }
            catch (Exception e) {
                Log.Debug("Error al crear documento pdf");
                Log.Error(e.Message);
            }

            return document;
        }
    }
}
