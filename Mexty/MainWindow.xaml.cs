﻿using Mexty.MVVM.Model;
using Mexty.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Mexty {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            
            InitializeComponent();
                        
            DataContext = new MainViewModel();
            
            if (Database.GetRol().Equals(3)) {
                Admn.Visibility = Visibility.Collapsed;
            }
            activeUser.Text = Database.GetUsername();

            //Para mostrar la hora actual del sistema
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
 
        }

        /// <summary>
        /// Método para mostrar hora y fecha actual en pantalla.
        /// </summary>
        public void UpdateTimerTick(object sender, EventArgs e) {
            time.Content = DateTime.Now.ToString("G");
        }

        /// <summary>
        /// Logica del botón para cerrar sesión desde la pantalla principal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SignOut(object sender, RoutedEventArgs e) {
            var message = "¿Desea cerrar sesión?";
            var title = "Confirmación.";
            if (MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK) {
                Login login = new();
                Database.CloseConnection();
                login.Show();
                Close();
            }
            
        }

    }
}
