using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;

namespace AutoSlashConverter.Presentation.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private bool _isConversionEnabled = true;
        
        public static string Title => "AutoSlashConverter";

        public bool IsConversionEnabled
        {
            get => _isConversionEnabled;
            set
            {
                _isConversionEnabled = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConversionStatusText));
            }
        }

        public string ConversionStatusText => IsConversionEnabled ? "已启用" : "已禁用";

        public ICommand MinsizeCommand => new RelayCommand(() => Application.Current.MainWindow.WindowState = WindowState.Minimized);

        public ICommand CloseCommand => new RelayCommand(Application.Current.MainWindow.Close);

        public ICommand ShowCommand => new RelayCommand(() =>
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            Application.Current.MainWindow.Activate();
        });

        public ICommand ShutdownCommand => new RelayCommand(Application.Current.Shutdown);

        public ICommand ShowStartupNotificationCommand => new RelayCommand(ShowStartupNotification);

        private void ShowStartupNotification()
        {
            var taskbarIcon = Application.Current.MainWindow.FindName("MyNotifyIcon") as TaskbarIcon;
            taskbarIcon?.ShowBalloonTip(Title, "程序已在后台运行", BalloonIcon.Info);
        }
    }
}