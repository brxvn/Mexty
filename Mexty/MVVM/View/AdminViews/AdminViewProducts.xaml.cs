﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Mexty.MVVM.Model.DataTypes;
using Mexty.MVVM.Model.Validations;

namespace Mexty.MVVM.View.AdminViews{
    /// <summary>
    /// Interaction logic for AdminViewProducts.xaml
    /// </summary>
    public partial class AdminViewProducts : UserControl {

        /// <summary>
        /// Lista de productos dada por la base de datos.
        /// </summary>
        private List<Producto> ListaProductos { get; set; }

        /// <summary>
        /// Collection view actual de la datagrid.
        /// </summary>
        private CollectionView CollectionView { get; set; }

        /// <summary>
        /// El último producto seleccionado de la datagrid.
        /// </summary>
        private Producto SelectedProduct { get; set; }
        
        public AdminViewProducts() {
            
            InitializeComponent();
            FillData();
            FillSucursales();
            ClearFields();
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
        /// Método que llena la datadrid con los productos.
        /// </summary>
        private void FillData() {
            var data = Database.GetTablesFromProductos();
            ListaProductos = data;
            var collectionView = new ListCollectionView(data) {
                Filter = (e) => e is Producto producto && producto.Activo != 0 // Solo productos activos en la tabla.
            };
            CollectionView = collectionView;
            DataProductos.ItemsSource = collectionView;
            
            //Datos ComboVenta
            ComboVenta.ItemsSource = Producto.TiposVentaTexto;
            
            //Datos Combo tipos.
            ComboTipo.ItemsSource = Producto.GetTiposProducto();

            //Datos Combo Medida.
            ComboMedida.ItemsSource = Producto.GetTiposMedida();

        }

        private void FillSucursales() {
            var sucursales = Database.GetTablesFromSucursales();
            foreach (var sucursal in sucursales) {
                ComboSucursal.Items.Add(sucursal.NombreTienda);
            }
        }

        /// <summary>
        /// Método que reacciona al evento SelectionChanged del datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, SelectionChangedEventArgs e) {
            ClearFields();
            txtNombreProducto.IsReadOnly = true;
            ComboTipo.IsEnabled = false;
            
            if (DataProductos.SelectedItem == null) return;
            var producto = (Producto) DataProductos.SelectedItem;
            if (producto == null) return;
            SelectedProduct = producto;
            txtNombreProducto.Text = producto.NombreProducto;
            ComboVenta.SelectedIndex = producto.TipoVenta;
            ComboTipo.SelectedItem = producto.TipoProducto;
            txtPrecioMayoreo.Text = producto.PrecioMayoreo.ToString();
            txtPrecioMenudeo.Text = producto.PrecioMenudeo.ToString();
            txtDetalle.Text = producto.DetallesProducto;
            ComboMedida.SelectedItem = producto.MedidaProducto;
            ComboSucursal.SelectedItem = producto.Sucursal;// ojo
            Eliminar.IsEnabled = true;
            Guardar.IsEnabled = true;
        }

        /// <summary>
        /// Método que limpia los campos de datos.
        /// </summary>
        private void ClearFields() {
            txtNombreProducto.Text = "";
            ComboVenta.SelectedIndex = 0;
            ComboTipo.SelectedIndex = 0;
            txtPrecioMayoreo.Text = "";
            txtPrecioMenudeo.Text = "";
            txtDetalle.Text = "";
            ComboMedida.SelectedIndex = 0;
            ComboSucursal.SelectedIndex = 0;
            txtNombreProducto.IsReadOnly = false;
            ComboTipo.IsEnabled = true;
            //Guardar.IsEnabled = false;
            Eliminar.IsEnabled = false;
        }

        /// <summary>
        /// Lógica para el campo de búsqueda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterSearch(object sender, TextChangedEventArgs e) {
            TextBox tbx = sender as TextBox;
            var collection = CollectionView;
            if (tbx != null && tbx.Text != "") {
                var newText = tbx.Text;
                var customFilter = new Predicate<object>(o => FilterLogic(o, newText));

                collection.Filter = customFilter;
                DataProductos.ItemsSource = collection;
                CollectionView = collection;
            }
            else {
                collection.Filter = null;
                var noNull = new Predicate<object>(producto => {
                    if (producto == null) return false;
                    return ((Producto) producto).Activo == 1;
                });
                collection.Filter += noNull;
                DataProductos.ItemsSource = collection;
                CollectionView = collection;
            }
        }

