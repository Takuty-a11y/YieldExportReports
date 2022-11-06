using AvalonDock;
using System;
using System.Windows.Input;
using YieldExportReports.ViewModels.Main;

namespace YieldExportReports.Commands
{
    public class WindowLoadedCommand : ICommand
    {
        readonly MainWindowViewModel _vm;
        public WindowLoadedCommand(MainWindowViewModel vm)
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
            return parameter is DockingManager;
        }

        public void Execute(object? parameter)
        {
            if (parameter is not null and DockingManager manager)
            {
                _vm.LoadLayout(manager);
            }
        }
    }
}
