using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LinKu.Models
{
    public class Game : INotifyPropertyChanged,IComparable
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
        private string image;
        private string group;
        private int hot;

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }

        bool isChecked = false;
        public bool IsChecked
        {
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
            get { return isChecked; }
        }
        public string Image { get => image; set => image = value; }
        public string Group { get => group; set => group = value; }
        public int Hot { get => hot; set => hot = value; }

        public int CompareTo(object obj)
        {
            Game ob = (Game)obj;
            if (ob.Hot>Hot)
            {
                return -1;
            }
            else if (ob.Hot == Hot)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
