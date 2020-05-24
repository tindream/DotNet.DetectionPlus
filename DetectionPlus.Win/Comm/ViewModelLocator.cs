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

namespace DetectionPlus.Win
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<TeachViewModel>();
        }

        /// <summary>
        /// View和ViewModel之间不再直接引用，而是通过ViewModelLocator
        /// <para>DataContext="{Binding Main ,Source={StaticResource Locator}}"</para>
        /// <para>x:Key="Locator"在App.xaml全局定义</para>
        /// <para>单例，可以在全局引用绑定</para>
        /// </summary>
        public MainViewModel Main { get { return ServiceLocator.Current.GetInstance<MainViewModel>(); } }
        public TeachViewModel Teach { get { return ServiceLocator.Current.GetInstance<TeachViewModel>(); } }
        public ShootViewModel Shoot { get { return ServiceLocator.Current.GetInstance<ShootViewModel>(); } }
        public FunctionViewModel Function { get { return ServiceLocator.Current.GetInstance<FunctionViewModel>(); } }


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}