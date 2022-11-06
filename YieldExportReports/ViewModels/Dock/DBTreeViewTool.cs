using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using YieldExportReports.Database.DBLibraries;
using YieldExportReports.Database.DBObjects;
using YieldExportReports.Dock;

namespace YieldExportReports.ViewModels.Dock
{
    public sealed class DBTreeViewTool : ToolContent
    {
        public DBTreeViewTool() : base(string.Empty) { }
        public DBTreeViewTool(string contentId, string? title = null)
            : base(contentId, title)
        {
        }

        /// <summary>
        /// 選択テーブルのデータ取得コマンド
        /// </summary>
        public ICommand SelectTableCommand
        {
            get
            {
                if (m_selectTableCommand == null)
                {
                    m_selectTableCommand = new RelayCommand(() =>
                    {
                        var tableName = GetSelectedItem()?.Name ?? string.Empty;
                        SetSelectQuery?.Invoke($"SELECT * FROM {tableName}");
                    });
                }
                return m_selectTableCommand;
            }
        }
        private RelayCommand? m_selectTableCommand;
        internal Action<string>? SetSelectQuery { get; set; }

        /// <summary>
        /// 名称コピーコマンド
        /// </summary>
        public ICommand NameCopyCommand
        {
            get
            {
                if (m_nameCopyCommand == null)
                {
                    m_nameCopyCommand = new RelayCommand(() =>
                    {
                        var tableName = GetSelectedItem()?.Name ?? string.Empty;
                        Clipboard.SetText(tableName);
                    });
                }
                return m_nameCopyCommand;
            }
        }
        private RelayCommand? m_nameCopyCommand;

        /// <summary>
        /// DB接続変更コマンド
        /// </summary>
        public ICommand ConnectCommand
        {
            get
            {
                if (m_connectCommand == null)
                {
                    m_connectCommand = new RelayCommand(() =>
                    {
                        DBReConnect?.Invoke();
                    });
                }
                return m_connectCommand;
            }
        }
        private RelayCommand? m_connectCommand;
        internal Action? DBReConnect { get; set; }

        /// <summary>
        /// 最新の情報取得コマンド
        /// </summary>
        public ICommand LatestInfoCommand
        {
            get
            {
                if (m_latestInfoCommand == null)
                {
                    m_latestInfoCommand = new RelayCommand(() =>
                    {
                        GetLatestDBInfo?.Invoke();
                    });
                }
                return m_latestInfoCommand;
            }
        }
        private RelayCommand? m_latestInfoCommand;
        internal Action? GetLatestDBInfo { get; set; }

        /// <summary>
        /// 全て展開コマンド
        /// </summary>
        public ICommand ExpandAllCommand
        {
            get
            {
                if (m_expandAllCommand == null)
                {
                    m_expandAllCommand = new RelayCommand(() =>
                    {
                        SwitchAllTreeViewItem(true);
                    });
                }
                return m_expandAllCommand;
            }
        }
        private RelayCommand? m_expandAllCommand;

        /// <summary>
        /// 全て折りたたみコマンド
        /// </summary>
        public ICommand CollapseAllCommand
        {
            get
            {
                if (m_collapseAllCommand == null)
                {
                    m_collapseAllCommand = new RelayCommand(() =>
                    {
                        SwitchAllTreeViewItem(false);
                    });
                }
                return m_collapseAllCommand;
            }
        }
        private RelayCommand? m_collapseAllCommand;

        /// <summary>
        /// 右クリックメニュー表示切替コマンド
        /// </summary>
        public ICommand ContextChangeCommand
        {
            get
            {
                if (m_contextChangeCommand == null)
                {
                    m_contextChangeCommand = new RelayCommand(() =>
                    {
                        var target = GetSelectedItem()
                            ?? throw new ArgumentNullException("SelectedItem");

                        if (target.Type != DBObjectType.Table)
                        { SelectTableVisible = Visibility.Collapsed; }
                        else
                        { SelectTableVisible = Visibility.Visible; }
                    });
                }
                return m_contextChangeCommand;
            }
        }
        private RelayCommand? m_contextChangeCommand;

        /// <summary>
        /// 右クリックメニュー[データ選択]の表示
        /// </summary>
        public Visibility SelectTableVisible
        {
            get { return m_selectTableVisible; }
            set
            {
                if (value != m_selectTableVisible)
                {
                    m_selectTableVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Visibility m_selectTableVisible;

        /// <summary>
        /// データベースツリーデータ
        /// </summary>
        public DBObjectCollection? DbObjectCollection
        {
            get { return m_dbObjectCollection; }
            set
            {
                if (value != m_dbObjectCollection)
                {
                    m_dbObjectCollection = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private DBObjectCollection? m_dbObjectCollection;

        /// <summary>
        /// 選択中の項目を取得します
        /// </summary>
        /// <returns></returns>
        private DBObject? GetSelectedItem()
        {
            DBObject? targetItem = null;
            if (m_dbObjectCollection == null)
            { return null; }

            foreach (var item in m_dbObjectCollection)
            {
                if (item.IsSelected)
                {
                    targetItem = item;
                    break;
                }
                SearchSelectedItem(item.Children, ref targetItem);
            }
            return targetItem;
        }
        private void SearchSelectedItem(List<DBObject> dBObjects, ref DBObject? dBObject)
        {
            if (dBObjects == null) return;
            foreach (var item in dBObjects)
            {
                if (item.IsSelected)
                {
                    dBObject = item;
                    break;
                }
                SearchSelectedItem(item.Children, ref dBObject);
            }
        }

        /// <summary>
        /// 全てのノードで展開/折りたたみを切り替えます
        /// </summary>
        /// <param name="dBObjects">子ノード</param>
        /// <param name="pbExpand">[True]展開[False]折りたたみ</param>
        private void SwitchAllTreeViewItem(bool pbExpand)
        {
            var tmpCollection = m_dbObjectCollection;
            if (tmpCollection == null) return;

            foreach (var item in tmpCollection)
            {
                item.IsExpanded = pbExpand;
                SwitchExpandForChildren(item.Children, pbExpand);
            }
            DbObjectCollection = null;
            DbObjectCollection = tmpCollection;
        }
        private void SwitchExpandForChildren(List<DBObject> dBObjects, bool pbExpand)
        {
            if (dBObjects == null) return;
            foreach (var item in dBObjects)
            {
                item.IsExpanded = pbExpand;
                SwitchExpandForChildren(item.Children, pbExpand);
            }
        }

    }
}
