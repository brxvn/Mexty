using System;
using log4net;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Clase de Database que tiene métodos que necesitan otras clases.
    /// </summary>
    public static class DatabaseHelper {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);


        /// <summary>
        /// Método que obtiene la hora actual en formato Msql friendly
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentTimeNDate(bool allDate=true) {
            var currentTime = DateTime.Now;
            if (allDate) {
                return currentTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else {
                return currentTime.ToString("yyyy-MM-dd_HH-mm-ss");
            }
        }
    }
}