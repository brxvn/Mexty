using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
using Mexty.MVVM.Model;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Interaction logic for AdminViewUser.xaml
    /// </summary>
    public partial class AdminViewUser : UserControl {

        public AdminViewUser() {
            InitializeComponent();
            
            FillDataGrid();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        public void UpdateTimerTick(object sender, EventArgs e) { // TODO: hacerlo una clase para no repetir código
            time.Content = DateTime.Now.ToString("G");
        }

        /// <summary>
        /// Método que llena la tabla con los datos de la tabla usuarios.
        /// </summary>
        public void FillDataGrid() {
            var connObj = new Database();
            var query = connObj.GetTablesFromUsuarios();
            DataUsuarios.ItemsSource = query;
        }

        public void ItemSelected(object sender, EventArgs e) {
            //DataRowView selected = (DataRowView) DataUsuarios.SelectedItems;
            //     nombreUsuario.Text = rowSelected["ID_USUARIO"].ToString();
            //     apMaternoUsuario.Text = rowSelected["id_usuario"].ToString();
            //     apPaternoUsuario.Text = rowSelected["IDUSUARIO"].ToString();
            //     txtDireccion.Text = rowSelected["idusuario"].ToString();
            //     txtContraseña.Text = (string) DataUsuarios.SelectedCells[1].Column.Header;
        }
        
        /// <summary>
        /// Take a value from a the selected row of a DataGrid
        /// </summary>
        /// <param name="dGrid">The DataGrid where we take the value.</param>
        /// <param name="columnName">The column's name of the searched value. Be careful, the parameter must be the same as the shown on the dataGrid</param>
        /// <returns>The value contained in the selected line or an empty string if nothing is selected or if the column doesn't exist</returns>
        public static string getDataGridValueAt(DataGrid dGrid, string columnName)
        {
            if (dGrid.SelectedItem == null)
                return "...";
            for (int i = 0; i < columnName.Length; i++)
                if (columnName.ElementAt(i) == '_')
                {
                    columnName = columnName.Insert(i, "_");
                    i++;
                }
            string str = dGrid.SelectedItem.ToString(); // Get the selected Line
            str = str.Replace("}", "").Trim().Replace("{", "").Trim(); // Remove useless characters
            for (int i = 0; i < str.Split(',').Length; i++)
                if (str.Split(',')[i].Trim().Split('=')[0].Trim() == columnName) // Check if the searched column exists in the dataGrid.
                    return str.Split(',')[i].Trim().Split('=')[1].Trim();
                else {
                    //return "-----****";
                    return str;
                }
            return str;
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
