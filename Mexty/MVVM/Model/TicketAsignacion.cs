using log4net;
using Mexty.MVVM.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mexty.MVVM.Model {
    class TicketAsignacion {

        private static readonly ILog Log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private List<LogInventario> itemInventarios;
        private List<Sucursal> sucursals = DatabaseQuerys.QuerysSucursales.GetTablesFromSucursales();
        private PrintDocument pd = new();
        private Font consola = new("Courier New", 8, FontStyle.Bold);


        public TicketAsignacion(List<LogInventario> itemInventarios) {
            Log.Debug("Modulo de impresión de asignación de producto empezado");
            this.itemInventarios = itemInventarios;
            try {
                pd.PrinterSettings.PrinterName = "EC-PM-5890X";
                pd.PrintPage += new PrintPageEventHandler(imprimitTicketAsignacion);
                pd.Print();
            }
            catch (Exception e) {
                Log.Error(e.Message);

            }
        }

        private void imprimitTicketAsignacion(object sender, PrintPageEventArgs ppeArgs) {
            Graphics g = ppeArgs.Graphics;

            /* g.DrawIcon(icon, new Rectangle(0,0,100,50))*/
            ;
            var settings = ppeArgs.PageSettings;
            float yPos = 0;
            int count = 0;
            //Read margins from PrintPageEventArgs  
            float leftMargin = 0;
            int renglon = 18;

            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos);
            yPos += renglon;
            g.DrawString("Ticket asignación de produc", consola, Brushes.Black, leftMargin, yPos);
            yPos += renglon;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos);
            float topMargin = 40 + renglon;

            foreach (var item in itemInventarios) {
                yPos = topMargin + (count * consola.GetHeight(g));
                g.DrawString(item.Mensaje.Substring(0, 21), consola, Brushes.Black, leftMargin, yPos);
                yPos += renglon;
                g.DrawString(item.Mensaje.Substring(21,30), consola, Brushes.Black, leftMargin, yPos);
                yPos += renglon;
                g.DrawString(item.Mensaje.Substring(49), consola, Brushes.Black, leftMargin, yPos);
                yPos += renglon;
                g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos);
                yPos += renglon;
                count++;
                count++;
                count++;
                count++;
                count++;
            }
            Log.Debug("Se han impreso las asignaciones seleccionadas");
        }
    }
}
