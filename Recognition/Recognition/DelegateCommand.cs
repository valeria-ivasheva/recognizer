﻿using System;
using System.Windows.Input;

namespace Recognition
{
    public class DelegateCommand : ICommand
    {
        private readonly Action command;
        private readonly Func<bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public DelegateCommand(Action command, Func<bool> canExecute = null)
        {
            this.canExecute = canExecute;
            this.command = command ?? throw new ArgumentNullException();
        }

        public void Execute(object parameter)
        {
            command();
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }
    }
}
