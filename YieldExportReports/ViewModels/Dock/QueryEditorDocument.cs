using ICSharpCode.AvalonEdit.Document;
using System;
using System.Windows.Input;
using System.Windows.Media;
using YieldExportReports.Dock;

namespace YieldExportReports.ViewModels.Dock
{
    public sealed class QueryEditorDocument : ToolContent
    {
        public QueryEditorDocument() : base(string.Empty)
        {
        }
        public QueryEditorDocument(string contentId, string? title = null) : base(contentId, title)
        {
            IsDocument = true;
            ExecEnabled = true;
        }

        /// <summary>
        /// 実行ボタン有効無効
        /// </summary>
        public bool ExecEnabled
        {
            get { return m_execEnabled; }
            set
            {
                if (value != m_execEnabled)
                {
                    m_execEnabled = value;
                    CancelEnabled = !value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(ExecColor));
                }
            }
        }
        private bool m_execEnabled;

        /// <summary>
        /// 実行アイコン色
        /// </summary>
        public Brush ExecColor
        {
            get
            {
                if (m_execEnabled)
                    return new SolidColorBrush(Colors.Red);
                else
                    return new SolidColorBrush(Colors.LightGray);
            }
        }

        /// <summary>
        /// キャンセルボタン有効性
        /// </summary>
        public bool CancelEnabled
        {
            get { return m_cancelEnabled; }
            set
            {
                if (value != m_cancelEnabled)
                {
                    m_cancelEnabled = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(CancelColor));
                }
            }
        }
        private bool m_cancelEnabled;

        /// <summary>
        /// キャンセルアイコン色
        /// </summary>
        public Brush CancelColor
        {
            get
            {
                if (m_cancelEnabled)
                    return new SolidColorBrush(Colors.Red);
                else
                    return new SolidColorBrush(Colors.LightGray);
            }
        }

        /// <summary>
        /// エディタドキュメント
        /// </summary>
        public TextDocument EditorDocument
        {
            get { return m_editorDocument; }
            set
            {
                if (value != m_editorDocument)
                {
                    m_editorDocument = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private TextDocument m_editorDocument = new();

        /// <summary>
        /// クエリ実行コマンド
        /// </summary>
        public ICommand ExecCommand
        {
            get
            {
                if (m_execCommand == null)
                {
                    m_execCommand = new RelayCommand(() =>
                    {
                        GetDataFromQuery?.Invoke(EditorDocument.Text);
                    });
                }
                return m_execCommand;
            }
        }
        private RelayCommand? m_execCommand;
        internal Action<string>? GetDataFromQuery { get; set; }

        /// <summary>
        /// クエリキャンセルコマンド
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                if (m_cancelCommand == null)
                {
                    m_cancelCommand = new RelayCommand(() =>
                    {
                        GetDataFromQueryCancel?.Invoke();
                    });
                }
                return m_cancelCommand;
            }
        }
        private RelayCommand? m_cancelCommand;
        internal Action? GetDataFromQueryCancel { get; set; }
    }
}
