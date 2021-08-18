﻿using log4net;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using log4net;

namespace Mexty.MVVM.Model {
    public class Ticket {
        private static readonly ILog Log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private Font consola = new("Courier New", 8, FontStyle.Bold);
        private PrintDocument pd = new();
        //("Courier New"

        private readonly string _date = DateTime.Now.ToString("dd-MM-yy");
        private readonly string _dateNow = DateTime.Now.ToString("G");

        private string sucursal = "";
        private string direccion = "";
        private readonly string usuarioActivo = DatabaseInit.GetUsername();
        private string totalVenta;
        private decimal recibido;
        private string cambio;
        private List<ItemInventario> listaVenta;
        private List<Cliente> listaClientes = QuerysClientes.GetTablesFromClientes();
        private Image qrCode = Image.FromFile(@"C:\Mexty\Brand\qr.png");
        private int totalProductos;
        private int idCliente;
        private float deudaCliente;

        public Ticket(string totalVenta, string recibido, string cambio, List<ItemInventario> listaVenta, Venta ventaActual) {
            this.totalVenta = totalVenta;
            this.recibido = Convert.ToDecimal(recibido);
            this.cambio = cambio;
            this.listaVenta = listaVenta;
            idCliente = ventaActual.IdCliente;
        }

        public void ImprimirTicketVenta(bool tipoVenta = true) {
            try {
                if (tipoVenta) {
                    pd.PrinterSettings.PrinterName = "EC-PM-5890X";
                    pd.PrintPage += new PrintPageEventHandler(PrintTicketMenudeo);
                    pd.Print();
                }
                else {
                    pd.PrinterSettings.PrinterName = "EC-PM-5890X";
                    pd.PrintPage += new PrintPageEventHandler(PrintTicketMayoreo);
                    pd.Print();
                }

            }
            catch (Exception e) {
                Log.Error("No se encontró la impresora");
                Log.Error(e.Message);
            }
        }

