using AutoSlashConverter.Framework.Utilities;
using AutoSlashConverter.Presentation.ViewModels;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace AutoSlashConverter.Presentation.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int WM_USER = 0x0400; // 0x0400 是用户定义的消息起始值
        public const int WM_RESTOREWINDOW = WM_USER + 1; // 自定义消息，用于恢复窗口
        private bool _isProcessingClipboard = false;

        public MainWindow()
        {
            InitializeComponent();

            this.WindowState = WindowState.Minimized;

            // 在窗口完全初始化后执行隐藏
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Hide();
            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);

            this.Unloaded += (s, e) =>
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                if (hwnd != IntPtr.Zero && HwndSource.FromHwnd(hwnd) != null)
                {
                    Win32.RemoveClipboardFormatListener(hwnd);
                }
            };

            MyNotifyIcon.ContextMenu.Opened += (s, e) =>
            {
                Win32.GetCursorPos(out Win32.POINT point);

                PresentationSource source = PresentationSource.FromVisual(this);

                MyNotifyIcon.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
                MyNotifyIcon.ContextMenu.HorizontalOffset = point.X / source.CompositionTarget.TransformToDevice.M11;
                MyNotifyIcon.ContextMenu.VerticalOffset = point.Y / source.CompositionTarget.TransformToDevice.M22;
            };
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Hide();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        /// <summary>
        /// 捕获自定义消息
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            HwndSource source = HwndSource.FromHwnd(hwnd);
            if (source != null)
            {
                source.AddHook(WndProc);
                Win32.AddClipboardFormatListener(hwnd);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_RESTOREWINDOW:
                    Show();
                    WindowState = WindowState.Normal;
                    Topmost = true;
                    Topmost = false;
                    handled = true;
                    break;

                case Win32.WM_CLIPBOARDUPDATE:
                    // 检查是否启用转换功能
                    var viewModel = this.DataContext as MainWindowViewModel;
                    if (viewModel?.IsConversionEnabled != true || _isProcessingClipboard)
                    {
                        break;
                    }
                    
                    try
                    {
                        _isProcessingClipboard = true;

                        if (Clipboard.ContainsText())
                        {
                            string clipboardText = Clipboard.GetText();
                            if (clipboardText != null &&
                                clipboardText.Length > 2 &&
                                Regex.IsMatch(clipboardText, @"^[a-zA-Z]:\\") &&
                                clipboardText.Contains(@"\") &&
                                !clipboardText.Contains(@"/"))
                            {
                                string modifiedText = Regex.Replace(clipboardText, @"\\+", "/");
                                if (modifiedText != clipboardText)
                                {
                                    Win32.RemoveClipboardFormatListener(hwnd);
                                    Clipboard.SetText(modifiedText);
                                    Win32.AddClipboardFormatListener(hwnd);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error processing clipboard: " + ex.Message);
                    }
                    finally
                    {
                        _isProcessingClipboard = false;
                    }
                    handled = true;
                    break;
            }

            return IntPtr.Zero;
        }
    }
}