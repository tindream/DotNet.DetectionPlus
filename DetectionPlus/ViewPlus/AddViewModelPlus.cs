using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.Helper;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus
{
    public class AddViewModelPlus<T> : ViewModelPlus where T : class, IModel, IId
    {
        #region 属性
        private T info;
        private T normal;
        public T Info
        {
            get { return info; }
            set
            {
                normal = value;
                info = value.Clone();
                Title = info.Id == 0 ? $"新加{info.GetType().Description()}" : $"编辑{info.GetType().Description()} - {info.Desc()}";
                RaisePropertyChanged();
            }
        }
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }

        #endregion

        #region 命令
        private ICommand cancel;
        public ICommand Cancel
        {
            get
            {
                return cancel ?? (cancel = new RelayCommand<Window>(wd =>
                {
                    wd.DialogResult = false;
                }));
            }
        }
        protected virtual void OnSave(Window wd, T info) { }
        private ICommand save;
        public ICommand Save
        {
            get
            {
                return save ?? (save = new RelayCommand<Window>(wd =>
                {
                    OnSave(wd, info);
                    info.Clone(normal);
                    wd.DialogResult = true;
                }));
            }
        }

        #endregion

        public AddViewModelPlus() { }
    }
}