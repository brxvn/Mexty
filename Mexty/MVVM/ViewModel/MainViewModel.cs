﻿using Mexty.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Mexty.MVVM.ViewModel {
    public class MainViewModel : BaseViewModel {

        private BaseViewModel _selectedViewModel;
        public BaseViewModel SelectedViewModel {
            get { return _selectedViewModel; }
            set {
                _selectedViewModel = value;
                OnPropertyChanged(nameof(SelectedViewModel));
            }
        }

        public ICommand UpdateViewCommand { get; set; }

        public MainViewModel() {
            UpdateViewCommand = new UpdateViewCommand(this);
        }
    }
}
