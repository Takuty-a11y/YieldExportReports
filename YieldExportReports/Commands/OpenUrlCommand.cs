using MahApps.Metro.Controls.Dialogs;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace YieldExportReports.Commands
{
    public class OpenUrlCommand : ICommand
    {
        private object _vm;
        public OpenUrlCommand(object vm)
        {
            _vm = vm;
        }

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
            OpenUrlLink(parameter as string);
        }

        private async void OpenUrlLink(string? link)
        {
            try
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = link ?? throw new ArgumentNullException(nameof(link)),
                        UseShellExecute = true,
                    }
                );
            }
            catch (Exception e)
            {
                await DialogCoordinator.Instance.ShowMessageAsync(_vm, "Error", e.Message);
            }
        }
    }
}
