using NLog;
using System.IO;
using System.Windows;

namespace SanGuoCharacterEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string LogFilePath = "logs/editor.log";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private async void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Fatal(e.Exception.ToString);
            string bakLogPath = LogFilePath + $".{DateTime.Now:yyMMdd_HHmmss}.bak";
            File.Copy(LogFilePath, bakLogPath, true);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // 日志配置
            SetLoggingConfiguration();
        }

        private void SetLoggingConfiguration()
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget()
            {
                FileName = LogFilePath,
                Name = "logfile",
                Layout = "[${date:format=MM/dd HH\\:mm\\:ss.fff}] [${level}] [${callsite:className=True:includeNamespace=False:methodName=True}] ${message}",
                KeepFileOpen = false,
                ArchiveOldFileOnStartup = true,
                MaxArchiveFiles = 1,
            };

            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Debug, logfile));

            LogManager.Configuration = config;
        }
    }
}
