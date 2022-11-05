using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YieldExportReports.Dock
{
    public class DocumentContent : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Guid m_guid;
        public Guid Guid
        {
            get { return m_guid; }
            set
            {
                if (m_guid == value) return;
                m_guid = value;
                RaisePropertyChanged("Guid");
                RaisePropertyChanged("ContentId");
            }
        }

        [ContentProperty]
        public string ContentId
        {
            get { return m_guid.ToString(); }
            set
            {
                var guid = Guid.Parse(value);
                Guid = guid;
            }
        }

        private string m_title = string.Empty;
        [ContentProperty]
        public string Title
        {
            get { return m_title; }
            set
            {
                if (m_title == value) return;
                m_title = value;
                RaisePropertyChanged("Title");
            }
        }

        public DocumentContent()
            : this(Guid.NewGuid().ToString())
        {
            Title = "Untitled";
        }

        public DocumentContent(string contentId)
        {
            m_guid = Guid.Parse(contentId);
        }
    }
}
