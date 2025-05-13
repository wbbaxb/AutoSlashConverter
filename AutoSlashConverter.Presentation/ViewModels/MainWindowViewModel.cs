using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;

namespace AutoSlashConverter.Presentation.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        public static string Title => "AutoSlashConverter";

        public ICommand MinsizeCommand => new RelayCommand(() => Application.Current.MainWindow.WindowState = WindowState.Minimized);

        public ICommand CloseCommand => new RelayCommand(Application.Current.MainWindow.Close);

        public ICommand ShowCommand => new RelayCommand(() =>
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            Application.Current.MainWindow.Activate();
        });

        public ICommand ShutdownCommand => new RelayCommand(Application.Current.Shutdown);
    }
}