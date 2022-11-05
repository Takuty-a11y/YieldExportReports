using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace YieldExportReports.Dock
{
    public class ToolContent : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string m_contentId;
        [ContentProperty]
        public string ContentId
        {
            get { return m_contentId; }
            set
            {
                if (m_contentId == value) return;
                m_contentId = value;
                RaisePropertyChanged("ContentId");
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

        private Visibility m_visiblity;
        [ContentProperty(BindingMode = BindingMode.TwoWay)]
        public Visibility Visibility
        {
            get { return m_visiblity; }
            set
            {
                if (m_visiblity == value) return;
                m_visiblity = value;
                RaisePropertyChanged("Visibility");
                RaisePropertyChanged("IsVisible");
            }
        }

        public bool IsVisible
        {
            get { return m_visiblity == Visibility.Visible; }
            set
            {
                if (IsVisible == value) return;
                if (value)
                {
                    Visibility = Visibility.Visible;
                }
                else
                {
                    Visibility = Visibility.Hidden;
                }
            }
        }
        public bool IsDocument { get; set; }

        public ToolContent(string contentId, string? title = "")
        {
            m_contentId = contentId;
            Title = string.IsNullOrWhiteSpace(title) ? contentId : title;
        }
    }
}
