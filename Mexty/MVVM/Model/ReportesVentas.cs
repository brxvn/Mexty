using FluentDate;
using iText.Layout.Element;
using iText.Layout.Properties;
using log4net;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Windows;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace Mexty.MVVM.Model {
    class ReportesVentas : BaseReportes {
        private static readonly ILog Log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private static string[] columnas = { "Nombre Producto", "Piezas Vendidas", "Precio", "Total Venta" };
        private static float[] tamaños = { 1, 1, 1, 1 };


        List<ItemInventario> itemInventarios = new();
        List<ItemInventario> ventaTotal = new();
        private int idTienda;
        private string comandoSucursal;
        private string comandoUsuario;
        private string username;

        private Font consola1 = new("Courier New", 8, System.Drawing.FontStyle.Bold);


        List<Producto> productos = QuerysProductos.GetTablesFromProductos();
        List<Sucursal> sucursales = QuerysSucursales.GetTablesFromSucursales();

        public void ReporteVentasSucursal(int idTienda, string comando) {
            var data = QuerysReportesVentas.GetVentasPorSucursal(idTienda, comando);
            if (data.Count == 0) {
                MessageBox.Show("No hay nada que mostrar.\nPrimero realiza ventas.");
                return;
            }
            this.idTienda = idTienda;
            this.comandoSucursal = comando;
            Table table = new Table(UnitValue.CreatePercentArray(tamaños));

            table.SetWidth(UnitValue.CreatePercentValue(100));

            foreach (Sucursal tienda in sucursales) {
                if (tienda.IdTienda == idTienda) {
                    sucursal = tienda.NombreTienda;
                    direccion = tienda.Dirección;

                    break;
                }
            }

            string path = $"{_sucursalesVentas}{sucursal}\\{comando}\\";
            Directory.CreateDirectory($"{path}");
            string nombreReporte = $"ReporteVentas-{sucursal}-{comando}-{_date}";
            string tituloReporte = $"Reporte de Ventas de {comando} {sucursal}";

            var texto = $"{sucursal} - {direccion} \n" + $"{_dateNow} \n" + $"{usuarioActivo}";

            Paragraph p = new Paragraph().Add(texto);

            var document = CreateDocument(nombreReporte, p, tituloReporte, path);

            foreach (string columa in columnas) {
                table.AddHeaderCell(new Cell().Add(new Paragraph(columa).SetTextAlignment(TextAlignment.CENTER).SetFontSize(_fontSize)));
            }

            var totalProductos = 0;

            decimal totalDia = 0.0m;
            foreach (var detalle in data) {
                itemInventarios = Venta.StringProductosToList(detalle.DetalleVenta);
                foreach (var item in itemInventarios) {
                    var nombre = "";
                    var total = "";
                    var precioTotal = item.PrecioMenudeo != 0 ? item.PrecioMenudeo : item.PrecioMayoreo;
                    foreach (var producto in productos) {
                        if (item.IdProducto == producto.IdProducto) {
                            nombre = $"{producto.TipoProducto} {producto.NombreProducto}";
                            break;
                        }
                    }
                    table.AddCell(new Cell().Add(new Paragraph(nombre).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph(item.CantidadDependencias.ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph(precioTotal.ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph((precioTotal * item.CantidadDependencias).ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));

                    total = (precioTotal * item.CantidadDependencias).ToString();
                    totalProductos += item.CantidadDependencias;
                    totalDia += Convert.ToDecimal(total);
                }
            }
            document.Add(table);

            var texto1 = "";
            switch (comando) {
                case "hoy":
                    texto1 = $"Total de venta del dia {_date} es de: ${totalDia} con {totalProductos} productos vendidos.";
                    break;
                case "semana":
                    var ultimaSeana = (DateTime.Now - 1.Weeks()).ToString("dd-MM-yy");
                    texto1 = $"Total de venta de {ultimaSeana} a {_date} es de: ${totalDia} con {totalProductos} productos vendidos.";
                    break;
                case "mes":
                    var ultimoMes = (DateTime.Now - 1.Months()).ToString("dd-MM-yy");
                    texto1 = $"Total de venta de {ultimoMes} a {_date} es de: ${totalDia} con {totalProductos} productos vendidos.";
                    break;
            }

            Paragraph p1 = new Paragraph().Add(texto1);
            document.Add(p1);

            document.Close();

            var msg = $"Se ha creado {nombreReporte}.pdf en la ruta {path}";
            MessageBox.Show(msg, "Reporte de sucursal Creado");

            Log.Debug("Reporte de ventas por sucursal creado");

            try {
                PrintDocument pd = new();
                pd.PrinterSettings.PrinterName = "EC-PM-5890X";
                pd.PrintPage += new PrintPageEventHandler(this.ImprimirReporteVentas);
                pd.Print();
                Log.Debug("Reporte de ventas impreso en ticket");

            }
            catch (System.Exception e) {
                Log.Debug("Impresora de ticket no encontrada");
                Log.Error(e.Message);
            }
        }


        public void ReporteVentasUsuario(string username, string comando) {
            var data = QuerysReportesVentas.GetVentasPorUsuario(username, comando);
            if (data.Count == 0) {
                MessageBox.Show("No hay nada que mostrar. \nPrimero realiza ventas.");
                return;
            }
            this.username = username;
            this.comandoUsuario = comando;
            Table table = new Table(UnitValue.CreatePercentArray(tamaños));

            table.SetWidth(UnitValue.CreatePercentValue(100));

            string path = $"{_ventasUsuarios}{username}\\{comando}\\";
            Directory.CreateDirectory($"{path}");
            string nombreReporte = $"ReporteVentas-{username}-{comando}-{_date}";
            string tituloReporte = $"Reporte de Ventas de usuario: {username}";

            var texto = $"{sucursal} - {direccion} \n" + $"{_dateNow} \n" + $"{usuarioActivo}";

            Paragraph p = new Paragraph().Add(texto);

            var document = CreateDocument(nombreReporte, p, tituloReporte, path);

            foreach (string columa in columnas) {
                table.AddHeaderCell(new Cell().Add(new Paragraph(columa).SetTextAlignment(TextAlignment.CENTER).SetFontSize(_fontSize)));
            }


            var totalProductos = 0;

           decimal totalDia = 0.0m;
            foreach (var detalle in data) {
                itemInventarios = Venta.StringProductosToList(detalle.DetalleVenta);
                foreach (var item in itemInventarios) {
                    var nombre = "";
                    var total = "";
                    var precioTotal = item.PrecioMenudeo != 0 ? item.PrecioMenudeo : item.PrecioMayoreo;
                    foreach (var producto in productos) {
                        if (item.IdProducto == producto.IdProducto) {
                            nombre = $"{producto.TipoProducto} {producto.NombreProducto}";
                            break;
                        }
                    }
                    table.AddCell(new Cell().Add(new Paragraph(nombre).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph(item.CantidadDependencias.ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph(precioTotal.ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph((precioTotal * item.CantidadDependencias).ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));

                    total = (precioTotal * item.CantidadDependencias).ToString();
                    totalProductos += item.CantidadDependencias;
                    totalDia += Convert.ToDecimal(total);
                }
            }
            document.Add(table);

            var texto1 = "";
            switch (comando) {
                case "hoy":
                    texto1 = $"Total de venta del dia {_date} es de: ${totalDia} con {totalProductos} productos vendidos.";
                    break;
                case "semana":
                    var ultimaSeana = (DateTime.Now - 1.Weeks()).ToString("dd-MM-yy");
                    texto1 = $"Total de venta de {ultimaSeana} a {_date} es de: ${totalDia} con {totalProductos} productos vendidos.";
                    break;
                case "mes":
                    var ultimoMes = (DateTime.Now - 1.Months()).ToString("dd-MM-yy");
                    texto1 = $"Total de venta de {ultimoMes} a {_date} es de: ${totalDia} con {totalProductos} productos vendidos.";
                    break;
            }

            Paragraph p1 = new Paragraph().Add(texto1);
            document.Add(p1);

            document.Close();

            var msg = $"Se ha creado {nombreReporte}.pdf en la ruta {path}";
            MessageBox.Show(msg, "Reporte de sucursal Creado");

            Log.Debug("Reporte de ventas por sucursal creado");

            try {
                PrintDocument pd = new();

                pd.PrinterSettings.PrinterName = "EC-PM-5890X";
                pd.PrintPage += new PrintPageEventHandler(this.ImprimirReporteVentasXUsuario);
                pd.Print();
                Log.Debug("Reporte de ventas por usuario impreso en ticket");

            }
            catch (System.Exception e) {
                Log.Debug("Impresora de ticket no encontrada");
                Log.Error(e.Message);
            }

        }

        private void ImprimirReporteVentas(object sender, PrintPageEventArgs ppeArgs) {
            Log.Debug("Iniciando impresión de repote de ventas...");

            var data = QuerysReportesVentas.GetVentasPorSucursal(idTienda, comandoSucursal);

            var totalProductos = 0;

            decimal totalDia = 0.0m;

            System.Drawing.Image image = System.Drawing.Image.FromFile(@"C:\Mexty\Brand\LogoTicket.png");
            System.Drawing.Point ulCorner = new System.Drawing.Point(40, 0);
            var newimage = ResizeImage(image, 115, 115);

            Graphics g = ppeArgs.Graphics;
            float yPos = 130;
            int count = 0;
            //Read margins from PrintPageEventArgs  
            float leftMargin = 0;
            string type = null;
            string name = null;
            int renglon = 18;

            g.DrawImage(newimage, ulCorner);

            g.DrawString("---------------------------", consola1, Brushes.Black, leftMargin, yPos);
            renglon += 18;
            g.DrawString("     REPORTE DE VENTAS     ", consola1, Brushes.Black, leftMargin, yPos + renglon - 9);
            renglon += 18;
            g.DrawString("---------------------------", consola1, Brushes.Black, leftMargin, yPos + renglon - 9);
            renglon += 18;
            g.DrawString($"Sucursal: {sucursal.ToUpper()} ", consola1, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 18;
            g.DrawString($"Fecha: {_dateNow} ", consola1, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 18;
            g.DrawString("Cant Producto         Total", consola, Brushes.Black, leftMargin, yPos + renglon + 2);
            renglon += 15;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos + renglon - 8);
            float topMargin = 145 + renglon;
            foreach (var detalle in data) {
                itemInventarios = Venta.StringProductosToList(detalle.DetalleVenta);
                foreach (var item in itemInventarios) {
                    var precioTotal = item.PrecioMenudeo != 0 ? item.PrecioMenudeo : item.PrecioMayoreo;
                    foreach (var producto in productos) {
                        if (item.IdProducto == producto.IdProducto) {
                            if (producto.TipoProducto == "Otros" || producto.TipoProducto == "Extras") {
                                type = "";
                            }
                            else type = producto.TipoProducto[..1] + ".";

                            name = producto.NombreProducto.Length >= 15 ? producto.NombreProducto[..15] : producto.NombreProducto;

                            break;
                        }
                    }
                    var total = precioTotal * item.CantidadDependencias;
                    yPos = topMargin + (count * consola.GetHeight(g));
                    g.DrawString(string.Format("{0,2} {1,-2}{2,-11}", item.CantidadDependencias, type, name), consola, Brushes.Black, leftMargin, yPos);
                    g.DrawString(string.Format("                     {0,6}", total), consola, Brushes.Black, leftMargin, yPos);
                    count++;
                    totalProductos += item.CantidadDependencias;
                    totalDia += Convert.ToDecimal(total);

                }
            }
            var newYpos = yPos + 15;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;

            switch (comandoSucursal) {
                case "hoy":
                    //"---------------------------"
                    g.DrawString("Total de venta del dia", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"{_date} es ${totalDia}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"con {totalProductos} productos vendidos", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    break;
                case "semana":

                    var ultimaSeana = (DateTime.Now - 1.Weeks()).ToString("dd-MM-yy");

                    g.DrawString($"Total de ventas de", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"{ultimaSeana} a {_date}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"es ${totalDia}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"con {totalProductos} productos vendidos", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    break;
                case "mes":
                    var ultimoMes = (DateTime.Now - 1.Months()).ToString("dd-MM-yy");
                    g.DrawString($"Total de ventas de", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"{ultimoMes} a {_date}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"es ${totalDia}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"con {totalProductos} productos vendidos", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    break;
            }

            Log.Debug("Finalizando impresión de repote de ventas.");
        }

        private void ImprimirReporteVentasXUsuario(object sender, PrintPageEventArgs ppeArgs) {
            Log.Debug("Iniciando impresión de repote de ventas por usuario...");

            var data = QuerysReportesVentas.GetVentasPorUsuario(username, comandoUsuario);

            var totalProductos = 0;

            decimal totalDia = 0.0m;

            System.Drawing.Image image = System.Drawing.Image.FromFile(@"C:\Mexty\Brand\LogoTicket.png");
            System.Drawing.Point ulCorner = new System.Drawing.Point(40, 0);

            var newimage = ResizeImage(image, 115, 115);

            Graphics g = ppeArgs.Graphics;
            float yPos = 130;
            int count = 0;
            //Read margins from PrintPageEventArgs  
            float leftMargin = 0;
            string type = null;
            string name = null;
            int renglon = 18;

            g.DrawImage(newimage, ulCorner);

            g.DrawString("---------------------------", consola1, Brushes.Black, leftMargin, yPos);
            renglon += 18;
            g.DrawString(" REPORTE VENTAS DE USUARIO ", consola1, Brushes.Black, leftMargin, yPos + renglon - 9);
            renglon += 18;
            g.DrawString("---------------------------", consola1, Brushes.Black, leftMargin, yPos + renglon - 9);
            renglon += 18;
            g.DrawString($"Usuario: {username.ToUpper()} ", consola1, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 18;
            g.DrawString($"Fecha: {_dateNow} ", consola1, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 18;
            g.DrawString("Cant Tipo    Nombre   Total", consola1, Brushes.Black, leftMargin, yPos + renglon + 2);
            renglon += 18;
            g.DrawString("---------------------------", consola1, Brushes.Black, leftMargin, yPos + renglon - 8);
            float topMargin = 145 + renglon;
            foreach (var detalle in data) {
                itemInventarios = Venta.StringProductosToList(detalle.DetalleVenta);
                foreach (var item in itemInventarios) {
                    var precioTotal = item.PrecioMenudeo != 0 ? item.PrecioMenudeo : item.PrecioMayoreo;
                    foreach (var producto in productos) {
                        if (item.IdProducto == producto.IdProducto) {
                            if (producto.TipoProducto == "Otros" || producto.TipoProducto == "Extras") {
                                type = "";
                            }
                            else type = producto.TipoProducto[..1] + ".";

                            name = producto.NombreProducto.Length >= 15 ? producto.NombreProducto[..15] : producto.NombreProducto;

                            break;
                        }
                    }
                    var total = precioTotal * item.CantidadDependencias;
                    yPos = topMargin + (count * consola.GetHeight(g));
                    g.DrawString(string.Format("{0,2} {1,-2}{2,-11}", item.CantidadDependencias, type, name), consola, Brushes.Black, leftMargin, yPos);
                    g.DrawString(string.Format("                     {0,6}", total), consola, Brushes.Black, leftMargin, yPos);
                    count++;
                    totalProductos += item.CantidadDependencias;
                    totalDia += Convert.ToDecimal(total);
                }
            }
            var newYpos = yPos + 15;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;

            switch (comandoUsuario) {
                case "hoy":
                    //"---------------------------"
                    g.DrawString("Total de venta del dia", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"{_date} es ${totalDia}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"con {totalProductos} productos vendidos", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    break;
                case "semana":
                    var ultimaSeana = (DateTime.Now - 1.Weeks()).ToString("dd-MM-yy");
                    g.DrawString($"Total de ventas de", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"{ultimaSeana} a {_date}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"es ${totalDia}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"con {totalProductos} productos vendidos", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    break;
                case "mes":
                    var ultimoMes = (DateTime.Now - 1.Months()).ToString("dd-MM-yy");
                    g.DrawString($"Total de ventas de", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"{ultimoMes} a {_date}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"es ${totalDia}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"con {totalProductos} productos vendidos", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    break;
            }
            Log.Debug("Finalizando impresión de repote de ventas por usuario.");

        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImage(System.Drawing.Image image, int width, int height) {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

    }
}
