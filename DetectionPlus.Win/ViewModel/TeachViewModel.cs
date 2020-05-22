using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus.Win.ViewModel
{
    public class TeachViewModel : ViewModelPlus
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public TeachViewModel()
        {
            //Colors.Transparent
        }

        private ICommand m_ChangeTitleCmd;
        public ICommand ChangeTitleCmd
        {
            get
            {
                return m_ChangeTitleCmd ?? (m_ChangeTitleCmd = new RelayCommand(UpdateTitle));
            }
        }
        private void UpdateTitle()
        {
        }
    }
}