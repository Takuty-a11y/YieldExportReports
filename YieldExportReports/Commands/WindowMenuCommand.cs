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
                        _vm.CreateNewFile();
                        return;
                    case "Open":
                        _vm.ChangeDBConnection();
                        return;
                    case "Save":
                        _vm.SaveFile();
                        return;
                    case "SaveAs":
                        _vm.SaveFileAsNew();
                        return;
                    case "Close":
                        _vm.CloseFile();
                        return;
                    case "End":
                        Application.Current.Shutdown();
                        return;
                    case "Query":
                        _vm.ActiveDocument = _vm.QueryEditorViewModel;
                        _vm.GetDataFromTool();
                        return;
                    case "OutputXML":
                        _vm.DataGridViewModel.XMLOutCommand.Execute(null);
                        return;
                    case "OutputJSON":
                        _vm.DataGridViewModel.JSONOutCommand.Execute(null);
                        return;
                    case "OutputCSV":
                        _vm.DataGridViewModel.CSVOutCommand.Execute(null);
                        return;
                    case "OutputEXCEL":
                        _vm.DataGridViewModel.ExcelOutCommand.Execute(null);
                        return;
                    case "OutputReport":
                        _vm.ExportReportFromTool();
                        break;
                    case "ChangeDB":
                        _vm.ChangeDBConnection();
                        break;
                }
            }
        }
    }
}
