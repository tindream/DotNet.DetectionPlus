using DetectionPlus.Camera;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;

namespace DetectionPlus.Sign
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelPlus
    {
        #region 属性
        public string Text { get { return Config.Text; } }
        private string desc = "准备就绪";
        public string Desc
        {
            get { return desc; }
            set { desc = value; RaisePropertyChanged(); }
        }

        #endregion

        #region 命令
        private ICommand selectionCommand;
        public ICommand SelectionCommand
        {
            get
            {
                return selectionCommand ?? (selectionCommand = new RelayCommand<ListViewEXT>(listView1 =>
                {
                    if (listView1.SelectedItem is IListView info)
                    {
                        switch (info.Text)
                        {
                            case "主页":
                                if (Method.Child(listView1, out Frame frame))
                                {
                                    frame.Content = ViewlLocator.GetViewInstance<MonitorPage>();
                                }
                                break;
                            case "历史":
                                if (Method.Child(listView1, out frame))
                                {
                                    frame.Content = ViewlLocator.GetViewInstance<HistroyPage>();
                                }
                                break;
                            case "设置":
                                if (Method.Child(listView1, out frame))
                                {
                                    frame.Content = ViewlLocator.GetViewInstance<SetPage>();
                                }
                                break;
                        }
                        Statu(info.Text);
                    }
                }));
            }
        }
        private ICommand about;
        public ICommand About
        {
            get
            {
                return about ?? (about = new RelayCommand<WindowEXT>(wd =>
                {
                    var version = Assembly.GetExecutingAssembly().GetName().Version;
                    Statu($"v{version}");
                    Method.Toast(wd, $"v{version}");
                }));
            }
        }

        #endregion

        #region 消息
        private void Statu(string msg)
        {
            this.Desc = msg;
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            this.MessengerInstance.Register<StatuMessage>(this, msg => Statu(msg.Message));
        }
    }
}