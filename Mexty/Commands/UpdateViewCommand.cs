using Mexty.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mexty.Commands {
    public class UpdateViewCommand : ICommand {
        public MainViewModel viewModel;

        public UpdateViewCommand(MainViewModel viewModel) {
            this.viewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            if (parameter.ToString() == "Admin") {
                viewModel.SelectedViewModel = new AdminViewModel();
            }
            else if (parameter.ToString() == "Ventas") {
                viewModel.SelectedViewModel = new VentasViewModel();
            }
            else if (parameter.ToString() == "Inventario") {
                viewModel.SelectedViewModel = new InventarioViewModel();
            }
            else if (parameter.ToString() == "Reportes") {
                viewModel.SelectedViewModel = new ReportesViewModel();
            }

        }
    }
}
