using iText.Layout.Element;
using iText.Layout.Properties;
using log4net;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace Mexty.MVVM.Model {
    public class ReportesInventario : BaseReportes {

        private static readonly ILog Log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public void ReporteInventario() {
            var dataInventario = QuerysInventario.GetItemsFromInventario();

            int idTienda = DatabaseInit.GetIdTiendaIni();

            var ListaSucursales = QuerysSucursales.GetTablesFromSucursales();

            foreach (Sucursal tienda in ListaSucursales) {
                if (tienda.IdTienda == idTienda) {
                    sucursal = tienda.NombreTienda;
                    direccion = tienda.Dirección;
                }
            }

            string path = $"{_inventarioPath}";
            string nombreReporte = @$"ReporteInventario-{sucursal}-{_date}";
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

            try {
                pd.PrinterSettings.PrinterName = "EC-PM-5890X";
                var aber = pd.PrinterSettings.PaperSizes;
                pd.PrintPage += new PrintPageEventHandler(this.ImprimirReporteInventarioXSucursal);
                pd.Print();
                Log.Debug("Reporte de inventario impreso en ticket");

            }
            catch (System.Exception e) {
                Log.Debug("Impresora de ticket no encontrada");
                Log.Error(e.Message);
            }
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

            try {
                pd.PrinterSettings.PrinterName = "EC-PM-5890X";
                pd.PrintPage += new PrintPageEventHandler(this.ImprimirReporteInventarioXSucursal);
                pd.Print();
                Log.Debug("Reporte de inventario impreso en ticket");

            }
            catch (System.Exception e) {
                Log.Debug("Impresora de ticket no encontrada");
                Log.Error(e.Message);
            }
        }

        private void ImprimirReporteInventario(object sender, PrintPageEventArgs ppeArgs) {
            Log.Debug("Iniciando impresión del ticket de reporte de inventario...");
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
            Log.Debug("Finalizando impresión del ticket de reporte de inventario.");

        }

        private void ImprimirReporteInventarioXSucursal(object sender, PrintPageEventArgs ppeArgs) {
            Log.Debug("Iniciando impresión del ticket de reporte de inventario por sucursal...");

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
            Log.Debug("Finalizando impresión del ticket de reporte de inventario por sucursal.");
        }
    }
}