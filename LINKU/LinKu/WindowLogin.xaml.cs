using LinKu.Models;
using LinKu.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LinKu
{
    /// <summary>
    /// WindowLogin.xaml 的交互逻辑
    /// </summary>
    public partial class WindowLogin : DMSkinWindow
    {
        public WindowLogin()
        {
            InitializeComponent();

            TbUserName.Text = "5870352@qq.com";
            //tbPasswordBox.Text = "1111";
        }


        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        bool Loging = false;
        private void Login()
        {
            if (Loging)
            {
                return;
            }
            MessageText.Text = "";
            Auth.user = new User() { Name = TbUserName.Text.Trim(), Pwd = TbPasswordBox.Password.Trim() };
            if (Auth.user.Name == "" || Auth.user.Pwd == "")
            {
                MessageText.Text = "用户名密码不能为空!";
                return;
            }
            Loging = true;
            Service.Login(new Action<MsgBase>((msg) => {
                Dispatcher.Invoke(new Action(() => {
                    switch (msg.State)
                    {
                        case MsgState.Susuccess:
                            Close();
                            break;
                        case MsgState.Step:
                            LoginProgressBar.Value = msg.Value;
                            break;
                        default:
                            Loging = false;
                            MessageText.Text = msg.Message;
                            break;
                    }
                }));
            }));
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Auth.Home);
        }

        /// <summary>
        /// 记住密码
        /// </summary>
        private void CheckPassWord_Click(object sender, RoutedEventArgs e)
        {
             
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
        private void UpdateInfo()
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

        private void TbPasswordBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
    }
}
