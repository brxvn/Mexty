using System;
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

namespace Mexty.MVVM.View {
    /// <summary>
    /// Interaction logic for AdminViewUser.xaml
    /// </summary>
    public partial class AdminViewUser : UserControl {

        public AdminViewUser() {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        public void UpdateTimerTick(object sender, EventArgs e) {
            time.Content = DateTime.Now.ToString("G");
        }

        //private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) {

        //    var tbx = sender as TextBox;
        //    string empty = "";
        //    if (tbx.Text != "") {
        //        var newtext = tbx.Text;
        //        labelText.Content = newtext;
        //        labelText.Visibility = Visibility.Visible;
        //    }
        //    else labelText.Content = empty;
        //}
    }
}
