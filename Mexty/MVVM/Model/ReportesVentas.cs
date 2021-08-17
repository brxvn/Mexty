using FluentDate;
using iText.Layout.Element;
using iText.Layout.Properties;
using log4net;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace Mexty.MVVM.Model {
    class ReportesVentas : BaseReportes {
        private static readonly ILog Log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        
        public void ReporteVentasDia(int idTienda, string comando) {
            string[] columnas = { "Nombre Producto", "Piezas Vendidas", "Precio", "Total Venta" };
            float[] tamaños = { 1, 1, 1, 1 };

            Table table = new Table(UnitValue.CreatePercentArray(tamaños));
            table.SetWidth(UnitValue.CreatePercentValue(100));
            List<ItemInventario> itemInventarios = new();
            List<ItemInventario> ventaTotal = new();
            var ListaSucursales = QuerysSucursales.GetTablesFromSucursales();

            foreach (Sucursal tienda in ListaSucursales) {
                if (tienda.IdTienda == idTienda) {
                    sucursal = tienda.NombreTienda;
                    direccion = tienda.Dirección;
                    break;
                }
            }

            string path = $"{_sucursalesVentas}{sucursal}\\{comando}\\";
            Directory.CreateDirectory($"{path}");
            string nombreReporte = $"ReporteVentas-{sucursal}-{_date}-{comando}";
            string tituloReporte = $"Reporte de Ventas del {comando} de {sucursal}";

            var texto = $"{sucursal} - {direccion} \n" + $"{_dateNow} \n" + $"{usuarioActivo}";

            Paragraph p = new Paragraph().Add(texto);

            var document = CreateDocument(nombreReporte, p, tituloReporte, path);

            foreach (string columa in columnas) {
                table.AddHeaderCell(new Cell().Add(new Paragraph(columa).SetTextAlignment(TextAlignment.CENTER).SetFontSize(_fontSize)));
            }
            
            var data = QuerysReportesVentas.GetVentasPorSucursal(idTienda, comando);

            var productos = QuerysProductos.GetTablesFromProductos();
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

        }


    }
}
