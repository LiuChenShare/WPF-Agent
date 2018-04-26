using System;
using System.Windows;

namespace Titanium.Web.Proxy.Examples.Wpf
{
    public class App
    {

        public static Application Instance { get; private set; }
        public static MainWindow MainWindow;

        [STAThread]
        static void Main()
        {

            // 定义Application对象作为整个应用程序入口  

            App.Instance = new Application();

            // 通过Url的方式启动

            App.Instance.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            App.Instance.Run();

        }

    }
}
