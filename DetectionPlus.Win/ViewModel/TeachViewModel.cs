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
            stepList.Clear();
            stepList.Add(new MenuButtonModel("检测功能"));
            stepList.Add(new MenuButtonModel("物件形状"));
        }

        private List<MenuButtonModel> stepList = new List<MenuButtonModel>();
        public List<MenuButtonModel> StepList { get { return stepList; } }

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