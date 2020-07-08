/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:DetectionPlus.Sign"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace DetectionPlus.Sign
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public static ViewModelLocator Default { get; } = new ViewModelLocator();
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

            SimpleIoc.Default.Register<HistroyQueryModel>();
            SimpleIoc.Default.Register<HistroyViewModel>();
            SimpleIoc.Default.Register<SystemSetViewModel>();
            SimpleIoc.Default.Register<SetViewModel>();
            SimpleIoc.Default.Register<MonitorViewModel>();
            SimpleIoc.Default.Register<CameraSetViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public HistroyQueryModel HistroyQuery { get { return ServiceLocator.Current.GetInstance<HistroyQueryModel>(); } }
        public HistroyViewModel Histroy { get { return ServiceLocator.Current.GetInstance<HistroyViewModel>(); } }
        public SystemSetViewModel SystemSet { get { return ServiceLocator.Current.GetInstance<SystemSetViewModel>(); } }
        public SetViewModel Set { get { return ServiceLocator.Current.GetInstance<SetViewModel>(); } }
        public MonitorViewModel Monitor { get { return ServiceLocator.Current.GetInstance<MonitorViewModel>(); } }
        public CameraSetViewModel CameraSet { get { return ServiceLocator.Current.GetInstance<CameraSetViewModel>(); } }
        public MainViewModel Main { get { return ServiceLocator.Current.GetInstance<MainViewModel>(); } }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}