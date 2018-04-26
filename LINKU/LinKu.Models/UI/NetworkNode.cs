using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace LinKu.Models
{
    public class NetworkNode : INotifyPropertyChanged
    {
        #region UI更新接口
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private string name;
        private string id;
        private string info;
        private string ip;
        private string status;
        private string io;
        private string group;
        private int tunping;
        private string gamename;
        private string ping;
        private int pingvalue;

        public string Name { get => name; set => name = value; }
        public string Id { get => id; set => id = value; }
        public string Info { get => info; set => info = value; }
        public string Ip { get => ip; set => ip = value; }
        public string Status { get => status; set => status = value; }
        public string Io { get => io; set => io = value; }
        public string Group { get => group; set => group = value; }
        public int Tunping { get => tunping; set => tunping = value; }
        public string Gamename { get => gamename; set => gamename = value; }
        /// <summary>
        /// Ping 延迟
        /// </summary>
        public string Ping {
            set
            {
                ping = value;
                OnPropertyChanged("Ping");
            }
            get { return ping; }
        }

        public int PingValue { get => pingvalue; set => pingvalue = value; }
    }
}