        /// <summary>
        /// Lógica para el filtro del data grid.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool FilterLogic(object obj, string text) {
            text = text.ToLower();
            var producto = (Producto) obj;
            if (producto.NombreProducto.Contains(text) ||
                producto.IdProducto.ToString().Contains(text) ||
                producto.TipoProducto.ToLower().Contains(text) ||
                producto.TipoVentaNombre.ToLower().Contains(text)) {
                return producto.Activo == 1;
            }
            return false;
        }

        /// <summary>
        /// Lógica del boton de Guardar producto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrarProducto(object sender, RoutedEventArgs e) {
            var newProduct = new Producto();
            newProduct.NombreProducto = txtNombreProducto.Text;
            newProduct.MedidaProducto = ComboMedida.SelectedItem.ToString();
            newProduct.TipoProducto = ComboTipo.SelectedItem.ToString();
            newProduct.TipoVenta = ComboVenta.SelectedIndex;
            newProduct.TipoProducto = ComboTipo.SelectedItem.ToString();
            newProduct.PrecioMayoreo = txtPrecioMayoreo.Text == "" ? 0 : int.Parse(txtPrecioMayoreo.Text);
            newProduct.PrecioMenudeo = txtPrecioMayoreo.Text == "" ? 0 : int.Parse(txtPrecioMenudeo.Text);
            newProduct.DetallesProducto = txtDetalle.Text;
            newProduct.Sucursal = ComboSucursal.SelectedItem.ToString();

            var validator = new ProductValidation();
            var results = validator.Validate(newProduct);
            if (!results.IsValid) {
                foreach (var error in results.Errors) {
                    MessageBox.Show(error.ErrorMessage);
                }
                return;
            }

            if (SelectedProduct != null && SelectedProduct.NombreProducto == newProduct.NombreProducto) {
                newProduct.IdProducto = SelectedProduct.IdProducto;
                newProduct.Activo = SelectedProduct.Activo;
                Database.UpdateData(newProduct);
                
                var msg = $"Se ha actualizado el producto {newProduct.IdProducto.ToString()} {newProduct.NombreProducto}.";
                MessageBox.Show(msg, "Producto Actualizado");
            }
            else {
                var alta = true;
                foreach (var producto in ListaProductos) {
                    if (newProduct.NombreProducto == producto.NombreProducto && producto.Activo == 0) {
                        // actualizamos y activamos.
                        newProduct.IdProducto = producto.IdProducto;
                        newProduct.Activo = 1;
                        Database.UpdateData(newProduct);
                        alta = false;
                        var msg = $"Se ha activado y actualizado el producto {newProduct.IdProducto.ToString()} {newProduct.NombreProducto}.";
                        MessageBox.Show(msg, "Producto Actualizado");
                    }
                }
                if (alta) {
                    // Alta
                    Database.NewProduct(newProduct);
                    var msg = $"Se ha dado de alta el producto {newProduct.NombreProducto}.";
                    MessageBox.Show(msg, "Producto Actualizado");
                }
            }
            FillData();
            ClearFields();
        }

        /// <summary>
        /// Elimina (hace inactivo) el producto seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarProducto(object sender, RoutedEventArgs e) {
            var producto = SelectedProduct;
            var mensaje = $"¿Seguro quiere eliminar el producto {producto.NombreProducto}?";
            const MessageBoxButton buttons = MessageBoxButton.OKCancel;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            if (MessageBox.Show(mensaje, "Confirmación", buttons, icon) != MessageBoxResult.OK) return;
            producto.Activo = 0;
            Database.UpdateData(producto);
            ClearFields();
            FillData();
        }

        /// <summary>
        /// Función que valida los campos númericos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Regresa la cadena dada en Mayusculas y sin espeacios.
        /// </summary>
        /// <param name="text">Texto a Preparar.</param>
        /// <returns></returns>
        private static string StrPrep(string text) {
            return text.ToUpper().Replace(" ", "");
        }

        /// <summary>
        /// Limpia los text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            ClearFields();
        }
        
        // --- Eventos Text-Update--

        private void TextUpdateNombre(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            txtNombreProducto.Text = textbox.Text;
            Guardar.IsEnabled = textbox.Text != "";
        }

        private void TextUpdatePrecioMenudeo(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            txtPrecioMenudeo.Text = textbox.Text;
            Guardar.IsEnabled = textbox.Text != "";
        }    

        private void TextUpdatePrecioMayoreo(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            txtPrecioMayoreo.Text = textbox.Text;
            Guardar.IsEnabled = textbox.Text != "";
        }

        private void TextUpdateDetalle(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            txtDetalle.Text = textbox.Text;
            Guardar.IsEnabled = textbox.Text != "";
        }

        private void TextUpdateMedida(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            //txtMedida.Text = textbox.Text;
        }
    }
}
