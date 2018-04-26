using LinKu.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace LinKu.Windows
{
    /// <summary>
    /// WindowSpeed.xaml 的交互逻辑
    /// </summary>
    public partial class WindowMessage : UserControl, INotifyPropertyChanged
    {
        #region UI更新接口
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public WindowMessage()
        {
            InitializeComponent();
            DataContext = this;
            dgvList.ItemsSource = MessageList;
        }

        ObservableCollection<Message> _MessageList;
        public ObservableCollection<Message> MessageList
        {
            get
            {
                if (_MessageList == null)
                {
                    _MessageList = new ObservableCollection<Message>();
                }
                return _MessageList;
            }
            set
            {
                _MessageList = value;
                OnPropertyChanged("MessageList");
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
            MessageList.Clear();
            //加载网络节点信息
            Service.LoadMessage(new Action<MsgBase>((msg) => {
                Dispatcher.Invoke(new Action(() => {
                    switch (msg.State)
                    {
                        case MsgState.Susuccess:
                            //先取分组
                            foreach (var item in Auth.MessageList)
                            {
                                MessageList.Add(item);
                            }
                            if (MessageList.Count>0)
                            {
                                SelectMessage = MessageList[0];
                                MessageList[0].IsChecked = true;
                            }
                            break;
                        case MsgState.Step:
                            break;
                    }
                }));
            }));
        }

        private Message selectMessage;
        /// <summary>
        /// 游戏名字
        /// </summary>
        public Message SelectMessage
        {
            get
            {
                return selectMessage;
            }
            set
            {
                selectMessage = value;
                OnPropertyChanged("SelectMessage");
            }
        }

        /// <summary>
        /// 切换公告
        /// </summary>
        private void MessageItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton bo)
            {
                if (bo.Tag is Message g)
                {
                    SelectMessage = g;
                }
            }
        }
    }
}
