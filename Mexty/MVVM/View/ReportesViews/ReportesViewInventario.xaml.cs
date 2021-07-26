using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Mexty.MVVM.Model;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Mexty.MVVM.View.ReportesViews {
    /// <summary>
    /// Interaction logic for ReportesViewInventario.xaml
    /// </summary>
    public partial class ReportesViewInventario : UserControl {
        Reports report = new Reports();
        public ReportesViewInventario() {
            InitializeComponent();

            
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        /// <summary>
        /// Actualiza la hora.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimerTick(object sender, EventArgs e) {
            time.Content = DateTime.Now.ToString("G");
        }

    }
}
