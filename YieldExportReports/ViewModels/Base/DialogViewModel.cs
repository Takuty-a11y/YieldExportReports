using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using YieldExportReports.Dock;

namespace YieldExportReports.ViewModels.Base
{
    public abstract class DialogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 親Window
        /// </summary>
        public Window? Owner { get; set; }

        /// <summary>
        /// DialogResult
        /// </summary>
        public bool? ViewModelResult
        {
            get { return m_viewModelResult; }
            set
            {
                if (value != m_viewModelResult)
                {
                    m_viewModelResult = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? m_viewModelResult;

        /// <summary>
        /// OKボタンコマンド
        /// </summary>
        public ICommand OKCommand
        {
            get
            {
                if (m_okCommand == null)
                {
                    m_okCommand = new RelayCommand(() =>
                    {
                        var bRet = OKCommandMethod();

                        if (bRet) ViewModelResult = true;
                    });
                }
                return m_okCommand;
            }
        }
        private RelayCommand? m_okCommand;
        protected virtual bool OKCommandMethod() => true;

        /// <summary>
        /// キャンセルボタンコマンド
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                if (m_cancelCommand == null)
                {
                    m_cancelCommand = new RelayCommand(() =>
                    {
                        var bRet = CancelCommandMethod();

                        if (bRet) ViewModelResult = false;
                    });
                }
                return m_cancelCommand;
            }
        }
        private RelayCommand? m_cancelCommand;
        protected virtual bool CancelCommandMethod() => true;
    }
}
