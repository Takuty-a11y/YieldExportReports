using System.ComponentModel;
using System.Runtime.CompilerServices;
using YieldExportReports.Database.DBResources;

namespace YieldExportReports.ViewModels.Base
{
    public abstract class DBSettingViewModel : INotifyPropertyChanged
    {
        public DBSettingViewModel()
        {
            InstanceText = string.Empty;
            ServerText = string.Empty;
            DatabaseText = string.Empty;
            TimeoutText = string.Empty;
            UserText = string.Empty;
            PasswordText = string.Empty;
            IsIntegrate = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// データベースインスタンス
        /// </summary>
        public string InstanceText
        {
            get { return m_instanceText; }
            set
            {
                if (value != m_instanceText)
                {
                    m_instanceText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string m_instanceText = string.Empty;

        /// <summary>
        /// サーバー
        /// </summary>
        public string ServerText
        {
            get { return m_serverText; }
            set
            {
                if (value != m_serverText)
                {
                    m_serverText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string m_serverText = string.Empty;

        /// <summary>
        /// データベース
        /// </summary>
        public string DatabaseText
        {
            get { return m_databaseText; }
            set
            {
                if (value != m_databaseText)
                {
                    m_databaseText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string m_databaseText = string.Empty;

        /// <summary>
        /// タイムアウト
        /// </summary>
        public string TimeoutText
        {
            get { return m_timeoutText; }
            set
            {
                if (value != m_timeoutText)
                {
                    m_timeoutText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string m_timeoutText = string.Empty;

        /// <summary>
        /// Windows認証
        /// </summary>
        public bool? IsIntegrate
        {
            get { return m_isIntegrate; }
            set
            {
                m_isIntegrate = value;
                IsUserEnabled = value != true;
                NotifyPropertyChanged();
            }
        }
        private bool? m_isIntegrate;

        /// <summary>
        /// ユーザー/パスワードの有効無効
        /// </summary>
        public bool IsUserEnabled
        {
            get { return m_isUserEnabled; }
            set
            {
                if (value != m_isUserEnabled)
                {
                    m_isUserEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool m_isUserEnabled;

        /// <summary>
        /// ユーザー名
        /// </summary>
        public string UserText
        {
            get { return m_userText; }
            set
            {
                if (value != m_userText)
                {
                    m_userText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string m_userText = string.Empty;

        /// <summary>
        /// パスワード
        /// </summary>
        public string PasswordText
        {
            get { return m_passwordText; }
            set
            {
                if (value != m_passwordText)
                {
                    m_passwordText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string m_passwordText = string.Empty;
        protected abstract void InitializeSetting(DBResource dBResource);
        public abstract DBResource GetDataSource();
    }
}
