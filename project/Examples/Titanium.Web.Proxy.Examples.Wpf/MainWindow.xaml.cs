using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Helpers;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;
using Titanium.Web.Proxy.Network;

namespace Titanium.Web.Proxy.Examples.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<SessionListItem> Sessions { get; } = new ObservableCollection<SessionListItem>();

        public Rules _rules = Rules.GetRules();

        public SessionListItem SelectedSession
        {
            get => selectedSession;
            set
            {
                if (value != selectedSession)
                {
                    selectedSession = value;
                    OnSelectedSessionChanged();
                }
            }
        }

        public readonly Dictionary<HttpWebClient, SessionListItem> sessionDictionary = new Dictionary<HttpWebClient, SessionListItem>();
        private SessionListItem selectedSession;

        public Action OnSelectedSessionChanged;

        public MainWindow()
        {
            InitializeComponent();
            

            new HttpProxyAgent(this);
        }


        private void ListViewSessions_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedItems = ((ListView)sender).SelectedItems;
                foreach (var item in selectedItems.Cast<SessionListItem>().ToArray())
                {
                    Sessions.Remove(item);
                    sessionDictionary.Remove(item.WebSession);
                }
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //读取
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FileJson fileJson = new FileJson();
            var result = fileJson.GetResult();
            _rules.DomainRules = result;
            MessageBox.Show("读取配置文件成功！");
        }

        //保存
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveRules();
            FileJson fileJson = new FileJson();
            //fileJson.SetResult(new List<DomainRules>() { new DomainRules() { Host = "baidu.com", RexStr = new List<string>() { "dadwa","dwadaw"} } });
            fileJson.SetResult(_rules.DomainRules);
            MessageBox.Show("保存配置文件成功！");
        }

        //清空
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Sessions.Clear();
            sessionDictionary.Clear();
        }

        //保存修改的正则
        private void SaveRules()
        {
            var host = this.Host.Content.ToString().GetRootHost();
            var rulesStr = this.Rule.Text;
            var domainRule = _rules.DomainRules.Where(x => x.Host == host).FirstOrDefault();
            var rulesList = new List<string>();
            if (rulesStr != null)
            {
                var rules = System.Text.RegularExpressions.Regex.Split(this.Rule.Text, "\n");
                if (rules.Count() > 0)
                {
                    rulesList = rules.ToList();
                }
            }

            if (domainRule != null)
            {
                _rules.DomainRules.Where(x => x.Host == host).FirstOrDefault().RexStr = rulesList;
            }
            else
            {
                _rules.DomainRules.Add(new Wpf.DomainRules() { Host = host, RexStr = rulesList });
            }
        }
    }
}
