using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LinKu.Models
{
    public class GameGroup : INotifyPropertyChanged
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
    }
}
