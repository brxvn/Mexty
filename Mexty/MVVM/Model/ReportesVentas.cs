using FluentDate;
using iText.Layout.Element;
using iText.Layout.Properties;
using log4net;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            string tituloReporte = $"Reporte de Ventas del {comando} de {sucursal}";

            var texto = $"{sucursal} - {direccion} \n" + $"{_dateNow} \n" + $"{usuarioActivo}";

            Paragraph p = new Paragraph().Add(texto);

            var document = CreateDocument(nombreReporte, p, tituloReporte, path);

            foreach (string columa in columnas) {
                table.AddHeaderCell(new Cell().Add(new Paragraph(columa).SetTextAlignment(TextAlignment.CENTER).SetFontSize(_fontSize)));
            }

            var data = QuerysReportesVentas.GetVentasPorSucursal(idTienda, comando);

            var totalProductos = 0;

            decimal totalDia = 0.0m;
            foreach (var detalle in data) {
                itemInventarios = Venta.StringProductosToList(detalle.DetalleVenta);
                foreach (var item in itemInventarios) {
                    var nombre = "";
                    var total = "";
                    foreach (var producto in productos) {
                        if (item.IdProducto == producto.IdProducto) {
                            nombre = $"{producto.TipoProducto} {producto.NombreProducto}";
                            break;
                        }
                    }
                    table.AddCell(new Cell().Add(new Paragraph(nombre).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph(item.CantidadDependencias.ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph(item.PrecioMenudeo.ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph((item.PrecioMenudeo * item.CantidadDependencias).ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));

                    total = (item.PrecioMenudeo * item.CantidadDependencias).ToString();
                    totalProductos += item.CantidadDependencias;
                    totalDia += Convert.ToDecimal(total);
                }
            }
            document.Add(table);

            var texto1 = "";
            switch (comando) {
                case "hoy":
                    texto1 = $"Total de venta del dia {_date} es: ${totalDia} con {totalProductos} productos vendidos.";
                    break;
                case "semana":
                    var ultimaSeana = DateTime.Now - 1.Weeks();
                    texto1 = $"Total de venta del {ultimaSeana.ToString("d")} a hoy {_date} es: ${totalDia} con {totalProductos} productos vendidos.";
                    break;
                case "mes":
                    var ultimoMes = DateTime.Now - 1.Months();
                    texto1 = $"Total de venta del {ultimoMes.ToString("d")} a hoy {_date} es: ${totalDia} con {totalProductos} productos vendidos.";
                    break;
            }

            Paragraph p1 = new Paragraph().Add(texto1);
            document.Add(p1);

            document.Close();

            var msg = $"Se ha creado {nombreReporte}.pdf en la ruta {path}";
            MessageBox.Show(msg, "Reporte de sucursal Creado");

            Log.Debug("Reporte de ventas por sucursal creado");

            pd.PrinterSettings.PrinterName = "EC-PM-5890X";
            pd.PrintPage += new PrintPageEventHandler(ImprimirReporteVentas);
            pd.Print();
        }


        public void ReporteVentasUsuario(string username, string comando) {
            this.username = username;
            this.comandoUsuario = comando;
            Table table = new Table(UnitValue.CreatePercentArray(tamaños));

            table.SetWidth(UnitValue.CreatePercentValue(100));

            string path = $"{_ventasUsuarios}{username}\\{_date}\\";
            Directory.CreateDirectory($"{path}");
            string nombreReporte = $"ReporteVentas-{username}-{comando}-{_date}";
            string tituloReporte = $"Reporte de Ventas del usuario {username}";

            var texto = $"{sucursal} - {direccion} \n" + $"{_dateNow} \n" + $"{usuarioActivo}";

            Paragraph p = new Paragraph().Add(texto);

            var document = CreateDocument(nombreReporte, p, tituloReporte, path);

            foreach (string columa in columnas) {
                table.AddHeaderCell(new Cell().Add(new Paragraph(columa).SetTextAlignment(TextAlignment.CENTER).SetFontSize(_fontSize)));
            }

            var data = QuerysReportesVentas.GetVentasPorUsuario(username, comando);

            var totalProductos = 0;

            decimal totalDia = 0.0m;
            foreach (var detalle in data) {
                itemInventarios = Venta.StringProductosToList(detalle.DetalleVenta);
                foreach (var item in itemInventarios) {
                    var nombre = "";
                    var total = "";
                    foreach (var producto in productos) {
                        if (item.IdProducto == producto.IdProducto) {
                            nombre = $"{producto.TipoProducto} {producto.NombreProducto}";
                            break;
                        }
                    }
                    table.AddCell(new Cell().Add(new Paragraph(nombre).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph(item.CantidadDependencias.ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph(item.PrecioMenudeo.ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph((item.PrecioMenudeo * item.CantidadDependencias).ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));

                    total = (item.PrecioMenudeo * item.CantidadDependencias).ToString();
                    totalProductos += item.CantidadDependencias;
                    totalDia += Convert.ToDecimal(total);
                }
            }
            document.Add(table);

            var texto1 = "";
            switch (comando) {
                case "hoy":
                    texto1 = $"Total de venta del dia {_date} es: ${totalDia} con {totalProductos} productos vendidos.";
                    break;
                case "semana":
                    var ultimaSeana = DateTime.Now - 1.Weeks();
                    texto1 = $"Total de venta del {ultimaSeana.ToString("d")} a hoy {_date} es: ${totalDia} con {totalProductos} productos vendidos.";
                    break;
                case "mes":
                    var ultimoMes = DateTime.Now - 1.Months();
                    texto1 = $"Total de venta del {ultimoMes.ToString("d")} a hoy {_date} es: ${totalDia} con {totalProductos} productos vendidos.";
                    break;
            }

            Paragraph p1 = new Paragraph().Add(texto1);
            document.Add(p1);

            document.Close();

            var msg = $"Se ha creado {nombreReporte}.pdf en la ruta {path}";
            MessageBox.Show(msg, "Reporte de sucursal Creado");

            Log.Debug("Reporte de ventas por sucursal creado");

            pd.PrinterSettings.PrinterName = "EC-PM-5890X";
            pd.PrintPage += new PrintPageEventHandler(ImprimirReporteVentasXUsuario);
            pd.Print();

        }

        private void ImprimirReporteVentas(object sender, PrintPageEventArgs ppeArgs) {
            var data = QuerysReportesVentas.GetVentasPorSucursal(idTienda, comandoSucursal);

            var totalProductos = 0;

            decimal totalDia = 0.0m;

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
            g.DrawString("Cant Tipo    Nombre   Total", consola1, Brushes.Black, leftMargin, yPos + renglon + 2);
            renglon += 18;
            g.DrawString("---------------------------", consola1, Brushes.Black, leftMargin, yPos + renglon - 8);
            float topMargin = 75 + renglon;
            foreach (var detalle in data) {
                itemInventarios = Venta.StringProductosToList(detalle.DetalleVenta);
                foreach (var item in itemInventarios) {
                    foreach (var producto in productos) {
                        if (item.IdProducto == producto.IdProducto) {
                            if (producto.TipoProducto == "Paleta Agua" || producto.TipoProducto == "Paleta Leche" || producto.TipoProducto == "Paleta Fruta") {
                                type = producto.TipoProducto[..9];
                            }
                            else type = producto.TipoProducto;

                            name = producto.NombreProducto.Length >= 7 ? producto.NombreProducto[..7] : producto.NombreProducto;

                            break;
                        }
                    }
                    var total = item.PrecioMenudeo * item.CantidadDependencias;
                    yPos = topMargin + (count * consola.GetHeight(g));
                    g.DrawString(string.Format("{0,2} {1,-9} {2,-7} {3,6}", item.CantidadDependencias, type, name, total), consola1, Brushes.Black, leftMargin, yPos);
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

                    var ultimaSeana = DateTime.Now - 1.Weeks();
                    g.DrawString($"Total de ventas de", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"{ultimaSeana.ToString("d")} a {_date}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"es ${totalDia}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"con {totalProductos} productos vendidos", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    break;
                case "mes":
                    var ultimoMes = DateTime.Now - 1.Months();
                    g.DrawString($"Total de ventas de", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"{ultimoMes.ToString("d")} a {_date}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"es ${totalDia}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"con {totalProductos} productos vendidos", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    break;
            }


        }

        private void ImprimirReporteVentasXUsuario(object sender, PrintPageEventArgs ppeArgs) {
            var data = QuerysReportesVentas.GetVentasPorUsuario(username, comandoUsuario);

            var totalProductos = 0;

            decimal totalDia = 0.0m;

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
            float topMargin = 75 + renglon;
            foreach (var detalle in data) {
                itemInventarios = Venta.StringProductosToList(detalle.DetalleVenta);
                foreach (var item in itemInventarios) {
                    foreach (var producto in productos) {
                        if (item.IdProducto == producto.IdProducto) {
                            if (producto.TipoProducto == "Paleta Agua" || producto.TipoProducto == "Paleta Leche" || producto.TipoProducto == "Paleta Fruta") {
                                type = producto.TipoProducto[..9];
                            }
                            else type = producto.TipoProducto;

                            name = producto.NombreProducto.Length >= 7 ? producto.NombreProducto[..7] : producto.NombreProducto;

                            break;
                        }
                    }
                    var total = item.PrecioMenudeo * item.CantidadDependencias;
                    yPos = topMargin + (count * consola.GetHeight(g));
                    g.DrawString(string.Format("{0,2} {1,-9} {2,-7} {3,6}", item.CantidadDependencias, type, name, total), consola1, Brushes.Black, leftMargin, yPos);
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

                    var ultimaSeana = DateTime.Now - 1.Weeks();
                    g.DrawString($"Total de ventas de", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"{ultimaSeana.ToString("d")} a {_date}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"es ${totalDia}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"con {totalProductos} productos vendidos", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    break;
                case "mes":
                    var ultimoMes = DateTime.Now - 1.Months();
                    g.DrawString($"Total de ventas de", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"{ultimoMes.ToString("d")} a {_date}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"es ${totalDia}", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    g.DrawString($"con {totalProductos} productos vendidos", consola, Brushes.Black, leftMargin, newYpos);
                    newYpos += 15;
                    break;
            }


        }

    }
}
