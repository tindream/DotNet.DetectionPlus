using DetectionPlus.Camera;
using DetectionPlus.HWindowTool;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HalconDotNet;
using Paway.Helper;
using Paway.WPF;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DetectionPlus.Sign
{
    public class CameraSetViewModel : ViewModelPlus
    {
        #region 属性
        public AdminInfo Admin { get { return Config.Admin; } }

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
                            case "连接":
                                Connect(listView1, info);
                                break;
                            case "拍照":
                                Messenger.Default.Send(new PictureMessage() { Obj = listView1 });
                                break;
                            case "重置":
                                Messenger.Default.Send(new ResetMessage() { Obj = listView1 });
                                break;
                            case "Open":
                                Messenger.Default.Send(new OpenMessage() { Obj = listView1 });
                                break;
                            case "Save":
                                DataService.Default.Update(nameof(Config.Admin.CameraName));
                                DataService.Default.Update(nameof(Config.Admin.IsTrigger));
                                DataService.Default.Update(nameof(Config.Admin.ExposureTime));

                                Messenger.Default.Send(new SaveMessage() { Obj = listView1 });

                                Method.Toast(listView1, "保存成功");
                                break;
                        }
                        Messenger.Default.Send(new StatuMessage(info.Text));
                    }
                    listView1.SelectedIndex = -1;
                }));
            }
        }
        private void Connect(ListViewEXT listView1, IListView info)
        {
            Method.Progress(listView1, () =>
            {
                Config.Camera.CameraName = Config.Admin.CameraName;
                if (!Config.Camera.IsOpen)
                {
                    Config.Camera.Connect();
                    Config.Camera.SetTriggerMode(Config.Admin.IsTrigger);
                    Config.Admin.ExposureTime = Config.Camera.ExposureTime;
                    Method.Invoke(listView1, () =>
                    {
                        info.Image = new ImageEXT(new BitmapImage(new Uri("pack://application:,,,/DetectionPlus.Sign;component/Images/disconnect.png")),
                            new BitmapImage(new Uri("pack://application:,,,/DetectionPlus.Sign;component/Images/disconnect_s.png")));
                    });
                }
                else
                {
                    Config.Camera.CameraStop();
                    Config.Camera.CameraClose();
                    Method.Invoke(listView1, () =>
                    {
                        info.Image = new ImageEXT(new BitmapImage(new Uri("pack://application:,,,/DetectionPlus.Sign;component/Images/connect.png")),
                            new BitmapImage(new Uri("pack://application:,,,/DetectionPlus.Sign;component/Images/connect_s.png")));
                    });
                }
            });
        }

        private ICommand valueChanged;
        public ICommand ValueChanged
        {
            get
            {
                return valueChanged ?? (valueChanged = new RelayCommand<SliderEXT>(slider =>
                {
                    //曝光
                    var value = (float)slider.Value;
                    Config.Camera.InitExposureTime = value;
                    Config.Camera.ExposureTime = value;
                    Config.Admin.ExposureTime = value;
                }));
            }
        }

        #endregion

        public CameraSetViewModel() { }
    }
}