        private void PrintTicketMenudeo(object sender, PrintPageEventArgs ppeArgs) {
            foreach (var item in listaVenta) {
                totalProductos += item.CantidadDependencias;
            }

            int idTienda = DatabaseInit.GetIdTienda();

            var ListaSucursales = QuerysSucursales.GetTablesFromSucursales();

            foreach (Sucursal tienda in ListaSucursales) {
                if (tienda.IdTienda == idTienda) {
                    sucursal = tienda.NombreTienda;
                    direccion = tienda.Dirección;
                }
            }

            Image image = Image.FromFile(@"C:\Mexty\Brand\LogoTicket.png");

            Point ulCorner = new Point(0, 0);

            Graphics g = ppeArgs.Graphics;

            /* g.DrawIcon(icon, new Rectangle(0,0,100,50))*/
            ;
            var settings = ppeArgs.PageSettings;
            float yPos = 70;
            int count = 0;
            //Read margins from PrintPageEventArgs  
            float leftMargin = 0;
            int renglon = 18;

            g.DrawImage(image, ulCorner);

            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos);
            renglon += 15;
            g.DrawString("      TICKET DE VENTA      ", consola, Brushes.Black, leftMargin, yPos + renglon - 10);
            renglon += 15;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos + renglon - 10);
            renglon += 15;
            g.DrawString($"Sucursal: {sucursal.ToUpper()} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 15;
            g.DrawString($"Usuario: {usuarioActivo} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 15;
            g.DrawString($"Fecha: {_dateNow} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 15;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos + renglon);

            float topMargin = 85 + renglon;

            foreach (var item in listaVenta) {
                var total = (item.CantidadDependencias * item.PrecioMenudeo).ToString();
                var type = "";
                var name = "";
                if (item.TipoProducto is "Paleta Agua" or "Paleta Leche" or "Paleta Fruta") {
                    type = item.TipoProducto[..9];
                }
                else type = item.TipoProducto;

                name = item.NombreProducto.Length >= 7 ? item.NombreProducto[..7] : item.NombreProducto;
                yPos = topMargin + (count * consola.GetHeight(g));
                g.DrawString(string.Format("{0,2} {1,-9} {2,-7} {3,6}", item.CantidadDependencias, type, name, total), consola, Brushes.Black, leftMargin, yPos);
                count++;
            }
            var newYpos = yPos + 15;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            g.DrawString(string.Format("Total {0,21}", totalVenta), consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            g.DrawString(string.Format("Recibido {0,18:C}", recibido), consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            g.DrawString(string.Format("Cambio {0,20}", cambio), consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            g.DrawString(string.Format("  Productos Vendidos  {0,3}", totalProductos), consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            g.DrawString(string.Format("  ¡Gracias por su compra!  ", totalVenta), consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            Point point = new Point(45, (int)newYpos);
            g.DrawImage(qrCode, point);
            newYpos += 100;
            g.DrawString("ENTRA PARA MÁS PROMOCIONES ", consola, Brushes.Black, leftMargin, newYpos);
        }

        private void PrintTicketMayoreo(object sender, PrintPageEventArgs ppeArgs) {
            foreach (var item in listaVenta) {
                totalProductos += item.CantidadDependencias;
            }

            foreach (var item in listaClientes) {
                if (item.IdCliente == idCliente) {
                    deudaCliente = item.Debe;
                    break;
                }
            }

            int idTienda = DatabaseInit.GetIdTienda();

            var ListaSucursales = QuerysSucursales.GetTablesFromSucursales();

            foreach (Sucursal tienda in ListaSucursales) {
                if (tienda.IdTienda == idTienda) {
                    sucursal = tienda.NombreTienda;
                    direccion = tienda.Dirección;
                }
            }

            Image image = Image.FromFile(@"C:\Mexty\Brand\LogoTicket.png");

            Point ulCorner = new Point(0, 0);

            Graphics g = ppeArgs.Graphics;

            /* g.DrawIcon(icon, new Rectangle(0,0,100,50))*/
            ;
            var settings = ppeArgs.PageSettings;
            float yPos = 70;
            int count = 0;
            //Read margins from PrintPageEventArgs  
            float leftMargin = 0;
            int renglon = 18;

            g.DrawImage(image, ulCorner);

            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos);
            renglon += 15;
            g.DrawString("  TICKET DE VENTA MAYOREO  ", consola, Brushes.Black, leftMargin, yPos + renglon - 10);
            renglon += 15;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos + renglon - 10);
            renglon += 15;
            g.DrawString($"Sucursal: {sucursal.ToUpper()} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 15;
            g.DrawString($"Usuario: {usuarioActivo} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 15;
            g.DrawString($"Fecha: {_dateNow} ", consola, Brushes.Black, leftMargin, yPos + renglon);
            renglon += 15;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, yPos + renglon);

            float topMargin = 85 + renglon;

            foreach (var item in listaVenta) {
                var total = (item.CantidadDependencias * item.PrecioMenudeo).ToString();
                var type = "";
                var name = "";
                if (item.TipoProducto is "Paleta Agua" or "Paleta Leche" or "Paleta Fruta") {
                    type = item.TipoProducto[..9];
                }
                else type = item.TipoProducto;

                name = item.NombreProducto.Length >= 7 ? item.NombreProducto[..7] : item.NombreProducto;
                yPos = topMargin + (count * consola.GetHeight(g));
                g.DrawString(string.Format("{0,2} {1,-9} {2,-7} {3,6}", item.CantidadDependencias, type, name, total), consola, Brushes.Black, leftMargin, yPos);
                count++;
            }
            var newYpos = yPos + 15;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            g.DrawString(string.Format("Total {0,21}", totalVenta), consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            g.DrawString(string.Format("Recibido {0,18:C}", recibido), consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            g.DrawString(string.Format("Cambio {0,20}", cambio), consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            g.DrawString("---------------------------", consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            g.DrawString(string.Format("  Productos Vendidos  {0,3}", totalProductos), consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            g.DrawString(string.Format("  No. Cliente         {0,3}", idCliente), consola, Brushes.Black, leftMargin, newYpos); newYpos += 15;
            g.DrawString(string.Format("  Debe:          {0,6:C}", deudaCliente), consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;            g.DrawString(string.Format("  ¡Gracias por su compra!  ", totalVenta), consola, Brushes.Black, leftMargin, newYpos);
            newYpos += 15;
            Point point = new Point(45, (int)newYpos);
            g.DrawImage(qrCode, point);
            newYpos += 100;
            g.DrawString("ENTRA PARA MÁS PROMOCIONES ", consola, Brushes.Black, leftMargin, newYpos);
        }

    }
}
