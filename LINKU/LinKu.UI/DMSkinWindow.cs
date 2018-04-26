﻿using LinKu.UI.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace LinKu.UI
{
    /// <summary>
    /// 阴影窗体
    /// </summary>
    public partial class DMSkinWindow : Window, INotifyPropertyChanged
    {
        #region UI更新接口
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region 初始化
        public DMSkinWindow()
        {
            AllowsTransparency = true;
            WindowStyle = WindowStyle.None;
            InitializeStyle();
            DataContext = this;
            BindFunc();
            Loaded += new RoutedEventHandler(DMLoad);
        }


        private void DMLoad(object sender, RoutedEventArgs e)
        {
            WindowBorder = (Grid)Template.FindName("WindowBorder", this);

            Button btnClose = (Button)Template.FindName("PART_Close", this);
            btnClose.Click += delegate
            {
                Close();
            };
            Button btnMax = (Button)Template.FindName("PART_Max", this);
            btnMax.Click += delegate
            {
                WindowState = WindowState.Maximized;
            };
            Button btnRestore = (Button)Template.FindName("PART_Restore", this);
            btnRestore.Click += delegate
            {
                WindowState = WindowState.Normal;
            };
            Button btnMin = (Button)Template.FindName("PART_Min", this);
            btnMin.Click += delegate
            {
                WindowState = WindowState.Minimized;
            };
        }

        Style MainWindowShadow;
        Style MainWindowMetro;
        private void InitializeStyle()
        {
            string packUri = @"/LinKu.UI;component/Themes/DMSkin.xaml";
            ResourceDictionary dic = new ResourceDictionary { Source = new Uri(packUri, UriKind.Relative) };
            this.Resources.MergedDictionaries.Add(dic);
            MainWindowShadow = this.Style = (Style)dic["MainWindow"];
            MainWindowMetro = (Style)dic["MainWindowMetro"];
        }
        #endregion

        #region 窗体操作函数
        //绑定
        private void BindFunc()
        {
            //绑定窗体操作函数
            this.SourceInitialized += MainWindow_SourceInitialized;
            this.StateChanged += MainWindow_StateChanged;
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
        }

        //自定义边框宽度
        private readonly int customBorderThickness = 10;

        //边框
        private Grid WindowBorder;

        //WIN32 互操作
        void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            if (source == null)
            { throw new Exception("Cannot get HwndSource instance."); }
            source.AddHook(new HwndSourceHook(this.WndProc));
        }

        //消息
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_GETMINMAXINFO: // WM_GETMINMAXINFO message  
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
                    //case Win32.WM_NCCALCSIZE:
                    //        if (window->is_borderless) return 0;
                    //        else return DefWindowProc(hwnd, msg, wparam, lparam);
                    //    break;
            }
            return IntPtr.Zero;
        }

        //最大最小化信息
        void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            // MINMAXINFO structure  
            NativeMethods.MINMAXINFO mmi = (NativeMethods.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(NativeMethods.MINMAXINFO));

            // Get handle for nearest monitor to this window  
            WindowInteropHelper wih = new WindowInteropHelper(this);
            IntPtr hMonitor = NativeMethods.MonitorFromWindow(wih.Handle, NativeMethods.MONITOR_DEFAULTTONEAREST);

            // Get monitor info   显示屏
            NativeMethods.MONITORINFOEX monitorInfo = new NativeMethods.MONITORINFOEX();

            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            NativeMethods.GetMonitorInfo(new HandleRef(this, hMonitor), monitorInfo);

            // Convert working area  
            NativeMethods.RECT workingArea = monitorInfo.rcWork;

            // Set the maximized size of the window  
            //ptMaxSize：  设置窗口最大化时的宽度、高度
            //mmi.ptMaxSize.x = (int)dpiIndependentSize.X;
            //mmi.ptMaxSize.y = (int)dpiIndependentSize.Y;

            // Set the position of the maximized window  
            mmi.ptMaxPosition.x = workingArea.Left;
            mmi.ptMaxPosition.y = workingArea.Top;

            // Get HwndSource  
            HwndSource source = HwndSource.FromHwnd(wih.Handle);
            if (source == null)
                // Should never be null  
                throw new Exception("Cannot get HwndSource instance.");
            if (source.CompositionTarget == null)
                // Should never be null  
                throw new Exception("Cannot get HwndTarget instance.");

            Matrix matrix = source.CompositionTarget.TransformToDevice;

            Point dpiIndenpendentTrackingSize = matrix.Transform(new Point(
               this.MinWidth,
               this.MinHeight
               ));

            if (DMFull)
            {
                Point dpiSize = matrix.Transform(new Point(
              System.Windows.SystemParameters.PrimaryScreenWidth,
              System.Windows.SystemParameters.PrimaryScreenHeight
              ));

                mmi.ptMaxSize.x = (int)dpiSize.X;
                mmi.ptMaxSize.y = (int)dpiSize.Y;
            }
            else
            {
                mmi.ptMaxSize.x = workingArea.Right;
                mmi.ptMaxSize.y = workingArea.Bottom;
            }

            // Set the minimum tracking size ptMinTrackSize： 设置窗口最小宽度、高度 
            mmi.ptMinTrackSize.x = (int)dpiIndenpendentTrackingSize.X;
            mmi.ptMinTrackSize.y = (int)dpiIndenpendentTrackingSize.Y;

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        //窗体最大化 隐藏阴影
        void MainWindow_StateChanged(object sender, EventArgs e)
        {
            //WindowBorder 窗体边框
            if (WindowState == WindowState.Maximized)
            {
                if (DMShowMax)
                {
                    BtnMaxVisibility = Visibility.Collapsed;
                    BtnRestoreVisibility = Visibility.Visible;
                }
                WindowBorder.Margin = new Thickness(0);
            }
            if (WindowState == WindowState.Normal)
            {
                if (DMShowMax)
                {
                    BtnMaxVisibility = Visibility.Visible;
                    BtnRestoreVisibility = Visibility.Collapsed;
                }
                WindowBorder.Margin = new Thickness(customBorderThickness);
            }
        }

        //窗体移动
        void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Grid || e.OriginalSource is Border || e.OriginalSource is Window)
            {
                WindowInteropHelper wih = new WindowInteropHelper(this);
                NativeMethods.SendMessage(wih.Handle, NativeMethods.WM_NCLBUTTONDOWN, (int)NativeMethods.HitTest.HTCAPTION, 0);
                return;
            }
        }

        public class NativeMethods
        {
            // Sent to a window when the size or position of the window is about to change  
            public const int WM_GETMINMAXINFO = 0x0024;

            // Retrieves a handle to the display monitor that is nearest to the window  
            public const int MONITOR_DEFAULTTONEAREST = 2;

            // Retrieves a handle to the display monitor  
            [DllImport("user32.dll")]
            internal static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

            // RECT structure, Rectangle used by MONITORINFOEX  
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

            // MONITORINFOEX structure, Monitor information used by GetMonitorInfo function  
            [StructLayout(LayoutKind.Sequential)]
            public class MONITORINFOEX
            {
                public int cbSize;
                public RECT rcMonitor; // The display monitor rectangle  
                public RECT rcWork; // The working area rectangle  
                public int dwFlags;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
                public char[] szDevice;
            }

            // Point structure, Point information used by MINMAXINFO structure  
            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int x;
                public int y;

                public POINT(int x, int y)
                {
                    this.x = x;
                    this.y = y;
                }
            }

            // MINMAXINFO structure, Window's maximum size and position information  
            [StructLayout(LayoutKind.Sequential)]
            public struct MINMAXINFO
            {
                public POINT ptReserved;
                public POINT ptMaxSize; // The maximized size of the window  
                public POINT ptMaxPosition; // The position of the maximized window  
                public POINT ptMinTrackSize;
                public POINT ptMaxTrackSize;
            }

            // Get the working area of the specified monitor  
            [DllImport("user32.dll")]
            internal static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX monitorInfo);

            // Sent to a window in order to determine what part of the window corresponds to a particular screen coordinate  
            public const int WM_NCHITTEST = 0x0084;

            /// <summary>  
            /// Indicates the position of the cursor hot spot.  
            /// </summary>  
            public enum HitTest : int
            {
                /// <summary>  
                /// On the screen background or on a dividing line between windows (same as HTNOWHERE, except that the DefWindowProc function produces a system beep to indicate an error).  
                /// </summary>  
                HTERROR = -2,

                /// <summary>  
                /// In a window currently covered by another window in the same thread (the message will be sent to underlying windows in the same thread until one of them returns a code that is not HTTRANSPARENT).  
                /// </summary>  
                HTTRANSPARENT = -1,

                /// <summary>  
                /// On the screen background or on a dividing line between windows.  
                /// </summary>  
                HTNOWHERE = 0,

                /// <summary>  
                /// In a client area.  
                /// </summary>  
                HTCLIENT = 1,

                /// <summary>  
                /// In a title bar.  
                /// </summary>  
                HTCAPTION = 2,

                /// <summary>  
                /// In a window menu or in a Close button in a child window.  
                /// </summary>  
                HTSYSMENU = 3,

                /// <summary>  
                /// In a size box (same as HTSIZE).  
                /// </summary>  
                HTGROWBOX = 4,

                /// <summary>  
                /// In a size box (same as HTGROWBOX).  
                /// </summary>  
                HTSIZE = 4,

                /// <summary>  
                /// In a menu.  
                /// </summary>  
                HTMENU = 5,

                /// <summary>  
                /// In a horizontal scroll bar.  
                /// </summary>  
                HTHSCROLL = 6,

                /// <summary>  
                /// In the vertical scroll bar.  
                /// </summary>  
                HTVSCROLL = 7,

                /// <summary>  
                /// In a Minimize button.  
                /// </summary>  
                HTMINBUTTON = 8,

                /// <summary>  
                /// In a Minimize button.  
                /// </summary>  
                HTREDUCE = 8,

                /// <summary>  
                /// In a Maximize button.  
                /// </summary>  
                HTMAXBUTTON = 9,

                /// <summary>  
                /// In a Maximize button.  
                /// </summary>  
                HTZOOM = 9,

                /// <summary>  
                /// In the left border of a resizable window (the user can click the mouse to resize the window horizontally).  
                /// </summary>  
                HTLEFT = 10,

                /// <summary>  
                /// In the right border of a resizable window (the user can click the mouse to resize the window horizontally).  
                /// </summary>  
                HTRIGHT = 11,

                /// <summary>  
                /// In the upper-horizontal border of a window.  
                /// </summary>  
                HTTOP = 12,

                /// <summary>  
                /// In the upper-left corner of a window border.  
                /// </summary>  
                HTTOPLEFT = 13,

                /// <summary>  
                /// In the upper-right corner of a window border.  
                /// </summary>  
                HTTOPRIGHT = 14,

                /// <summary>  
                /// In the lower-horizontal border of a resizable window (the user can click the mouse to resize the window vertically).  
                /// </summary>  
                HTBOTTOM = 15,

                /// <summary>  
                /// In the lower-left corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).  
                /// </summary>  
                HTBOTTOMLEFT = 16,

                /// <summary>  
                /// In the lower-right corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).  
                /// </summary>  
                HTBOTTOMRIGHT = 17,

                /// <summary>  
                /// In the border of a window that does not have a sizing border.  
                /// </summary>  
                HTBORDER = 18,

                /// <summary>  
                /// In a Close button.  
                /// </summary>  
                HTCLOSE = 20,

                /// <summary>  
                /// In a Help button.  
                /// </summary>  
                HTHELP = 21,
            };

            // Posted when the user presses the left mouse button while the cursor is within the nonclient area of a window  
            public const int WM_NCLBUTTONDOWN = 0x00A1;

            // Sends the specified message to a window or windows  
            [DllImport("user32.dll", EntryPoint = "SendMessage")]
            internal static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        }
        #endregion

        #region 窗体控制按钮


        private bool _DMFull = false;
        [Description("全屏是否保留任务栏显示"), Category("DMSkin")]
        public bool DMFull
        {
            get
            {
                return _DMFull;
            }

            set
            {
                _DMFull = value;
                OnPropertyChanged("DMFull");
            }
        }


        private int titleHeight = 30;

        [Description("窗体标题高度(关系到系统按钮)"), Category("DMSkin")]
        public int DMTitleSize
        {
            get
            {
                return titleHeight;
            }

            set
            {
                titleHeight = value;
                OnPropertyChanged("DMTitleSize");
            }
        }


        private int borderSize = 1;

        [Description("Metro窗体边框宽度"), Category("DMSkin")]
        public int DMBorderSize
        {
            get
            {
                return borderSize;
            }

            set
            {
                borderSize = value;
                OnPropertyChanged("DMBorderSize");
            }
        }


        private Brush borderColor = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        [Description("Metro窗体边框颜色"), Category("DMSkin")]
        public Brush DMBorderColor
        {
            get
            {
                return borderColor;
            }

            set
            {
                borderColor = value;
                OnPropertyChanged("DMBorderColor");
            }
        }



        private Brush titleHoverColor = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));

        [Description("窗体系统按钮鼠标悬浮背景颜色"), Category("DMSkin")]
        public Brush DMTitleHoverColor
        {
            get
            {
                return titleHoverColor;
            }

            set
            {
                titleHoverColor = value;
                OnPropertyChanged("DMTitleHoverColor");
            }
        }

        private SolidColorBrush titleColor = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        [Description("窗体系统按钮颜色"), Category("DMSkin")]
        public SolidColorBrush DMTitleColor
        {
            get
            {
                return titleColor;
            }

            set
            {
                titleColor = value;
                OnPropertyChanged("DMTitleColor");
            }
        }

        private DMWindow dmDMWindow = DMWindow.Shadow;
        [Description("窗体类型"), Category("DMSkin")]
        public DMWindow DMWindow
        {
            get
            {
                return dmDMWindow;
            }

            set
            {
                dmDMWindow = value;
                if (dmDMWindow == DMWindow.Shadow)//阴影
                {
                    this.Style = MainWindowShadow;
                }
                else
                {
                    this.Style = MainWindowMetro;
                }
                OnPropertyChanged("DMWindow");
            }
        }

        private bool dmShowMax = true;
        [Description("显示最大化按钮"), Category("DMSkin")]
        public bool DMShowMax
        {
            get
            {
                return dmShowMax;
            }

            set
            {
                dmShowMax = value;
                if (dmShowMax)
                {
                    ResizeMode = ResizeMode.CanResize;
                    BtnMaxVisibility = Visibility.Visible;
                    BtnRestoreVisibility = Visibility.Collapsed;
                }
                else
                {
                    ResizeMode = ResizeMode.CanMinimize;
                    BtnMaxVisibility = Visibility.Collapsed;
                    BtnRestoreVisibility = Visibility.Collapsed;
                }

                OnPropertyChanged("DMShowMax");
            }
        }

        private bool dmShowMin = true;
        [Description("显示最小化按钮"), Category("DMSkin")]
        public bool DMShowMin
        {
            get
            {
                return dmShowMin;
            }

            set
            {
                dmShowMin = value;
                if (dmShowMin)
                {
                    ResizeMode = ResizeMode.CanResize;
                    BtnMinVisibility = Visibility.Visible;
                }
                else
                {
                    ResizeMode = ResizeMode.CanMinimize;
                    BtnMinVisibility = Visibility.Collapsed;
                }
                OnPropertyChanged("DMShowMin");
            }
        }


        private bool dmShowClose = true;
        [Description("显示关闭按钮"), Category("DMSkin")]
        public bool DMShowClose
        {
            get
            {
                return dmShowClose;
            }

            set
            {
                dmShowClose = value;
                if (dmShowClose)
                {
                    BtnCloseVisibility = Visibility.Visible;
                }
                else
                {
                    BtnCloseVisibility = Visibility.Collapsed;
                }
                OnPropertyChanged("DMShowClose");
            }
        }

        private int dmShadowSize = 10;
        [Description("窗体阴影大小"), Category("DMSkin")]
        public int DMShadowSize
        {
            get
            {
                return dmShadowSize;
            }

            set
            {
                dmShadowSize = value;
                OnPropertyChanged("DMShadowSize");
            }
        }

        private Color dmShadowColor = Color.FromArgb(255, 151, 151, 151);

        [Description("窗体阴影颜色"), Category("DMSkin")]
        public Color DMShadowColor
        {
            get
            {
                return dmShadowColor;
            }

            set
            {
                dmShadowColor = value;
                OnPropertyChanged("DMShadowColor");
            }
        }

        private Visibility btnMinVisibility = Visibility.Visible;
        //最小化按钮显示
        public Visibility BtnMinVisibility
        {
            get
            {
                return btnMinVisibility;
            }

            set
            {
                btnMinVisibility = value;
                OnPropertyChanged("BtnMinVisibility");
            }
        }

        private Visibility btnCloseVisibility = Visibility.Visible;
        //关闭按钮显示
        public Visibility BtnCloseVisibility
        {
            get
            {
                return btnCloseVisibility;
            }

            set
            {
                btnCloseVisibility = value;
                OnPropertyChanged("BtnCloseVisibility");
            }
        }

        private Visibility btnMaxVisibility = Visibility.Visible;
        //最大化按钮显示
        public Visibility BtnMaxVisibility
        {
            get
            {
                return btnMaxVisibility;
            }

            set
            {
                btnMaxVisibility = value;
                OnPropertyChanged("BtnMaxVisibility");
            }
        }

        private Visibility btnRestoreVisibility = Visibility.Collapsed;
        //最大化按钮显示
        public Visibility BtnRestoreVisibility
        {
            get
            {
                return btnRestoreVisibility;
            }

            set
            {
                btnRestoreVisibility = value;
                OnPropertyChanged("BtnRestoreVisibility");
            }
        }

        #endregion
    }
}
