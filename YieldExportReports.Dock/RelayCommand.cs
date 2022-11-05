using System;
using System.Windows.Input;

namespace YieldExportReports.Dock
{
    public class RelayCommand : ICommand
    {
        private Action? m_callback;

        public RelayCommand(Action? callback)
        {
            m_callback = callback;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public event EventHandler? CanExecuteChanged;

        public void Execute(object? parameter)
        {
            if (m_callback != null)
            { m_callback(); }
        }
    }
}
