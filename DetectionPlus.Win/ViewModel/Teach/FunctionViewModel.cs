using DetectionPlus.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus.Win.ViewModel
{
    public class FunctionViewModel : ViewModelPlus
    {
        public FunctionViewModel() { }

        private ICommand checkedCommand;
        public ICommand CheckedCommand
        {
            get
            {
                return checkedCommand ?? (checkedCommand = new RelayCommand<RadioButton>(obj =>
                {
                    Method.Toast(Method.GetTemplateXaml(obj));
                }));
            }
        }
        private ICommand checkBoxCommand;
        public ICommand CheckBoxCommand
        {
            get
            {
                return checkBoxCommand ?? (checkBoxCommand = new RelayCommand<CheckBox>(obj =>
                {
                    Method.Toast(Method.GetTemplateXaml(obj));
                }));
            }
        }

    }
}