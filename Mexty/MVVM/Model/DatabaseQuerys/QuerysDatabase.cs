using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Xps;
using log4net;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Consutlas a base de datos y metodos del modulo de admin de Base de datos.
    /// </summary>
    public static class QuerysDatabase {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Nombre del ultimo archivo de delta que se ingreso al programa.
        /// </summary>
        private static string LastImportName { get; set; }

        public static string GetLastImportName() {
            return LastImportName;
        }

        /// <summary>
        /// Struct que contiene la querry y la fecha en la que se hizo.
        /// </summary>
        private struct Deltas {
            public string Query;
            public DateTime Date;
        }

        /// <summary>
        /// Método que vacia los contenidos de la tabla control_sincbd a un script sql para ser aplicados en otra base de datos.
        /// </summary>
        public static void DumpDeltas() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            // Obtenemos las querys y las fechas de cada una
            MySqlCommand query1 = new() {
                Connection = connObj,
                CommandText = @"select QUERY, fecha_sinc from control_sincbd;"
            };

            // Vaciamos la tabla.
            MySqlCommand query2 = new() {
                Connection = connObj,
                CommandText = @"truncate table control_sincbd;"
            };

            // Reseteamos el auto increment del id
            MySqlCommand query3 = new() {
                Connection = connObj,
                CommandText = @"ALTER TABLE control_sincbd AUTO_INCREMENT = 1"
            };

            try {
                // query1
                var data = new List<Deltas>();
                using var reader = query1.ExecuteReader();
                while (reader.Read()) {
                    var delta = new Deltas {
                        Query = reader.GetString("query"),
                        Date = reader.GetDateTime("fecha_sinc")
                    };
                    data.Add(delta);
                }
                Log.Debug("Se ha creado la lista de deltas con exito.");

                if (data.Count == 0) {
                    MessageBox.Show("Error: La lista de cambios esta vacia, intenta después de hacer algunos cambios.");
                    Log.Warn("Se ha intentado hacer un export de los cambios con la tabla de control_sincbd vacia.");
                    return;
                }

                var date = DateTime.Today;
                var fileName = $"DBChangesTienda-{DatabaseInit.GetIdTiendaIni().ToString()}-From_{data[0].Date:yyyy-MM-dd_HH-mm-ss}_to_{DatabaseHelper.GetCurrentTimeNDate(false)}.sql";
                var path = $@"C:\Mexty\Backups\{date:yyyy-MMMM}\";
                Directory.CreateDirectory(path);
                var file = $"{path}{fileName}";

                if (!File.Exists(file)) {
                    using (var streamWriter = File.CreateText(file)) {
                        for (var index = 0; index < data.Count; index++) {
                            var delta = data[index];
                            var line = $"{delta.Query} -- {delta.Date}";
                            streamWriter.WriteLine(line);
                        }
                    }
                }
                Log.Info("Se ha creado y escrito el archivo con los deltas con exito.");
                MessageBox.Show($"Se han exportado los cambios exitosamente al archivo {file}");

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener y escribir los deltas en el archivo.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 16: ha ocurrido un error al obtener y escribir los cambios en el archivo. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }

            try {
                // query2 y 3
                query2.ExecuteNonQuery();
                Log.Info("Se ha vaciado la tabla de control_sincbd con exito.");
                query3.ExecuteNonQuery();
                Log.Info("Se ha reseteado el id de contorl_sincbd con exito.");
                MessageBox.Show("Se han terminado de exportar los cambios exitosamente.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al vaciar y resetear el id de la tabla control_sincbd.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 16.5: ha ocurrido un error al vaciar y resetear la tabla de deltas. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Método que recibe un objeto tipo MySqlCommand, obtiene la querry y remplaza los parametros para guardarse.
        /// </summary>
        /// <param name="query">Objeto tipo <c>MySqlCommand</c>.</param>
        /// <exception cref="Exception"></exception>
        public static void ProcessQuery(MySqlCommand query) {
            try {
                var cmd = query.CommandText;
                for (var index = 0; index < query.Parameters.Count; index++) {
                    var queryParameter = query.Parameters[index];

                    if (queryParameter.ParameterName.Contains("X")) { // Campos numericos no llevan comillas y llevan X en el nombre.
                        cmd = cmd.Replace(queryParameter.ParameterName, queryParameter.Value?.ToString());
                    }
                    else {
                        cmd = cmd.Replace(queryParameter.ParameterName, $"'{queryParameter.Value}'");
                    }
                }
                Log.Debug("Se ha obtenido la querry.");

                var exit = SaveQuery(cmd);
                if (exit == 0) throw new Exception();
                Log.Debug("Se ha guardado la querry en sincroniza.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener la query y replazar los parametros de esta.");
                Log.Error($"Error: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Método que guarda la querry dada en la tabla de control_sincbd
        /// </summary>
        /// <returns></returns>
        public static int SaveQuery(string cmd) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            MySqlCommand query = new() {
                Connection = connObj,
                CommandText = @"insert into control_sincbd
                                values (default, @query, @date)"
            };

            // String sanitization
            var strReader = new StringReader(cmd);
            var cmdNew = "";
            while (true) {
                var line = strReader.ReadLine();
                if (line != null) {
                    cmdNew = $"{cmdNew}{line.TrimStart()}";
                }
                else {
                    cmdNew = $"{cmdNew};";
                    break;
                }
            }

            query.Parameters.AddWithValue("@query", cmdNew);
            query.Parameters.AddWithValue("@date", DatabaseHelper.GetCurrentTimeNDate());

            try {
                var res = query.ExecuteNonQuery();
                Log.Info("Se ha guardado la query exitosamente.");
                return res;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al guardar la query.");
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
        /// Metodo a usar para exportar toda la base de datos.
        /// </summary>
        // info: https://github.com/MySqlBackupNET/MySqlBackup.Net/wiki
        public static bool BackUpBd() {
            Log.Info("Se ha empezado el backUp de la base de datos.");
            try {
                var time = DateTime.Today;
                var fileName = $"FullBackupBD{time:dd-MM-yy}.sql";
                const string path = @"C:\Mexty\Backups\FullBackUp\";
                Directory.CreateDirectory(path);
                var file = $"{path}{fileName}";
                using (MySqlConnection conn = new MySqlConnection(IniFields.GetConnectionString())) {
                    using (MySqlCommand cmd = new MySqlCommand()) {
                        using (MySqlBackup mb = new MySqlBackup(cmd)) {
                            cmd.Connection = conn;
                            conn.Open();

                            var customQuery = new Dictionary<string, string> {
                                { "cat_producto", "select * from cat_producto;" },
                                { "cat_rol_usuario", "select * from cat_rol_usuario;" },
                                { "cat_tienda", "select * from cat_tienda;" },
                                { "cliente_mayoreo", "select * from cliente_mayoreo;" },
                                { "control_imports", "select * from control_imports;" },
                                { "control_sincbd", "select * from control_sincbd where ID_REGISTRO = 0;" }, // Nada de deltas.
                                { "inventario", "select * from inventario;" },
                                { "inventario_matriz", "select * from inventario_matriz;" },
                                { "movimientos_clientes", "select * from movimientos_clientes;" },
                                { "movimientos_inventario", "select * from movimientos_inventario;" },
                                { "usuario", "select * from usuario;" },
                                { "venta_mayoreo", "select * from venta_mayoreo;" },
                                { "venta_menudeo", "select * from venta_menudeo;" }
                            };
                            mb.ExportInfo.TablesToBeExportedDic = customQuery;

                            mb.ExportToFile(file);
                            Log.Debug("Se ha exportado el archivo exitosamente.");
                            conn.Close();
                        }
                    }
                }

                MessageBox.Show($"Se ha creado el archivo {file}");
                return true;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al intentar exportar la base de datos.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 17: ha ocurrido un error al intentar exportar toda la base de datos. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Método que se encarga de exportar la base de datos limpia para una nueva sucursal.
        /// </summary>
        /// <returns></returns>
        public static bool ExportNewTienda() {
            Log.Info("Se ha empezado el proceso de exportar una base de datos limpia para una nueva sucursal.");
            try {
                var time = DateTime.Today;
                var fileName = $"FullBackupBDNuevaTienda{time:dd-MM-yy}.sql";
                const string path = @"C:\Mexty\Backups\Scripts_Nueva_Tienda\";
                Directory.CreateDirectory(path);
                var file = $"{path}{fileName}";
                using (MySqlConnection conn = new MySqlConnection(IniFields.GetConnectionString())) {
                    using (MySqlCommand cmd = new MySqlCommand()) {
                        using (MySqlBackup mb = new MySqlBackup(cmd)) {
                            cmd.Connection = conn;
                            conn.Open();

                            var customQuery = new Dictionary<string, string> {
                                { "cat_producto", "select * from cat_producto;" },
                                { "cat_rol_usuario", "select * from cat_rol_usuario;" },
                                { "cat_tienda", "select * from cat_tienda;" },
                                { "cliente_mayoreo", "select * from cliente_mayoreo;" },
                                { "control_imports", "select * from control_imports;" },
                                { "control_sincbd", "select * from control_sincbd where ID_REGISTRO = 0;" }, // Nada de deltas.
                                { "inventario", "select * from inventario where ID_REGISTRO = 0;" }, // Nada de inventario
                                { "inventario_matriz", "select * from inventario_matriz where ID_REGISTRO = 0;" }, // Nada de inventario Matriz.
                                { "movimientos_clientes", "select * from movimientos_clientes where ID_REGISTRO = 0;" }, // Nada de mov clientes.
                                { "movimientos_inventario", "select * from movimientos_inventario where ID_REGISTRO = 0;" }, // Nada de mov inventario.
                                { "usuario", "select * from usuario;" },
                                { "venta_mayoreo", "select * from venta_mayoreo where ID_VENTA_MAYOREO = 0;" }, // Nada de ventas mayoreo.
                                { "venta_menudeo", "select * from venta_menudeo where ID_VENTA_MENUDEO = 0;" } // Nada de ventas menudeo.
                            };
                            mb.ExportInfo.TablesToBeExportedDic = customQuery;

                            mb.ExportToFile(file);
                            Log.Debug("Se ha exportado el archivo exitosamente.");
                            conn.Close();
                        }
                    }
                }

                MessageBox.Show($"Se ha creado el archivo {file}");
                return true;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al intentar exportar la base de datos.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 17: ha ocurrido un error al intentar exportar toda la base de datos. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }


        /// <summary>
        /// Método que verifica si hay deltas en la bd.
        /// </summary>
        /// <returns></returns>
        public static bool CheckDeltas() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            MySqlCommand query = new() {
                Connection = connObj,
                CommandText = @"select * from control_sincbd"
            };

            try {
                using var reader = query.ExecuteReader();
                return reader.HasRows;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al checar si la tabla de Deltas tiene datos.");
                Log.Error($"{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Importa un archivo SQL para ejecutarlo
        /// </summary>
        public static bool Import(string file) {
            Log.Info("Se ha empezado el proceso de Importar un archivo SQL.");
            const string path = @"C:\Mexty\Backups\ErrorLogDb\";
            Directory.CreateDirectory(path);

            try {
                if (file.Contains("FullBackupBD")) {
                    // solo BackUps de toda la base de datos.
                    Log.Debug("Detecatado archivo full backup.");
                    using (MySqlConnection conn = new MySqlConnection(IniFields.GetConnectionString())) {
                        using (MySqlCommand cmd = new MySqlCommand()) {
                            using (MySqlBackup mb = new MySqlBackup(cmd)) {
                                cmd.Connection = conn;
                                conn.Open();
                                mb.ImportInfo.ErrorLogFile = $"{path}errors.log";
                                mb.ImportFromFile(file);
                                Log.Debug("Se ha importado el archivo Exitosamente.");
                                conn.Close();
                            }
                        }
                    }

                    return true;
                }
                else {
                    Log.Debug("Detecatado archivo de sincronización.");
                    return ExecFromScript(file);
                }
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al intentar importar y ejecutar el archivo SQL.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 18: ha ocurrido un error al intentar importar y ejecutar el archivo SQL. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Funcion que lee y ejecuta comandos sql de un script.
        /// </summary>
        /// <param name="file">Ruta al archivo .sql a ejecutar.</param>
        /// <returns></returns>
        private static bool ExecFromScript(string file) {
            var numberLine = 0;

            var name = file.Split('\\').Last(); // obtengo el nombre del archivo.
            if (!Validate(name)) return false; // valido que el archivo sea valido.
            NewImportFile(name); // si es valido lo guardo en la bd.

            try {
                var connObj = new MySqlConnection(IniFields.GetConnectionString());
                connObj.Open();

                var script = new StreamReader(file);

                while (true) {
                    var line = script.ReadLine();
                    if (line != null) {
                        MySqlCommand query = new() {
                            Connection = connObj,
                            CommandText = line
                        };
                        query.ExecuteNonQuery();
                        numberLine += 1;
                    }
                    else {
                        break;
                    }
                }
                return true;
            }
            catch (Exception e) {
                Log.Error($"Ha cocurrido un error al ejecutar el script sql en la linea {numberLine}.");
                Log.Error($"Error: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Método que valida que el archivo de configuración sea valido
        /// </summary>
        /// <param name="name"></param>
        /// <returns>False si el archivo de configuración no es valido.</returns>
        private static bool Validate(string name) {
            if (ValidateIdTienda(name)) {
                MessageBox.Show(
                    $"Esta intentando importar un archivo de sincronización generado por esta misma tienda, lo cual puede causar incoherencias en la Base de datos.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
            return !CheckFile(name);
        }

        /// <summary>
        ///  Método que valida que no se metan los scripts de la misma tienda.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>True si el script fue generado por la tienda actual.</returns>
        private static bool ValidateIdTienda(string file) {
            var id = file.Split('-')[1];
            return int.Parse(id) == DatabaseInit.GetIdTiendaIni();
        }

        /// <summary>
        /// Método que verifica que el script no se haya ejecutado antes.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true si encuentra que el archivo ya fue importado.</returns>
        private static bool CheckFile(string file) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            MySqlCommand query = new() {
                Connection = connObj,
                CommandText = @"select FECHA from control_imports where NOMBRE=@file"
            };
            query.Parameters.AddWithValue("@file", file);

            try {
                var res = query.ExecuteReader();
                if (res.HasRows) {
                    // Encontro el archivo.
                    res.Read();
                    var date = res.GetString("fecha");

                    MessageBox.Show(
                        $"Error: Este archivo de sincronización ya fue importado el {date} importarlo otra vez causara incoherencias en la base de datos.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return true;
                }
                return false;
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
            finally {
                connObj.Close();
            }
        }

        /// <summary>
        /// Método que da de alta un nuevo registro en la tabla de control_imports.
        /// </summary>
        /// <returns>El número de filas afectadas.</returns>
        private static int NewImportFile(string file) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            MySqlCommand query = new() {
                Connection = connObj,
                CommandText = @"insert into control_imports
                                values (default, @file, @date)"
            };

            query.Parameters.AddWithValue("@file", file);
            query.Parameters.AddWithValue("@date", DatabaseHelper.GetCurrentTimeNDate());

            try {
                var res = query.ExecuteNonQuery();
                Log.Info("Se ha guardado la query exitosamente.");
                return res;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al guardar el nombe de el archivo de sincronización.");
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