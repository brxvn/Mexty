using Mexty.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mexty.MVVM.ViewModel {
    class MainViewModel : ObservableObject {

        public AdminViewModel AdmnVm { get; set; }

        private object _currenView;

        public object CurrentView {
            get { return _currenView; }
            set { _currenView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel() {
            AdmnVm = new AdminViewModel();
            CurrentView = AdmnVm;

        }
    }
}
