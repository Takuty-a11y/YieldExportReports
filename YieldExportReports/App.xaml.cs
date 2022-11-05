using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace YieldExportReports
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex? _isRunning;

        protected override void OnStartup(StartupEventArgs args)
        {
            // 2重起動防止
            _isRunning = new Mutex(true, nameof(YieldExportReports), out bool isFirst);
            if (!isFirst)
            {
                Shutdown();
                return;
            }

            //UI例外
            this.DispatcherUnhandledException += (s, e) =>
            {
                if (e != null)
                {
                    const string dispFormat = @"例外が{0}で発生。プログラムを継続しますか？エラーメッセージ：{1}";
                    string errorMember = e.Exception.TargetSite?.Name ?? string.Empty;
                    string errorMessage = e.Exception.Message;
                    string message = string.Format(dispFormat, errorMember, errorMessage);
                    MessageBoxResult result
                      = MessageBox.Show(message, "DispatcherUnhandledException",
                                        MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                        e.Handled = true;
                }
            };
            //非同期例外
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                if (e != null)
                {
                    const string unoFormat = @"例外がバックグラウンドタスクの{0}で発生。プログラムを継続しますか？エラーメッセージ：{1}";
                    string errorMember = e.Exception.InnerException?.TargetSite?.Name ?? string.Empty;
                    string errorMessage = e.Exception.InnerException?.Message ?? string.Empty;
                    string message = string.Format(unoFormat, errorMember, errorMessage);
                    MessageBoxResult result
                      = MessageBox.Show(message, "UnobservedTaskException",
                                        MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                        e.SetObserved();
                }
            };

            //未処理例外
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is not Exception exception)
                {
                    MessageBox.Show("System.Exceptionとして扱えない例外");
                    return;
                }

                //ログ出力
                const string reportFormat =
                    "===========================================================\r\n" +
                    "ERROR Date = {0}, Sender = {1}, \r\n" +
                    "{2}\r\n\r\n";

                var reportText = string.Format(reportFormat, DateTimeOffset.Now, sender, exception);
                File.AppendAllText("OccuredException.log", reportText);

                //例外メッセージ出力
                if (exception != null)
                {
                    const string msgtFormat = @"例外が{0}で発生。プログラムは終了します。エラーメッセージ：{1}";
                    string errorMember = exception.TargetSite?.Name ?? string.Empty;
                    string errorMessage = exception.Message;
                    string message = string.Format(msgtFormat, exception.TargetSite?.Name, exception.Message);
                    MessageBox.Show(message, "UnhandledException",
                                    MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                Shutdown();
            };

            base.OnStartup(args);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // リソース開放
            _isRunning?.ReleaseMutex();
            _isRunning?.Close();
            GC.Collect();

            base.OnExit(e);
        }
    }
}
