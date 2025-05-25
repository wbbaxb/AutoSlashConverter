using AutoSlashConverter.Framework.Models;
using AutoSlashConverter.Framework.Services;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace AutoSlashConverter.Presentation.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private bool _isConversionEnabled = true;
        private readonly ConversionHistoryService _historyService;
        private int _selectedTabIndex = 0;

        public MainWindowViewModel()
        {
            _historyService = new ConversionHistoryService();
        }

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

        public ObservableCollection<ConversionHistory> ConversionHistories => _historyService.Histories;

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                _selectedTabIndex = value;
                OnPropertyChanged();
            }
        }

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

        public ICommand ClearHistoryCommand => new RelayCommand(_historyService.ClearHistories);

        public ICommand CopyCommand => new RelayCommand<string>(CopyPath);

        public ICommand ShowHistoryCommand => new RelayCommand(ShowHistory);

        public void AddConversionHistory(string originalPath, string convertedPath)
        {
            _historyService.AddHistory(originalPath, convertedPath);
        }

        private void ShowStartupNotification()
        {
            var taskbarIcon = Application.Current.MainWindow.FindName("MyNotifyIcon") as TaskbarIcon;
            taskbarIcon?.ShowBalloonTip(Title, "程序已在后台运行", BalloonIcon.Info);
        }

        private void CopyPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                WeakReferenceMessenger.Default.Send(path);
            }
        }

        private void ShowHistory()
        {
            ShowCommand.Execute(null);
            SelectedTabIndex = 1;
        }
    }
}