using LinKu.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace LinKu.Windows
{
    /// <summary>
    /// WindowSpeed.xaml 的交互逻辑
    /// </summary>
    public partial class WindowGame : UserControl, INotifyPropertyChanged
    {
        #region UI更新接口
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public WindowGame()
        {
            InitializeComponent();
            DataContext = this;
            dgvAddList.ItemsSource = GameGroupList;
            GamedgvList.ItemsSource = null;
            GamedgvList.ItemsSource = GameList;
        }

        ObservableCollection<GameGroup> _GameGroupList;
        /// <summary>
        /// 游戏分组
        /// </summary>
        public ObservableCollection<GameGroup> GameGroupList
        {
            get
            {
                if (_GameGroupList == null)
                {
                    _GameGroupList = new ObservableCollection<GameGroup>();
                }
                return _GameGroupList;
            }
            set
            {
                _GameGroupList = value;
                OnPropertyChanged("GameGroupList");
            }
        }

        /// <summary>
        /// 热门游戏排行榜
        /// </summary>
        ObservableCollection<Game> _GameHotList;
        /// <summary>
        /// 热门游戏 5 个
        /// </summary>
        public ObservableCollection<Game> GameHotList
        {
            get
            {
                if (_GameHotList == null)
                {
                    _GameHotList = new ObservableCollection<Game>();
                }
                return _GameHotList;
            }
            set
            {
                _GameHotList = value;
                OnPropertyChanged("GameHotList");
            }
        }

        /// <summary>
        /// 热门游戏排行榜
        /// </summary>
        ObservableCollection<Game> _GameList;
        /// <summary>
        /// 全部游戏
        /// </summary>
        public ObservableCollection<Game> GameList
        {
            get
            {
                if (_GameList == null)
                {
                    _GameList = new ObservableCollection<Game>();
                }
                return _GameList;
            }
            set
            {
                _GameList = value;
                OnPropertyChanged("GameList");
            }
        }

        /// <summary>
        /// 页面读取次数
        /// </summary>
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
            //读取游戏列表 跟 分组
            GameHotList.Clear();
            GameList.Clear();
            //加载网络节点信息
            Service.LoadGameList(new Action<MsgBase>((msg) => {
                Dispatcher.Invoke(new Action(() => {
                    switch (msg.State)
                    {
                        case MsgState.Susuccess:
                            //先取分组
                            foreach (var item in Auth.GameGroupList)
                            {
                                GameGroupList.Add(item);
                            }
                            //再去取游戏
                            foreach (var item in Auth.GameList)
                            {
                                GameList.Add(item);
                                if (GameHotList.Count<5)
                                {
                                    GameHotList.Add(item);
                                }
                            }
                            break;
                        case MsgState.Step:
                            break;
                    }
                }));
            }));
        }



        /// <summary>
        /// 切换游戏分组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameGroup_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton ra)
            {
                if (ra.Tag is GameGroup gm)
                {
                    if (gm.Name=="热门游戏")
                    {
                        GamedgvList.Visibility = Visibility.Collapsed;
                        GameGroupHot.Visibility = Visibility.Visible;
                        //加载
                    }
                    else
                    {
                        GameGroupHot.Visibility = Visibility.Collapsed;
                        GamedgvList.Visibility = Visibility.Visible;
                        //加载 游戏类型
                    }

                    if (gm.Name == "全部游戏")
                    {
                        FillGame("");
                    }
                    else if (gm.Name == "Steam游戏")
                    {
                        FillGame("Steam");
                    }
                    else if (gm.Name == "Origin游戏")
                    {
                        FillGame("Origin");
                    }
                    else if (gm.Name == "Battle游戏")
                    {
                        FillGame("Battle");
                    }
                    else//其他游戏
                    {
                        FillGame("Other");
                    }

                }
            }
        }

        /// <summary>
        /// 填充游戏
        /// </summary>
        public void FillGame(string key)
        {
            GameList.Clear();
            //再去取游戏
            foreach (var item in Auth.GameList)
            {
                if (item.Group.Contains(key))
                {
                    GameList.Add(item);
                }
            }
            UpdateView();
        }

        /// <summary>
        /// 游戏分组滚动到最上面
        /// </summary>
        public void UpdateView() {
            GamedgvList.BringIntoView(new Rect(0,0,0,0));
        }

        /// <summary>
        /// 搜索 填充游戏
        /// </summary>
        public void SerachGame()
        {
            string key = TbSerach.Text.Trim();
            GameGroupHot.Visibility = Visibility.Collapsed;
            GamedgvList.Visibility = Visibility.Visible;
            GameGroupList[0].IsChecked = false;
            GameGroupList[1].IsChecked = true;

            GameList.Clear();
            //再去取游戏
            foreach (var item in Auth.GameList)
            {
                if (item.Group.ToUpper().Contains(key.ToUpper()) || item.Name.ToUpper().Contains(key.ToUpper()))
                {
                    GameList.Add(item);
                }
            }
            UpdateView();
        }

        #region 热门游戏切换
        /// <summary>
        /// 向左移动1位
        /// </summary>
        private void LeftBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Game g = GameHotList[GameHotList.Count-1];//取出最后
            GameHotList.RemoveAt(GameHotList.Count - 1);
            GameHotList.Insert(0,g);
        }
        /// <summary>
        /// 向右移动1位
        /// </summary>
        private void RightBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Game g = GameHotList[0];//取出最后
            GameHotList.RemoveAt(0);
            GameHotList.Add(g);
        }
        /// <summary>
        /// 向左移动2位
        /// </summary>
        private void LeftBorder2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Game g = GameHotList[GameHotList.Count - 1];//取出最后
            GameHotList.RemoveAt(GameHotList.Count - 1);
            GameHotList.Insert(0, g);

            Game g2 = GameHotList[GameHotList.Count - 1];//取出最后
            GameHotList.RemoveAt(GameHotList.Count - 1);
            GameHotList.Insert(0, g2);
        }
        /// <summary>
        /// 向右移动2位
        /// </summary>
        private void RightBorder2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Game g = GameHotList[0];//取出最后
            GameHotList.RemoveAt(0);
            GameHotList.Add(g);

            Game g2 = GameHotList[0];//取出最后
            GameHotList.RemoveAt(0);
            GameHotList.Add(g2);
        }
        #endregion

        /// <summary>
        /// 搜索
        /// </summary>
        private void BtnSerach_Click(object sender, RoutedEventArgs e)
        {
            SerachGame();
        }
        /// <summary>
        /// 搜索
        /// </summary>
        private void TbSerach_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            SerachGame();
        }

        /// <summary>
        /// 切换到目标游戏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border bo)
            {
                if (bo.Tag is Game g)
                {
                    TurnGame(g);
                }
            }
        }

        public void TurnGame(Game name)
        {
            App.main.TurnGame(name);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button bo)
            {
                if (bo.Tag is Game g)
                {
                    TurnGame(g);
                }
            }
        }
    }
}
