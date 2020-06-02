/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:DetectionPlus.Win"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using DetectionPlus.Win.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;

namespace DetectionPlus.Win
{
    /// <summary>
    /// 单实例
    /// </summary>
    public class ViewlLocator
    {
        /// <summary>
        /// 单实例列表
        /// </summary>
        private static readonly Dictionary<string, object> instanceDic = new Dictionary<string, object>();

        public static T GetViewInstance<T>(object tag = null)
        {
            var type = typeof(T);
            var name = $"{type.Name}-{tag}";
            if (instanceDic.ContainsKey(name)) return (T)instanceDic[name];
            var obj = Activator.CreateInstance(type);
            instanceDic.Add(name, obj);
            return (T)obj;
        }
    }
}