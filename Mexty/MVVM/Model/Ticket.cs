﻿using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;

namespace Mexty.MVVM.Model {
    public class Ticket {

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
        private Image qrCode = Image.FromFile(@"C:\Mexty\Brand\qr.png");
        private int totalProductos;

        public Ticket(string totalVenta, string recibido, string cambio, List<ItemInventario> listaVenta) {
            this.totalVenta = totalVenta;
            this.recibido = Convert.ToDecimal(recibido);
            this.cambio = cambio;
            this.listaVenta = listaVenta;
        }

        public void ImprimirTicketVenta() {
            pd.PrinterSettings.PrinterName = "EC-PM-5890X";
            pd.PrintPage += new PrintPageEventHandler(PrintTicket);
            pd.Print();
        }

        private void PrintTicket(object sender, PrintPageEventArgs ppeArgs) {
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

            Image image = Image.FromFile(@"C:\Mexty\Brand\LogoReportes - Copy.png");
            var imageResized = ResizeImage(image, 185, 58);
            //Icon icon = Icon.ExtractAssociatedIcon(@"C:\Mexty\Brand\LogoTicket.ico");

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

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImage(Image image, int width, int height) {
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