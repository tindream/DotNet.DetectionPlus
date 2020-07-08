using Paway.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DetectionPlus
{
    /// <summary>
    /// Info基类
    /// </summary>
    [Serializable]
    public class BaseInfo : IId, INotifyPropertyChanged, IModel
    {
        [NoShow]
        public int Id { get; set; }
        public virtual string Desc() { return null; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged()
        {
            OnPropertyChanged(Method.GetLastModelName());
        }
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
