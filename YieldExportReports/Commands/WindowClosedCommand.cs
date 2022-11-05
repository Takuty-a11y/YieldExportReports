using AvalonDock;
using System;
using System.Windows.Input;
using YieldExportReports.ViewModels.Main;

namespace YieldExportReports.Commands
{
    public class WindowClosedCommand : ICommand
    {
        MainWindowViewModel _vm;
        public WindowClosedCommand(MainWindowViewModel vm)
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
                _vm.SaveLayout(manager);
            }
        }
    }
}
