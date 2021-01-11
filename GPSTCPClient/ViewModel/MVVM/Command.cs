﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GPSTCPClient.ViewModel.MVVM
{
    internal sealed class Command : ICommand
    {
        private readonly Action<object> _action;

        public Command(Action<object> action) => _action = action;

        public void Execute(object parameter) => _action(parameter);

        public bool CanExecute(object parameter) => true;

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}
