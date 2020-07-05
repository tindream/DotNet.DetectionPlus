using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
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
        #region 命令
        private ICommand executeCommand;
        public ICommand ExecuteCommand
        {
            get
            {
                return executeCommand ?? (executeCommand = new RelayCommand<ButtonEXT>(btn =>
                {
                    if (Expand.Run(out int result, "B"))
                    {
                        Method.Toast(btn, "Hello, " + result);
                    }
                }));
            }
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
        }
    }
}