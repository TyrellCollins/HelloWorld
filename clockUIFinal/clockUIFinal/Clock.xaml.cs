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
using static Windows.UI.Color;

// Final Project of Tyrell Collins ECET 230 2018

namespace clockUIFinal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClockPage : Page
    {
        DispatcherTimer Timer = new DispatcherTimer(); //create new instance of dispatch timer
        //DispatcherTimer Stopwatch = new DispatcherTimer(); //create new instance of dispatch timer
        public ClockPage()
        {
            InitializeComponent();
            DataContext = this; 
            Timer.Tick += Timer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 1); //1 second interval
            Timer.Start(); //start timer
        }

        private void Timer_Tick(object sender, object e)
        {
            Time.Text = DateTime.Now.ToString("h:mm:ss tt"); //Displays system time as a string in textblock
        }
    }
}
