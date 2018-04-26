using LinKu.Models;
using LinKu.Titanium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Titanium.Web.Proxy.Http;

namespace LinKu.Windows
{
    /// <summary>
    /// WindowStarter.xaml 的交互逻辑
    /// </summary>
    public partial class WindowStarter : UserControl, INotifyPropertyChanged
    {
        #region UI更新接口
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public ObservableCollection<SessionListItem> Sessions { get; } = new ObservableCollection<SessionListItem>();
        public SessionListItem SelectedSession
        {
            get => selectedSession;
            set
            {
                if (value != selectedSession)
                {
                    selectedSession = value;
                }
            }
        }

        public readonly Dictionary<HttpWebClient, SessionListItem> sessionDictionary = new Dictionary<HttpWebClient, SessionListItem>();
        private SessionListItem selectedSession;


        private HttpProxyAgent agent = null;
        private bool open = false;


        public WindowStarter()
        {
            InitializeComponent();
            //DataContext = this;
        }

        /// <summary>
        /// 开启和关闭
        /// </summary>
        private void BtnOpenSpeed_Click(object sender, RoutedEventArgs e)
        {
            if (!open)
            {
                agent = new HttpProxyAgent(this);
                open = true;
                BtnOpenSpeed.Content = "关闭代理";
                MessageBox.Show("开启成功！");
            }
            else
            {
                if (agent != null)
                {
                    agent.HttpProxyAgentEnd();

                    BtnOpenSpeed.Content = "启动代理";
                    MessageBox.Show("已关闭！");
                }
                else
                {
                    BtnOpenSpeed.Content = "开启代理";
                    MessageBox.Show("未开启！");
                }
                open = false;
            }
            //MessageBox.Show("开启成功！");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
