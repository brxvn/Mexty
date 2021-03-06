using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using log4net;

namespace Mexty {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        protected override void OnStartup(StartupEventArgs e) {
              log4net.Config.XmlConfigurator.Configure();
              log.Info("        =============  Started Logging  =============        ");
              base.OnStartup(e);
          }
    }
}
