using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace LinKu
{
    /// <summary>
    /// xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 主程序路径
        /// </summary>
        public static WindowMain main;
        public static App app;


        /// <summary>
        /// 背景图路径
        /// </summary>
        private static string backgroundPath;
        public static string BackgroundPath {
            get { return backgroundPath; }
            set
            {
                backgroundPath = value;
                ini.IniWriteValue("Skin","Background",value);
                main.UpdateInfo();
            }
        }

        /// <summary>
        /// 背景图透明度
        /// </summary>
        private static double backgroundOpacity;
        public static double BackgroundOpacity {
            get { return backgroundOpacity; }
            set {
                backgroundOpacity = value;
                ini.IniWriteValue("Skin", "BackgroundOpacity",value.ToString());
                main.UpdateInfo();
            }
        }

        public static double MainCornerRadius;

        /// <summary>
        /// 默认背景图
        /// </summary>
        public string Background = Environment.CurrentDirectory + "\\bg.jpg";


        protected override void OnStartup(StartupEventArgs e)
        {
            app = this;
            Init();

            ShutdownMode = ShutdownMode.OnMainWindowClose;
            main = new WindowMain();
            MainWindow = main;
            //启动
            //Global.Start();

            WindowLogin login = new WindowLogin();
            login.ShowDialog();

            //WindowBall w = new WindowBall();
            //w.Show();


            if (Auth.IsLogin)
            {
                 //启动主界面   
                 main.Show();
            }
            else
            {
                main.Close();
            }
        }


        static INIFileHelper ini = new INIFileHelper();
        /// <summary>
        /// 读取窗体背景色
        /// </summary>
        public void ReadBackColor()
        {
            string bgcolor_astr = ini.IniReadValue("Skin", "BackColor-A");
            string bgcolor_rstr = ini.IniReadValue("Skin", "BackColor-R");
            string bgcolor_gstr = ini.IniReadValue("Skin", "BackColor-G");
            string bgcolor_bstr = ini.IniReadValue("Skin", "BackColor-B");
            if (bgcolor_astr != "" || bgcolor_rstr != "" || bgcolor_gstr != "" || bgcolor_bstr != "")
            {
                byte A = Convert.ToByte(bgcolor_astr);
                byte R = Convert.ToByte(bgcolor_rstr);
                byte G = Convert.ToByte(bgcolor_gstr);
                byte B = Convert.ToByte(bgcolor_bstr);
                SolidColorBrush MainBackColor = new SolidColorBrush(Color.FromArgb(A, R, G, B));
                if (MainBackColor == null)//默认白色
                {
                    MainBackColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                }
                Resources.Remove("MainBackColor");
                Resources.Add("MainBackColor",MainBackColor);
            }
            else
            {
                ini.IniWriteValue("Skin", "BackColor-A", "255");
                ini.IniWriteValue("Skin", "BackColor-R", "0");
                ini.IniWriteValue("Skin", "BackColor-G", "0");
                ini.IniWriteValue("Skin", "BackColor-B", "0");
            }
        }
        /// <summary>
        /// 设置窗体背景色
        /// </summary>
        internal void SetMainBackColor(Color c)
        {
            ini.IniWriteValue("Skin", "BackColor-A", c.A.ToString());
            ini.IniWriteValue("Skin", "BackColor-R", c.R.ToString());
            ini.IniWriteValue("Skin", "BackColor-G", c.G.ToString());
            ini.IniWriteValue("Skin", "BackColor-B", c.B.ToString());
        }

        static int MainColorValue = 10;

        /// <summary>
        /// 读取主题色
        /// </summary>
        internal void ReadMainColor()
        {
            //设置文字的颜色
            string astr = ini.IniReadValue("Skin", "MainColor-A");
            string rstr = ini.IniReadValue("Skin", "MainColor-R");
            string gstr = ini.IniReadValue("Skin", "MainColor-G");
            string bstr = ini.IniReadValue("Skin", "MainColor-B");
            if (astr != "" || rstr != "" || gstr != "" || bstr != "")
            {
                byte A = Convert.ToByte(astr);
                byte R = Convert.ToByte(rstr);
                byte G = Convert.ToByte(gstr);
                byte B = Convert.ToByte(bstr);
                SolidColorBrush MainTextColor = new SolidColorBrush(Color.FromArgb(A, R, G, B));
                if (MainTextColor == null)//默认白色
                {
                    MainTextColor = new SolidColorBrush(Color.FromRgb(81, 175, 66));
                }
                Resources.Remove("MainColor");
                Resources.Add("MainColor", MainTextColor);

                Resources.Remove("MainColor1");
                Resources.Add("MainColor1", MainTextColor);

                byte r = (byte)(MainTextColor.Color.R - MainColorValue);
                byte r2 = (byte)(MainTextColor.Color.R - MainColorValue*1.5);

                byte g = (byte)(MainTextColor.Color.G - MainColorValue);
                byte g2 = (byte)(MainTextColor.Color.G - MainColorValue*1.5);

                byte b = (byte)(MainTextColor.Color.B - MainColorValue);
                byte b2 = (byte)(MainTextColor.Color.B - MainColorValue * 1.5);

                Resources.Add("MainColor2", new SolidColorBrush(Color.FromArgb(A,r,g,b)));

                Resources.Remove("MainColor3");
                Resources.Add("MainColor3", new SolidColorBrush(Color.FromArgb(A,r2,g2,b2)));
            }
            else
            {
                ini.IniWriteValue("Skin", "MainColor-A", "255");
                ini.IniWriteValue("Skin", "MainColor-R", "81");
                ini.IniWriteValue("Skin", "MainColor-G", "174");
                ini.IniWriteValue("Skin", "MainColor-B", "66");
            }
        }

        /// <summary>
        /// 设置主题色
        /// </summary>
        internal void SetMainColor(Color c)
        {
            Resources.Remove("MainColor");
            Resources.Add("MainColor", new SolidColorBrush(c));

            Resources.Remove("MainColor1");
            Resources.Add("MainColor1", new SolidColorBrush(c));

            Resources.Remove("MainColor2");

            byte r = (byte)(c.R - MainColorValue);
            byte r2 = (byte)(c.R - MainColorValue * 1.5);

            byte g = (byte)(c.G - MainColorValue);
            byte g2 = (byte)(c.G - MainColorValue * 1.5);

            byte b = (byte)(c.B - MainColorValue);
            byte b2 = (byte)(c.B - MainColorValue * 1.5);

            Resources.Add("MainColor2", new SolidColorBrush(Color.FromArgb(c.A, r, g, b)));

            Resources.Remove("MainColor3");
            Resources.Add("MainColor3", new SolidColorBrush(Color.FromArgb(c.A, r2, g2, b2)));

            ini.IniWriteValue("Skin", "MainColor-A", c.A.ToString());
            ini.IniWriteValue("Skin", "MainColor-R", c.R.ToString());
            ini.IniWriteValue("Skin", "MainColor-G", c.G.ToString());
            ini.IniWriteValue("Skin", "MainColor-B", c.B.ToString());
        }

        /// <summary>
        /// 写入左侧颜色
        /// </summary>
        /// <param name="c"></param>
        internal void ReadMainLeftBack()
        {
            //设置文字的颜色
            string astr = ini.IniReadValue("Skin", "LeftBackColor-A");
            string rstr = ini.IniReadValue("Skin", "LeftBackColor-R");
            string gstr = ini.IniReadValue("Skin", "LeftBackColor-G");
            string bstr = ini.IniReadValue("Skin", "LeftBackColor-B");
            if (astr != "" || rstr != "" || gstr != "" || bstr != "")
            {
                byte A = Convert.ToByte(astr);
                byte R = Convert.ToByte(rstr);
                byte G = Convert.ToByte(gstr);
                byte B = Convert.ToByte(bstr);
                SolidColorBrush MainTextColor = new SolidColorBrush(Color.FromArgb(A, R, G, B));
                if (MainTextColor == null)//默认白色
                {
                    MainTextColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
                Resources.Remove("MainLeftBackColor");
                Resources.Add("MainLeftBackColor", MainTextColor);
            }
            else
            {
                ini.IniWriteValue("Skin", "LeftBackColor-A", "150");
                ini.IniWriteValue("Skin", "LeftBackColor-R", "80");
                ini.IniWriteValue("Skin", "LeftBackColor-G", "80");
                ini.IniWriteValue("Skin", "LeftBackColor-B", "80");
            }
        }
        /// <summary>
        /// 读取左侧颜色
        /// </summary>
        internal void SetMainLeftBack(Color c)
        {
            ini.IniWriteValue("Skin", "LeftBackColor-A", c.A.ToString());
            ini.IniWriteValue("Skin", "LeftBackColor-R", c.R.ToString());
            ini.IniWriteValue("Skin", "LeftBackColor-G", c.G.ToString());
            ini.IniWriteValue("Skin", "LeftBackColor-B", c.B.ToString());
        }

        /// <summary>
        /// 读取主题文字颜色
        /// </summary>
        public void ReadMainTextColor()
        {
            //设置文字的颜色
            string astr = ini.IniReadValue("Skin", "A");
            string rstr = ini.IniReadValue("Skin", "R");
            string gstr = ini.IniReadValue("Skin", "G");
            string bstr = ini.IniReadValue("Skin", "B");
            if (astr != "" || rstr != "" || gstr != "" || bstr != "")
            {
                byte A = Convert.ToByte(astr);
                byte R = Convert.ToByte(rstr);
                byte G = Convert.ToByte(gstr);
                byte B = Convert.ToByte(bstr);
                SolidColorBrush MainTextColor = new SolidColorBrush(Color.FromArgb(A, R, G, B));
                if (MainTextColor == null)//默认白色
                {
                    MainTextColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
                Resources.Remove("MainTextColor");
                Resources.Add("MainTextColor", MainTextColor);
            }
            else
            {
                ini.IniWriteValue("Skin", "A", "255");
                ini.IniWriteValue("Skin", "R", "255");
                ini.IniWriteValue("Skin", "G", "255");
                ini.IniWriteValue("Skin", "B", "255");
            }
        }
        /// <summary>
        /// 设置主题文字颜色
        /// </summary>
        public void SetMainTextColor(Color c)
        {
            ini.IniWriteValue("Skin", "A", c.A.ToString());
            ini.IniWriteValue("Skin", "R", c.R.ToString());
            ini.IniWriteValue("Skin", "G", c.G.ToString());
            ini.IniWriteValue("Skin", "B", c.B.ToString());
        }

        /// <summary>
        /// 读取圆角值
        /// </summary>
        public void ReadMainCornerRadius()
        {
            string MainCornerRadiusstr = ini.IniReadValue("Skin", "MainCornerRadius");
            if (MainCornerRadiusstr != "")
            {
                int value = Convert.ToInt32(MainCornerRadiusstr);
                CornerRadius MainCornerRadius = new CornerRadius(value);
                Resources.Remove("MainCornerRadius");
                Resources.Add("MainCornerRadius", MainCornerRadius);

                App.MainCornerRadius = value;
            }
            else
            {
                ini.IniWriteValue("Skin", "MainCornerRadius", "20");
            }
        }
       
        /// <summary>
        /// 设置圆角值
        /// </summary>
        public void SetMainCornerRadius(double value)
        {
            ini.IniWriteValue("Skin", "MainCornerRadius",value.ToString());
            CornerRadius MainCornerRadius = new CornerRadius(value);
            Resources.Remove("MainCornerRadius");
            Resources.Add("MainCornerRadius", MainCornerRadius);
        }

        /// <summary>
        /// 读取窗体背景图
        /// </summary>
        public void ReadBackground()
        {
            backgroundPath = ini.IniReadValue("Skin", "Background");
            if (backgroundPath == "" || !File.Exists(BackgroundPath))
            {
                backgroundPath = Background;
                ini.IniWriteValue("Skin", "Background", Background);
            }
            string BackgroundOpacitystr = ini.IniReadValue("Skin", "BackgroundOpacity");
            if (BackgroundOpacitystr == "")
            {
                BackgroundOpacitystr = "0.2";
                ini.IniWriteValue("Skin", "BackgroundOpacity", "0.2");
            }
            backgroundOpacity = Convert.ToDouble(BackgroundOpacitystr);
        }

        public void Init()
        {
            //读取本地INI文件
            if (File.Exists("Skin.ini"))
            {
                ReadMainColor();

                ReadMainLeftBack();

                ReadBackColor();

                ReadBackground();

                ReadMainTextColor();

                ReadMainCornerRadius();
            }
            else
            {
                File.CreateText("Skin.ini");
            }
        }

    }
}
