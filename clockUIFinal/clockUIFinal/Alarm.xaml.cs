using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace clockUIFinal
{
   /* This c# file will let the user enter a time into a text box to set an alarm.
      once the string in the text box is equal to the alarm time, buzzer will sound. for 5s */


    public sealed partial class Alarm : Page
    {
        public Alarm()
        {
            this.InitializeComponent();
        }

        private void Alarmset_TextChanged(object sender, TextChangedEventArgs e)
        {
            //set bit to enable set alarm button to be pressed
        }
    }
}
