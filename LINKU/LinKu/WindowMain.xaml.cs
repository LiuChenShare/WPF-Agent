using LinKu.Models;
using LinKu.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    public partial class WindowMain : DMSkinWindow
    {
        public WindowMain()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 皮肤设置中心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSkin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainTab.SelectedItem = TabSkin;
        }

        private void DMSkinWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (LoadCount == 0)
            {
                UpdateInfo();
            }
            LoadCount++;
        }

        /// <summary>
        /// 初始化背景图
        /// </summary>
        internal void UpdateInfo()
        {
            if (File.Exists(App.BackgroundPath))
            {
                Uri uri = new Uri(App.BackgroundPath, UriKind.Absolute);
                BitmapImage bitmap = new BitmapImage(uri);
                ImageBrush image = new ImageBrush(bitmap)
                {
                    Opacity = App.BackgroundOpacity
                };
                MainBack.Background = image;
            }
            else
            {
                MainBack.Background = (SolidColorBrush)Application.Current.Resources["MainBackColor"];
            }
        }

        /// <summary>
        /// 页面读取次数
        /// </summary>
        int LoadCount = 0;
        /// <summary>
        /// 初始化
        /// </summary>

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Auth.Home);
        }

        public void TurnGame(Game name)
        {
            Auth.SelectGame = name;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(50);
                Dispatcher.Invoke(new Action(() => {
                    MainTab.SelectedItem = TabSpeed;
                    //windowspeed.UpdateInfo();
                }));
            });
        }
    }
}
