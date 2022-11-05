using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YieldExportReports.ViewModels.Main;

namespace YieldExportReports.Commands
{
    public class WindowMenuCommand : ICommand
    {
        MainWindowViewModel _vm;
        public WindowMenuCommand(MainWindowViewModel vm)
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
            return parameter is MenuItem;
        }

        public void Execute(object? parameter)
        {
            if (parameter is MenuItem menuCtrl)
            {
                if (!menuCtrl.Name.Contains("menu")) return;

                switch (menuCtrl.Name.Replace("menu", ""))
                {
                    case "New":
                        CreateNewProject();
                        return;
                    case "Open":
                        OpenProject();
                        return;
                    case "Save":
                        SaveProject();
                        return;
                    case "SaveAs":
                        SaveProjectAsNew();
                        return;
                    case "Close":
                        CloseProject();
                        return;
                    case "End":
                        CloseApplication();
                        return;
                }
            }
        }

        private void CreateNewProject() => _vm.CreateNewFile();
        private void OpenProject() => _vm.ChangeDBConnection();
        private void SaveProject() => _vm.SaveFile();
        private void SaveProjectAsNew() => _vm.SaveFileAsNew();
        private void CloseProject() => _vm.CloseFile();
        private void CloseApplication() => Application.Current.Shutdown();
    }
}
