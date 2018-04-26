using LinKu.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LinKu.Windows
{
    /// <summary>
    /// WindowSpeed.xaml 的交互逻辑
    /// </summary>
    public partial class WindowSpeed : UserControl, INotifyPropertyChanged
    {
        #region UI更新接口
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public WindowSpeed()
        {
            InitializeComponent();
            DataContext = this;

            dgvShow.ItemsSource = NetworkNodeList;
        }

        ObservableCollection<NetworkNode> _NetworkNodeList;
        /// <summary>
        /// 网络节点列表
        /// </summary>
        public ObservableCollection<NetworkNode> NetworkNodeList
        {
            get
            {
                if (_NetworkNodeList == null)
                {
                    _NetworkNodeList = new ObservableCollection<NetworkNode>();
                }
                return _NetworkNodeList;
            }
            set
            {
                _NetworkNodeList = value;
                OnPropertyChanged("NetworkNodeList");
            }
        }


        private Game selectGame;
        /// <summary>
        /// 游戏名字
        /// </summary>
        public Game SelectGame
        {
            get
            {
                return selectGame;
            }
            set
            {
                selectGame = value;
                OnPropertyChanged("SelectGame");
            }
        }

        private NetworkNode selectNetworkNode;
        /// <summary>
        /// 游戏名字
        /// </summary>
        public NetworkNode SelectNetworkNode
        {
            get
            {
                return selectNetworkNode;
            }
            set
            {
                selectNetworkNode = value;
                OnPropertyChanged("SelectNetworkNode");
            }
        }


        int LoadCount = 0;
        /// <summary>
        /// 初始化
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (LoadCount == 0)
            {
                UpdateInfo();
            }
            LoadCount++;
        }

        public void UpdateInfo()
        {

            //读取游戏信息
            SelectGame = Auth.SelectGame;

            if (SelectGame.Name== "未选择")
            {
                return;
            }
            NetworkNodeList.Clear();
            //加载网络节点信息
            Service.LoadNetworkNode(new Action<MsgBase>((msg) => {
                Dispatcher.Invoke(new Action(() => {
                    switch (msg.State)
                    {
                        case MsgState.Susuccess:
                            //获取到所有游戏节点
                            foreach (var item in Auth.NetworkNodeList)
                            {
                                NetworkNodeList.Add(item);
                            }

                            int num = 0;
                            //数据加载完成了,接下来PING获取延迟
                            foreach (var item in NetworkNodeList)
                            {
                                num++;//增加数量1
                                Service.PingOne(item.Ip, new Action<MsgBase>((pingmsg) => {
                                    Dispatcher.Invoke(new Action(() => {
                                        item.Ping = pingmsg.Message;
                                        item.PingValue = pingmsg.Value + item.Tunping;
                                    }));
                                    num--;//减少数量1
                                }));
                            }
                            //轮询
                            Task.Factory.StartNew(()=> {
                                while (num>0)
                                {
                                    Thread.Sleep(800);
                                }
                                Dispatcher.Invoke(new Action(() => {
                                    //排序
                                    //Sort("Ping", ListSortDirection.Ascending);
                                    if (NetworkNodeList.Count>=1)
                                    {
                                        SelectNetworkNode = NetworkNodeList[0];
                                    }
                                    foreach (var item in NetworkNodeList)
                                    {
                                        if (item.PingValue<SelectNetworkNode.PingValue)
                                        {
                                            SelectNetworkNode = item;
                                        }
                                    }
                                    dgvShow.SelectedItem = SelectNetworkNode;
                                }));
                            });
                            
                            break;
                        case MsgState.Step:
                            break;
                    }
                }));
            }));
        }

        /// <summary>
        /// 模拟点击列头
        /// </summary>
        /// <param name="c">列名</param>
        /// <param name="d">方向</param>
        private void Sort(string c, ListSortDirection d)
        {
            ICollectionView v = CollectionViewSource.GetDefaultView(dgvShow.ItemsSource);
            v.SortDescriptions.Clear();
            v.SortDescriptions.Add(new SortDescription(c, d));
            v.Refresh();
            this.dgvShow.ColumnFromDisplayIndex(0).SortDirection = d;
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateInfo();
        }

        private void DgvShow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectNetworkNode = dgvShow.SelectedItem as NetworkNode;
        }
    }
}
