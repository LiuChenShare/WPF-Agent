using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LinKu.Models
{
    public class Message : INotifyPropertyChanged
    {
        #region UI更新接口
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private string id;
        private string date;
        private string title;
        private string announcement;
        private string name;



        public string Id { get => id; set => id = value; }

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

        public string Date { get => date; set => date = value; }
        public string Announcement {
            set
            {
                announcement = value;
                OnPropertyChanged("Announcement");
            }
            get { return announcement; }
        }
        public string Title
        {
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
            get { return title; }
        }

        public string Name
        {
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
            get { return name; }
        }
    }
}
