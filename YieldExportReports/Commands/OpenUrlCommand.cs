using System;
using System.Windows.Input;
using YieldExportReports.Utils;

namespace YieldExportReports.Commands
{
    public class OpenUrlCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            //有効かどうか
            if (parameter is not string) return false;

            return !string.IsNullOrWhiteSpace((string)parameter);
        }

        public void Execute(object? parameter)
        {
            AppHelper.OpenLinkProcess(parameter as string);
        }
    }
}
