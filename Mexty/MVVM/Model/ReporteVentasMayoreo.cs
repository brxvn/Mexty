using FluentDate;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Element;
using iText.Layout.Properties;
using log4net;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TextAlignment = iText.Layout.Properties.TextAlignment;


namespace Mexty.MVVM.Model {
    class ReporteVentasMayoreo : BaseReportes {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        private static string[] columnas = { "Nombre Cliente", "Nombre Producto", "Piezas Vendidas", "Precio", "Total Venta" };
        private static float[] tamaños = { 1, 1, 1, 1, 1 };


        List<ItemInventario> itemInventarios = new();
        List<ItemInventario> ventaTotal = new();
        private int idTienda;
        private string comandoSucursal;

        private Font consola1 = new("Courier New", 8, System.Drawing.FontStyle.Bold);


        List<Producto> productos = QuerysProductos.GetTablesFromProductos();
        List<Sucursal> sucursales = QuerysSucursales.GetTablesFromSucursales();

        public void ReportesVentasMayoreo(int idTienda, string comando) {
            var rawData = QuerysReportesVentas.GetVentasMayoreo(idTienda, comando);
            var clientes = QuerysClientes.GetTablesFromClientes();

            if (rawData.Count == 0) {
                MessageBox.Show("No hay nada que mostrar.\nPrimero realiza ventas.");
                return;
            }
            this.idTienda = idTienda;
            this.comandoSucursal = comando;


            foreach (Sucursal tienda in sucursales) {
                if (tienda.IdTienda == idTienda) {
                    sucursal = tienda.NombreTienda;
                    direccion = tienda.Dirección;
                    break;
                }
            }

            string path = $"{_sucursalesVentas}{sucursal}-Mayoreo\\{comando}\\";
            Directory.CreateDirectory($"{path}");
            string nombreReporte = $"ReporteVentasMayoreo-{sucursal}-{comando}-{_date}";
            string tituloReporte = $"Reporte de Ventas Mayoreo {comando} {sucursal}";

            var texto = $"{sucursal} - {direccion} \n" + $"{_dateNow} \n" + $"{usuarioActivo}";

            Paragraph p = new Paragraph().Add(texto);

            var document = CreateDocument(nombreReporte, p, tituloReporte, path);
            decimal totalTotal = 0.0m;

            for (int j = 0; j <= rawData.Count - 1; j++) {
                var weekAux = 0;
                var weekNo = rawData[j].WeekNo;

                if (j < rawData.Count - 1) {
                    weekAux = rawData[j + 1].WeekNo;
                }
                else {
                    weekNo = rawData[j].WeekNo;

                }

                if (weekNo != weekAux) {
                    var weekStart = rawData[j].WeekStart;
                    var weekEnd = rawData[j].WeekEnd;
                    var semana = $"Semana del {weekStart.ToString("dd-MM-yy")} al {weekEnd.ToString("dd-MM-yy")}";
                    var totalSemana = 0.0m;


                    Paragraph w = new Paragraph().Add(semana);
                    document.Add(w);

                    for (int i = 0; i <= rawData.Count - 1; i++) {
                        var aux = "";

                        var fechaVenta = rawData[i].FechaRegistro.ToString("dd-MM-yy");

                        if (i < rawData.Count - 1) aux = rawData[i + 1].FechaRegistro.ToString("dd-MM-yy");
                        else fechaVenta = rawData[i].FechaRegistro.ToString("dd-MM-yy");

                        int gWeekStart = DateTime.Compare(rawData[i].FechaRegistro, weekStart);
                        int lWeekEnd = DateTime.Compare(rawData[i].FechaRegistro, weekEnd);


                        if (fechaVenta != aux && (gWeekStart == 1 || gWeekStart == 0) && (lWeekEnd == -1 || lWeekEnd == 0)) {
                            Paragraph f = new Paragraph().Add($"Día: {fechaVenta}");
                            document.Add(f);

                            Table table = new Table(UnitValue.CreatePercentArray(tamaños));

                            table.SetWidth(UnitValue.CreatePercentValue(100));
                            foreach (string columa in columnas) {
                                table.AddHeaderCell(new Cell().Add(new Paragraph(columa).SetTextAlignment(TextAlignment.CENTER).SetFontSize(_fontSize)));
                            }

                            var totalProductos = 0;
                            decimal totalDia = 0.0m;


                            foreach (var items in rawData) {
                                
                                foreach (var cliente in clientes) {

                                    if (fechaVenta == items.FechaRegistro.ToString("dd-MM-yy") && (gWeekStart == 1 || gWeekStart == 0) && (lWeekEnd == -1 || lWeekEnd == 0) && items.IdCliente == cliente.IdCliente) {
                                        var nombreCliente = $"{cliente.Nombre} {cliente.ApPaterno}";
                                        var nombre = "";
                                        var total = "";

                                        itemInventarios = Venta.StringProductosToList(items.DetalleVenta);
                                        foreach (var item in itemInventarios) {

                                            var precioTotal = item.PrecioMenudeo != 0 ? item.PrecioMenudeo : item.PrecioMayoreo;
                                            foreach (var producto in productos) {
                                                if (item.IdProducto == producto.IdProducto) {
                                                    nombre = $"{producto.TipoProducto} {producto.NombreProducto}";
                                                    break;
                                                }
                                            }
                                            table.AddCell(new Cell().Add(new Paragraph(nombreCliente).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                                            table.AddCell(new Cell().Add(new Paragraph(nombre).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                                            table.AddCell(new Cell().Add(new Paragraph(item.CantidadDependencias.ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                                            table.AddCell(new Cell().Add(new Paragraph(precioTotal.ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));
                                            table.AddCell(new Cell().Add(new Paragraph((precioTotal * item.CantidadDependencias).ToString()).SetFontSize(_fontSize).SetTextAlignment(TextAlignment.CENTER)));

                                            total = (precioTotal * item.CantidadDependencias).ToString();
                                            totalProductos += item.CantidadDependencias;
                                            totalDia += Convert.ToDecimal(total);

                                        }

                                    }
                                    else continue;
                                }
                            }
                            totalTotal += totalDia;
                            totalSemana += totalDia;
                            document.Add(table);
                            Paragraph totalp = new Paragraph().Add($"Productos Vendidos: {totalProductos}. Total Día: ${totalDia}");
                            document.Add(totalp);
                        }
                    }
                    Paragraph totalWeek = new Paragraph().Add($"Total de venta semana: ${totalSemana}");
                    document.Add(totalWeek);
                    SolidLine line = new SolidLine(1f);
                    LineSeparator ls = new LineSeparator(line);
                    ls.SetWidth(UnitValue.CreatePercentValue(100));
                    document.Add(ls);
                    document.Add(ls);

                }
            }

            var texto1 = "";
            switch (comando) {
                case "hoy":
                    texto1 = $"Total de venta del día {_date} es de: ${totalTotal}";
                    break;
                case "semana":
                    var ultimaSeana = (DateTime.Now - 1.Weeks()).ToString("dd-MM-yy");
                    texto1 = $"Total de venta de {ultimaSeana} a {_date} es de: ${totalTotal}";
                    break;
                case "mes":
                    var ultimoMes = (DateTime.Now - 1.Months()).ToString("dd-MM-yy");
                    texto1 = $"Total de venta de {ultimoMes} al {_date} es de: ${totalTotal}";
                    break;
            }

            Paragraph p1 = new Paragraph().Add(texto1);
            document.Add(p1);

            document.Close();
            var msg = $"Se ha creado {nombreReporte}.pdf en la ruta {path}";
            MessageBox.Show(msg, "Reporte de sucursal Creado");

            Log.Debug("Reporte de ventas por sucursal creado");
        }

    }
}
