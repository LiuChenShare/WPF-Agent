using LinKu.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LinKu
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WindowMessageBox : DMSkinWindow
    {
        public WindowMessageBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        string message = "";
        public string Message
        {
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
            get { return message; }
        }

        public static void Show(string message)
        {
            WindowMessageBox ms = new WindowMessageBox()
            {
                Message = message
            };
            ms.ShowDialog();
        }
    }
}
