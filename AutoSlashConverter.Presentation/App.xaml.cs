using AutoSlashConverter.Framework.Utilities;
using System;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace AutoSlashConverter.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 创建互斥体
            string mutexName = Assembly.GetExecutingAssembly().GetName().Name;
            mutex = new Mutex(true, mutexName, out bool createdNew);

            if (!createdNew)
            {
                //给已启动的程序发送自定义消息（显示并置顶窗口）
                var windowHandle = Win32.FindWindow(null, ViewModels.MainWindowViewModel.Title);
                Win32.SendMessage(windowHandle, Views.MainWindow.WM_RESTOREWINDOW, IntPtr.Zero, IntPtr.Zero);
                Environment.Exit(0);
            }
        }
    }
}