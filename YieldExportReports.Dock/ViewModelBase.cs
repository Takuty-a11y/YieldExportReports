using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace YieldExportReports.Dock
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        //ObservableCollection<ToolContent> m_documents;
        public ObservableCollection<ToolContent> Documents
        {
            get
            {
                return new ObservableCollection<ToolContent>(Tools.Where(x => x.IsDocument));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ToolContent m_activeDocument = new(string.Empty);
        public ToolContent ActiveDocument
        {
            get
            {
                return m_activeDocument;
            }
            set
            {
                if (m_activeDocument == value) return;
                m_activeDocument = value;
                NotifyPropertyChanged(nameof(ActiveDocument));
            }
        }

        ObservableCollection<ToolContent>? m_tools;
        public ObservableCollection<ToolContent> Tools
        {
            get
            {
                if (m_tools == null)
                { m_tools = new ObservableCollection<ToolContent>(); }
                return m_tools;
            }
        }

        protected abstract void InitializeTools();

        protected ToolContent GetDocumentByContentId(string contentId)
        {
            var doc = Documents.FirstOrDefault(d => d.ContentId == contentId);
            if (doc == null)
            { return new ToolContent(string.Empty); }
            else
            { return doc; }
        }
        protected ToolContent GetContentByContentId(string contentId)
        {
            var doc = Tools.FirstOrDefault(d => d.ContentId == contentId);
            if (doc == null)
            { return new ToolContent(string.Empty); }
            else
            { return doc; }
        }

        RelayCommand m_newDocumentCommand = new(null);
        public ICommand NewDocumentCommand
        {
            get
            {
                if (m_newDocumentCommand == null)
                {
                    m_newDocumentCommand = new RelayCommand(() =>
                    {
                        var document = NewDocument();
                        Documents.Add(document);
                    });
                }
                return m_newDocumentCommand;
            }
        }

        public virtual ToolContent NewDocument()
        {
            return new ToolContent(string.Empty);
        }

        #region Layout
        string LayoutFile
        {
            get
            {
                return Path.ChangeExtension(
                    Environment.GetCommandLineArgs()[0]
                    , ".AvalonDock.config"
                    );
            }
        }
        byte[]? m_defaultLayout;

        public void DefaultLayout(DockingManager dockManager)
        {
            if (m_defaultLayout == null) return;
            LoadLayoutFromBytes(dockManager, m_defaultLayout);
        }

        public void LoadLayout(DockingManager dockManager)
        {
            // backup default layout
            using (var ms = new MemoryStream())
            {
                var serializer = new XmlLayoutSerializer(dockManager);
                serializer.Serialize(ms);
                m_defaultLayout = ms.ToArray();
            }

            // load file
            byte[] bytes;
            try
            {
                bytes = File.ReadAllBytes(LayoutFile);
            }
            catch
            {
                return;
            }

            // restore layout
            LoadLayoutFromBytes(dockManager, bytes);
        }

        bool LoadLayoutFromBytes(DockingManager dockManager, byte[] bytes)
        {
            InitializeTools();

            var serializer = new XmlLayoutSerializer(dockManager);

            serializer.LayoutSerializationCallback += MatchLayoutContent;

            try
            {
                using (var stream = new MemoryStream(bytes))
                {
                    serializer.Deserialize(stream);
                }

                RestoreDocumentsFromBytes(bytes);

                return true;
            }
            catch
            {
                return false;
            }
        }

        void MatchLayoutContent(object? o, LayoutSerializationCallbackEventArgs e)
        {
            var contentId = e.Model.ContentId;

            if (e.Model is LayoutAnchorable)
            {
                // Tool Windows
                foreach (var tool in Tools)
                {
                    if (tool.ContentId == contentId)
                    {
                        e.Content = tool;
                        return;
                    }
                }

                // Unknown
                Console.WriteLine(new Exception("unknown ContentID: " + contentId));
                return;
            }

            if (e.Model is LayoutDocument)
            {
                // load済みを探す
                foreach (var document in Documents)
                {
                    if (document.ContentId == contentId)
                    {
                        e.Content = document;
                        return;
                    }
                }

                // Document
                {
                    var document = NewDocument();
                    Documents.Add(document);
                    document.ContentId = contentId;
                    e.Content = document;
                }

                return;
            }

            Console.WriteLine(new Exception("Unknown Model: " + e.Model.GetType()));
            return;
        }

        protected virtual void ModifySerializedXml(XmlDocument doc)
        {

        }
        protected virtual void RestoreDocumentsFromBytes(Byte[] bytes)
        {

        }

        public void SaveLayout(DockingManager dockManager)
        {
            var serializer = new XmlLayoutSerializer(dockManager);
            var doc = new XmlDocument();
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream);
                stream.Position = 0;
                doc.Load(stream);
            }

            //ModifySerializedXml(doc);

            using (var stream = new FileStream(LayoutFile, FileMode.Create))
            {
                doc.Save(stream);
            }
        }
        #endregion
    }
}
