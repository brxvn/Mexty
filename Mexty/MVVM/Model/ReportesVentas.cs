using iText.Layout.Element;
using iText.Layout.Properties;
using log4net;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mexty.MVVM.Model {
    class ReportesVentas : BaseReportes {
        private static readonly ILog Log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        
        public void ReporteVentas(int idTienda, string comando) {
            List<ItemInventario> itemInventarios = new();
            var data = QuerysReportesVentas.GetVentasPorSucursal(idTienda, comando);

            foreach (var item in data) {
                itemInventarios = Venta.StringProductosToList(item.DetalleVenta);
            }

            var ListaSucursales = QuerysSucursales.GetTablesFromSucursales();

            foreach (Sucursal tienda in ListaSucursales) {
                if (tienda.IdTienda == idTienda) {
                    sucursal = tienda.NombreTienda;
                    direccion = tienda.Dirección;
                    break;
                }
            }

            //var total = itemInventarios.FindAll(
            //    delegate (ItemInventario it) {
            //        return it.CantidadDependencias > 0;
            //    });

            string path = $"{_sucursalesVentas}{_date}";
            string nombreReporte = $"ReporteVentas-{sucursal}-{_date}";
            string tituloReporte = $"Reporte de Ventas de {sucursal}";

            var texto = $"{sucursal} - {direccion} \n" + $"{_dateNow} \n" + $"{usuarioActivo}";

            Paragraph p = new Paragraph().Add(texto);

            var document = CreateDocument(nombreReporte, p, tituloReporte, path);

            string[] columnas = { "ID", "Tipo de Producto", "Nombre", "Vendidos" };
            float[] tamaños = { 1, 3, 3, 1 };

            Table table = new Table(UnitValue.CreatePercentArray(tamaños));
            table.SetWidth(UnitValue.CreatePercentValue(100));

            foreach (string columa in columnas) {
                table.AddHeaderCell(new Cell().Add(new Paragraph(columa).SetTextAlignment(TextAlignment.CENTER).SetFontSize(_fontSize)));
            }

        }
    }
}
