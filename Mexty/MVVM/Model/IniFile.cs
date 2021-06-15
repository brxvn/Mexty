
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Mexty.MVVM.Model {
    /// <summary>
    /// Clase que se encarga de leer y escribir en el archivo ini.
    /// </summary>
    public class IniFile { 
        string Path;
        //string EXE = Assembly.GetExecutingAssembly().GetName().Name;
        string EXE = "Mexty";

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string section, string key, string defaultS, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// Constructor de la clase IniFile.
        /// </summary>
        /// <param name="iniPath">Ruta donde se va a guardar el archivo ini.</param>
        public IniFile(string iniPath = null) {
            Path = new FileInfo(iniPath ?? EXE + ".ini").FullName; //TODO: hacer que si no existen los campos los cree junto con el archivo.
        }

        /// <summary>
        /// Función para leer del archivo ini.
        /// </summary>
        /// <param name="key">Nombre del campo a leer.</param>
        /// <param name="section">Sección del campo a leer (opcional).</param>
        /// <returns></returns>
        public string Read(string key, string section = null) {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(section ?? EXE, key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        /// <summary>
        /// Función para escribir al archivo ini.
        /// </summary>
        /// <param name="key">Nombre del campo a escribir</param>
        /// <param name="value">Valor a escribir en el campo.</param>
        /// <param name="section">Sección donde se encuentra el campo (opcional).</param>
        public void Write(string key, string value, string section = null) {
            WritePrivateProfileString(section ?? EXE, key, value, Path);
        }

        public void DeleteKey(string key, string section = null) {
            Write(key, null, section ?? EXE);
        }

        public void DeleteSection(string section = null) {
            Write(null, null, section ?? EXE);
        }

        /// <summary>
        /// Función que verifica si un campo del archivo ini existe.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool KeyExists(string key, string section = null) {
            return Read(key, section).Length > 0;
        }
    }
}