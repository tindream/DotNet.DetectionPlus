using DetectionPlus.Message;
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
    public class BinaryViewModel : ViewModelPlus
    {
        public BinaryViewModel() { }

        private ICommand less;
        public ICommand Less
        {
            get
            {
                return less ?? (less = new RelayCommand<Slider>(slider =>
                {
                    slider.Value--;
                }));
            }
        }
        private ICommand add;
        public ICommand Add
        {
            get
            {
                return add ?? (add = new RelayCommand<Slider>(slider =>
                {
                    slider.Value++;
                }));
            }
        }
        private ICommand valueChangedCommand;
        public ICommand ValueChangedCommand
        {
            get
            {
                return valueChangedCommand ?? (valueChangedCommand = new RelayCommand<Slider>(slider =>
                {
                    this.MessengerInstance.Send(new BinaryMessage(slider.Value));
                }));
            }
        }
    }
}