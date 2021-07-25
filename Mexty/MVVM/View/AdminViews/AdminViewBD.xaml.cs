﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using log4net;
using Mexty.MVVM.Model;
using Microsoft.Win32;

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Interaction logic for AdminViewBD.xaml
    /// </summary>
    public partial class AdminViewBD : UserControl {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public AdminViewBD() {
            try {
                InitializeComponent();
                Log.Debug("Se han inicializado los campos de Adm Base de datos exitosamente.");

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos de Adm Base de datos.");
                Log.Error($"Error: {e.Message}");
            }

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


        /// <summary>
        /// Lógica del botón de exportar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Export(object sender, RoutedEventArgs e) {
            Log.Debug("Se ha presionado el boton de Exportar");
            try {
                if (Database.BackUp()) {
                    Log.Info("Se ha exportado la base de datos de manera exitosa.");
                    MessageBox.Show("Se ha exportado la base de datos con exito.");
                }
            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error al exportar la base de datos.");
                Log.Error($"Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Lógica del botón de importar datos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Import(object sender, RoutedEventArgs e) {
            Log.Debug("Se ha presionado el boton de importar datos.");
            try {
                OpenFileDialog dialog = new OpenFileDialog();
                // TODO: probablemente solo abrir .sql
                dialog.Filter = "Text files (*.sql;*.txt)|*.sql;*.txt";
                if (dialog.ShowDialog() == true) {
                    if (Database.Import(dialog.FileName)) {
                        Log.Info("Se ha importado a la base de datos de manera exitosa.");
                        MessageBox.Show("Se ha importado a la base de datos con exito.");
                    }
                }
            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error al importar la base de datos.");
                Log.Error($"Error: {exception.Message}");
            }
        }

        private void ExportData(object sender, RoutedEventArgs e) {

        }
    }
}
