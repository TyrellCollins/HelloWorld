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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Stopwatch : Page
    {
        public Stopwatch()
        {
            this.InitializeComponent();
        }

        //Create structs required for dispatch timer to function
        DispatcherTimer dispatcherTimer;
        DateTimeOffset startTime;
        DateTimeOffset lastTime;
        DateTimeOffset stopTime;

        int timesTicked = 1;
        int timesToTick = 10; // only necessary if stopping time

        /* byte a = 0; //brightness value
         * byte r = 50; //red value
         * byte g = 50; //green value
         * byte b = 50; //blue value
         */

        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            //IsEnabled defaults to false
            TimerLog.Text += "dispatcherTimer.IsEnabled = " + dispatcherTimer.IsEnabled + "\n";  //for debugging
            startTime = DateTimeOffset.Now;
            lastTime = startTime;
            TimerLog.Text += "Calling dispatcherTimer.Start()\n";  //for debugging purposes
            dispatcherTimer.Start();
            //IsEnabled should now be true after calling start
            TimerLog.Text += "dispatcherTimer.IsEnabled = " + dispatcherTimer.IsEnabled + "\n"; //for debugging
        }

        void DispatcherTimer_Tick(object sender, object e)
        {

            DateTimeOffset time = DateTimeOffset.Now;
            TimeSpan span = time - lastTime;
            lastTime = time;
            //Time since last tick should be very very close to Interval
            TimerLog.Text += timesTicked + "\t time since last tick: " + span.ToString() + "\n"; // for debugging
            timesTicked++;

            /*
            if (a > 255)
            {
                a = 0;
            }
            else a += 51;

            if (r > 255)
            {
                r = 0;
            }
            else r += 5;

            if (g < 0)
            {
                g = 255;
            }
            else g--;

            if (b > 255)
            {
                b = 0;
            }
            else b++;
            */


            if (timesTicked > timesToTick)
            {
                stopTime = time;
                TimerLog.Text += "Calling dispatcherTimer.Stop()\n";// for debugging end dispatch timer
                dispatcherTimer.Stop();
                //IsEnabled should now be false after calling stop
                TimerLog.Text += "dispatcherTimer.IsEnabled = " + dispatcherTimer.IsEnabled + "\n"; // for debugging
                span = stopTime - startTime;
                TimerLog.Text += "Total Time Start-Stop: " + span.ToString() + "\n"; // for debugging
            }



        }
     /*   
        private void Page_Loading(FrameworkElement sender, object args)
        {
            DispatcherTimerSetup(); //Start timer on button click
        }
*/
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimerSetup(); //Start timer on button click
        }
    }
}
