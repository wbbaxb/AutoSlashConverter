using AutoSlashConverter.Framework.Utilities;
using AutoSlashConverter.Presentation.ViewModels;
using Microsoft.Toolkit.Mvvm.Messaging;
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
    public partial class MainWindow : Window, IRecipient<string>
    {
        private const int WM_USER = 0x0400; // 0x0400 是用户定义的消息起始值
        public const int WM_RESTOREWINDOW = WM_USER + 1; // 自定义消息，用于恢复窗口
        private bool _isProcessingClipboard = false;

        public MainWindow()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register(this);

            //this.WindowState = WindowState.Minimized;

            //// 在窗口完全初始化后执行隐藏
            //Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    this.Hide();
            //}), System.Windows.Threading.DispatcherPriority.ApplicationIdle);

            this.Unloaded += (s, e) =>
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                if (hwnd != IntPtr.Zero && HwndSource.FromHwnd(hwnd) != null)
                {
                    Win32.RemoveClipboardFormatListener(hwnd);
                }

                WeakReferenceMessenger.Default.Unregister<string>(this);
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
                                IsWindowsPath(clipboardText) &&
                                clipboardText.Contains(@"\") &&
                                !clipboardText.Contains(@"/"))
                            {
                                string modifiedText = Regex.Replace(clipboardText, @"\\+", "/");
                                if (modifiedText != clipboardText)
                                {
                                    SetClipboardText(hwnd, modifiedText);

                                    viewModel.AddConversionHistory(clipboardText, modifiedText);
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

        private bool IsWindowsPath(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return Regex.IsMatch(text, @"^([a-zA-Z]:\\|\\.*|\.{1,2}\\|[\w\s$.-]+\\)", RegexOptions.IgnoreCase);
        }

        public void Receive(string message)
        {
            SetClipboardWithoutConversion(message);
        }

        private void SetClipboardWithoutConversion(string text)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            SetClipboardText(hwnd, text);
        }

        private void SetClipboardText(IntPtr hwnd, string text)
        {
            if (hwnd != IntPtr.Zero)
            {
                Win32.RemoveClipboardFormatListener(hwnd);
                Clipboard.SetText(text);
                Win32.AddClipboardFormatListener(hwnd);
            }
        }
    }
}