using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows;
using log4net;
using Mexty.MVVM.Model.DataTypes;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Clase que contiene las consultas a la base de datos del modulo de Productos
    /// </summary>
    public static class QuerysProductos {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Función que retorna una lista con los productos de la base de datos.
        /// </summary>
        /// <returns> Una lista de objetos tipo <c>Producto</c>.</returns>
        public static List<Producto> GetTablesFromProductos() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var querry = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select * from cat_producto"
            };

            var productos = new List<Producto>();
            try {
                using var reader = querry.ExecuteReader();
                while (reader.Read()) {
                    var producto = new Producto {
                        IdProducto = reader.IsDBNull("id_producto") ? 0 : reader.GetInt32("id_producto"),
                        NombreProducto = reader.IsDBNull("nombre_producto") ? "" : reader.GetString("nombre_producto"),
                        MedidaProducto = reader.IsDBNull("medida") ? "" : reader.GetString("medida"),
                        TipoProducto = reader.IsDBNull("tipo_producto") ? "" : reader.GetString("tipo_producto"),
                        TipoVenta = reader.IsDBNull("tipo_venta") ? 0 : reader.GetInt32("tipo_venta"),
                        PrecioMayoreo = reader.IsDBNull("precio_mayoreo") ? 0 : reader.GetDecimal("precio_mayoreo"),
                        PrecioMenudeo = reader.IsDBNull("precio_menudeo") ? 0 : reader.GetDecimal("precio_menudeo"),
                        DetallesProducto =
                            reader.IsDBNull("especificacion_producto") ? "" : reader.GetString("especificacion_producto"),
                        Activo = reader.IsDBNull("activo") ? 0 : reader.GetInt32("activo"),
                        DependenciasText = reader.IsDBNull("dependencias") ? "" : reader.GetString("dependencias")
                    };
                    productos.Add(producto);
                }

                Log.Debug("Se han obtenido con exito las tablas de productos.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de productos.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 14: ha ocurrido un error al intentar obtener la información de la base de datos. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
            finally {
                connObj.Close();
            }

            return productos;
        }


        /// <summary>
        /// Método que actualiza un producto en la base de datos.
        /// </summary>
        /// <param name="producto"></param>
        public static int UpdateData(Producto producto) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                update cat_producto 
                set NOMBRE_PRODUCTO=@nom, 
                    TIPO_PRODUCTO=@tipoP, 
                    MEDIDA=@med, 
                    DEPENDENCIAS=@dependencias,
                    TIPO_VENTA=@tipoVX, 
                    PRECIO_MAYOREO=@pMayo, 
                    PRECIO_MENUDEO=@pMenu, 
                    ESPECIFICACION_PRODUCTO=@esp, 
                    ACTIVO=@actX 
                where ID_PRODUCTO=@idX"
            };

            query.Parameters.AddWithValue("@nom", producto.NombreProducto);
            query.Parameters.AddWithValue("@tipoP", producto.TipoProducto);
            query.Parameters.AddWithValue("@med", producto.MedidaProducto);
            query.Parameters.AddWithValue("@piezasX", producto.Piezas.ToString());
            query.Parameters.AddWithValue("@tipoVX", producto.TipoVenta.ToString());
            query.Parameters.AddWithValue("@pMayo", producto.PrecioMayoreo.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@pMenu", producto.PrecioMenudeo.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@esp", producto.DetallesProducto);
            query.Parameters.AddWithValue("@idX", producto.IdProducto.ToString());
            query.Parameters.AddWithValue("@actX", producto.Activo.ToString());
            query.Parameters.AddWithValue("@dependencias", producto.DependenciasText);

            try {
                var cmd = query.CommandText;
                for (var index = 0; index < query.Parameters.Count; index++) {
                    var queryParameter = query.Parameters[index];

                    if (queryParameter.ParameterName.Contains("X")) { // Si tiene X es numerico
                        // No se exportan los cambios en los precios.
                        cmd = cmd.Replace(
                            queryParameter.ParameterName,
                            queryParameter.ParameterName is "@pMayo" or "@pMenu" ? "0" : queryParameter.Value?.ToString());
                    }
                    else {
                        cmd = cmd.Replace(queryParameter.ParameterName, $"'{queryParameter.Value}'");
                    }

                }
                Log.Debug("Se ha obtenido la querry.");

                var exit = QuerysDatabase.SaveQuery(cmd);
                if (exit == 0) throw new Exception();
                Log.Debug("Se ha guardado la querry en sincroniza.");

                var res = query.ExecuteNonQuery();
                Log.Info("Se han actualizado los datos de producto exitosamente.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al actualizar los datos en la tabla producto.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 15: ha ocurrido un error al intentar guardar la información de la base de datos. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
            finally {
                connObj.Close();
            }
        }


        /// <summary>
        /// Método que registra un nuevo producto.
        /// </summary>
        /// <param name="newProduct">Objeto tipo <c>Producto</c>.</param>
        public static int NewProduct(Producto newProduct) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                insert into cat_producto 
                    (ID_PRODUCTO, NOMBRE_PRODUCTO, MEDIDA, TIPO_PRODUCTO, 
                     TIPO_VENTA, DEPENDENCIAS,
                     PRECIO_MAYOREO, PRECIO_MENUDEO, 
                     ESPECIFICACION_PRODUCTO, ACTIVO) 
                values (default, @nom, @medida, @tipoP, 
                        @tipoV, @dependencias,
                        @pMayo, @pMenu, 
                        @esp, @actX)"
            };

            query.Parameters.AddWithValue("@nom", newProduct.NombreProducto);
            query.Parameters.AddWithValue("@medida", newProduct.MedidaProducto);
            query.Parameters.AddWithValue("@piezasX", newProduct.Piezas.ToString());
            query.Parameters.AddWithValue("@tipoP", newProduct.TipoProducto);
            query.Parameters.AddWithValue("@tipoV", newProduct.TipoVenta.ToString());
            query.Parameters.AddWithValue("@pMayo", newProduct.PrecioMayoreo.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@pMenu", newProduct.PrecioMenudeo.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@esp", newProduct.DetallesProducto);
            query.Parameters.AddWithValue("@actX", 1.ToString());
            query.Parameters.AddWithValue("@dependencias", newProduct.DependenciasText);

            try {
                var cmd = query.CommandText;
                for (var index = 0; index < query.Parameters.Count; index++) {
                    var queryParameter = query.Parameters[index];
                    if (queryParameter.ParameterName.Contains("X")) {
                        cmd = cmd.Replace(
                            queryParameter.ParameterName,
                            queryParameter.ParameterName is "@pMayo" or "@pMenu" ? "0" : queryParameter.Value?.ToString());
                    }
                    else {
                        cmd = cmd.Replace(queryParameter.ParameterName, $"'{queryParameter.Value}'");
                    }
                }
                Log.Debug("Se ha obtenido la querry.");

                var exit = QuerysDatabase.SaveQuery(cmd);
                if (exit == 0) throw new Exception();
                Log.Debug("Se ha guardado la querry en sincroniza.");

                var res = query.ExecuteNonQuery(); // retorna el número de columnas cambiadas.
                Log.Info("Se ha dado de alta un nuevo produto exitosamente.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al dar de alta un nuevo producto.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 15: ha ocurrido un error al intentar guardar la información de la base de datos. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
            finally {
                connObj.Close();
            }
        }
    }
}