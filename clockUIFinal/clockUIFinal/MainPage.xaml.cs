using serialPortUWPv2;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace clockUIFinal
{
    public sealed partial class MainPage : Page
    {
        private SerialDevice serialPort = null;
        private SolarCalc solarCalc = new SolarCalc(); //will need to modify

        DataWriter dataWriterObject = null;
        DataReader dataReaderObject = null;

        private ObservableCollection<DeviceInformation> listOfDevices;

        private CancellationTokenSource ReadCancellationTokenSource;

        DispatcherTimer Timer = new DispatcherTimer(); //create new instance of dispatch timer
        DispatcherTimer StopwatchTimer = new DispatcherTimer(); //create new instance of dispatch timer

        string received = "";
        

        public MainPage()
        {
            this.InitializeComponent();
            listOfDevices = new ObservableCollection<DeviceInformation>();
            ListAvailablePorts();
            DataContext = this;
            Timer.Tick += Timer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 1); //1 second interval
            Timer.Start(); //start timer

            //StopwatchTimer_Setup();

            //Start Stopwatch 
            // StopwatchTimer.Tick += StopwatchTimer_Tick;
            
        }

      //Create structs required for dispatch timer to function
        DispatcherTimer dispatcherTimer;
        DateTimeOffset startTime;
        DateTimeOffset lastTime;
        DateTimeOffset stopTime;
        DateTimeOffset previous;
        DateTimeOffset starttimernow;
        TimeSpan elapsed, span;

        
        int timesTicked = 1;
        int timesToTick = 100000000; // only necessary if stopping time
        int swTicked = 1;
        int swToTick = 1000000000; // only necessary if stopping time


            public void DispatcherTimerSetup()
            {
                
            dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += DispatcherTimer_Tick;
                //dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1); //smoother count
                //dispatcherTimer.Interval = new TimeSpan(0, 0, 1, 0, 0);

                startTime = DateTimeOffset.Now;

              //  Timertimer.Text = span.ToString(startTime - DateTimeOffset.Now);
                lastTime = startTime;
                //~~~TimerLog.Text += "Calling dispatcherTimer.Start()\n";  //for debugging purposes
                dispatcherTimer.Start();

            }
    /*
            public void StopwatchConfig()
            {
                DateTimeOffset startwatch = DateTimeOffset.Now;
                DateTimeOffset stopwatch = DateTimeOffset.Now;
                TimeSpan StopWatchTotal = stopwatch - startwatch;
                string counting = StopWatchTotal.ToString();
                Timertimer.Text = counting;

            }

            public void CalculateTime()
            {

                DateTime date1, Starttimernow;
                DateTimeOffset dateOffset1, dateOffset2;
                TimeSpan difference;

                // Find difference between Date.Now and Date.UtcNow
                date1 = DateTime.Now;
               // date2 = DateTime.UtcNow;
                difference = Starttimernow - date1;

                // Find difference between Now and UtcNow using DateTimeOffset
                dateOffset1 = DateTimeOffset.Now;
               // dateOffset2 = DateTimeOffset.UtcNow;
                difference = dateOffset1 - dateOffset2;
                Timertimer.Text = difference.ToString();
            }

    */
             
            void DispatcherTimer_Tick(object sender, object e)
            {
           
          
                DateTimeOffset time = DateTimeOffset.Now;
                TimeSpan span = time - lastTime;

                lastTime = time;

                timesTicked++;
 

                if (timesTicked > timesToTick)
                {
                    stopTime = time;

                    //~~TimerLog.Text += "Calling dispatcherTimer.Stop()\n";// for debugging end dispatch timer

                    dispatcherTimer.Stop();
                    //IsEnabled should now be false after calling stop
                    //~~~TimerLog.Text += "dispatcherTimer.IsEnabled = " + dispatcherTimer.IsEnabled + "\n"; // for debugging

                    span = stopTime - startTime;

                   TimerLog.Text = "Stop Time: " + span.ToString(); // for debugging

                    //~TimerLog.Text += "Total Time Start-Stop: " + span.ToString() + "\n"; // for debugging
                }
            }

            private void Timer_Tick(object sender, object e)
            {
            Boolean toggleBit = false;
            RTC.Text = DateTime.Now.ToString("h:mm:ss tt"); //Displays system time as a string in textblock

                DateTimeOffset stopwatchtimer;
                TimeSpan timertotal;
                // Timertimer.Text = DateTime.ToString("h:mm:ss tt");
                // DateTimeOffset active = previous;
                DateTimeOffset present = DateTimeOffset.Now;
                //TimeSpan span = active - present;
                TimeSpan elapsed = present - starttimernow;
            while(toggleBit==true)
            { 
                Timertimer.Text =  elapsed.ToString(); //test will edit
               }                                                   //DateTimeOffset startwatchtimer;
          

            // previous = present;



            swTicked++;

            //Timertimer.Text = "Time:  %s  PM " + elapsed.ToString(); //test will edit

            if (swTicked > swToTick)
            {
                toggleBit = false;
                stopwatchtimer = present;

                StopwatchTimer.Stop();

                timertotal = stopwatchtimer - starttimernow;

                Timertimer.Text = "Time since Started: " + timertotal.ToString(); // for debugging

            }

        }

        public void StopwatchTimer_Setup()
        {


            StopwatchTimer = new DispatcherTimer();
            StopwatchTimer.Interval = new TimeSpan(0, 0, 1);
            starttimernow = DateTimeOffset.Now;
            StopwatchTimer.Start();


        }

/*
        public void StopwatchTimer_Tick(object sender, object e)
        {
            //DateTimeOffset startwatchtimer;
            DateTimeOffset stopwatchtimer;
            TimeSpan timertotal; 
            // Timertimer.Text = DateTime.ToString("h:mm:ss tt");
           // DateTimeOffset active = previous;
            DateTimeOffset present = DateTimeOffset.Now;
            //TimeSpan span = active - present;
            TimeSpan elapsed = present - starttimernow;

           // previous = present;



            swTicked++;

            //Timertimer.Text = "Time:  %s  PM " + elapsed.ToString(); //test will edit

            if (swTicked > swToTick)
            {
                stopwatchtimer = present;

                StopwatchTimer.Stop();
 
                timertotal = stopwatchtimer - starttimernow;

                Timertimer.Text = "Total Time: " + timertotal.ToString(); // for debugging

            }
        }
        */

        private async void ListAvailablePorts()
            {
                try
                {
                    string aqs = SerialDevice.GetDeviceSelector();
                    var dis = await DeviceInformation.FindAllAsync(aqs);

                    for (int i = 0; i < dis.Count; i++)
                    {
                        listOfDevices.Add(dis[i]);
                    }

                    lstSerialDevices.ItemsSource = listOfDevices;

                    lstSerialDevices.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    txtMessage.Text = ex.Message;
                    //txtMessage.Text = "List Port" + ex.Message;
                }
            }

            private void ButtonConnectToDevice_Click(object sender, RoutedEventArgs e)
            {
                SerialPortConfiguration();
            }

            private async void SerialPortConfiguration()
            {
                var selection = lstSerialDevices.SelectedItems;

                if (selection.Count <= 0)
                {
                    txtMessage.Text = "Select an object for serial connection!";
                    return;
                }

                DeviceInformation entry = (DeviceInformation)selection[0];

                try
                {
                    serialPort = await SerialDevice.FromIdAsync(entry.Id);
                    serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                    serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                    serialPort.BaudRate = 115200;
                    serialPort.Parity = SerialParity.None;
                    serialPort.StopBits = SerialStopBitCount.One;
                    serialPort.DataBits = 8;
                    serialPort.Handshake = SerialHandshake.None;
                    txtMessage.Text = "Serial port correctly configured!";

                    ReadCancellationTokenSource = new CancellationTokenSource();
                    Listen();
                }
                catch (Exception ex)
                {
                    txtMessage.Text = ex.Message;
                    //txtMessage.Text = "Port Config" + ex.Message;
                }
            }

            private async void Listen()
            {
                try
                {
                    if (serialPort != null)
                    {
                        dataReaderObject = new DataReader(serialPort.InputStream);

                        while (true)
                        {
                            await ReadData(ReadCancellationTokenSource.Token);
                        }
                    }
                }
                catch (Exception ex)
                {
                    txtMessage.Text = ex.Message;
                    //txtMessage.Text = "Listen" + ex.Message;
                    //received = "";

                    //if (ex.GetType.Name=="TaskCancelledException")

                }
                finally
                {

                }

            }
            private async Task ReadData(CancellationToken cancellationToken)
            {
                Task<UInt32> loadAsyncTask;

                int calChkSum = 0;
                int recChkSum = 0;
                int an0, an1, an2, an3, an4, an5;

                uint ReadBufferLength = 1;

                cancellationToken.ThrowIfCancellationRequested();

                dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

                loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);

                UInt32 bytesRead = await loadAsyncTask;

                if (bytesRead > 0)
                {
                    received += dataReaderObject.ReadString(bytesRead);
                    //txtReceived.Text = received + txtReceived.Text;
                    if (received[0] == '#')
                    {
                        if (received.Length > 3)
                        {
                            if (received[2] == '#')
                            {
                                //txtReceived.Text = received;
                                if (received.Length > 42)
                                {
                                    txtReceived.Text = received + txtReceived.Text;
                                    //parse code
                                    txtPacketNum.Text = received.Substring(3, 3);
                                    txtAN0.Text = received.Substring(6, 4);
                                    txtAN1.Text = received.Substring(10, 4);
                                    txtAN2.Text = received.Substring(14, 4);
                                    txtAN3.Text = received.Substring(18, 4);
                                    txtAN4.Text = received.Substring(22, 4);
                                    txtAN5.Text = received.Substring(26, 4);
                                    txtBinOut.Text = received.Substring(30, 8);
                                    txtChkSum.Text = received.Substring(38, 3);

                                    for (int i = 3; i < 38; i++)
                                    {
                                        calChkSum += (byte)received[i];
                                    }
                                    txtCalChkSum.Text = Convert.ToString(calChkSum);
                                    an0 = Convert.ToInt32(received.Substring(6, 4));
                                    an1 = Convert.ToInt32(received.Substring(10, 4));
                                    an2 = Convert.ToInt32(received.Substring(14, 4));
                                    an3 = Convert.ToInt32(received.Substring(18, 4));
                                    an4 = Convert.ToInt32(received.Substring(22, 4));
                                    an5 = Convert.ToInt32(received.Substring(26, 4));

                                    recChkSum = Convert.ToInt32(received.Substring(38, 3));
                                    calChkSum %= 1000;
                                    if (recChkSum == calChkSum)
                                    {
                                        txtSolarVolt.Text = solarCalc.GetSolarVoltage(an0);
                                        txtBatteryVolt.Text = solarCalc.GetBatteryVoltage(an2);
                                        txtBatteryCurrent.Text = solarCalc.GetBatteryCurrent(an1, an2);
                                        txtLED1current.Text = solarCalc.GetLEDcurrent(an4, an1);
                                        txtLED2current.Text = solarCalc.GetLEDcurrent(an3, an1);

                                    }


                                    received = "";
                                }

                            }
                            else
                            {
                                received = "";
                            }
                        }
                    }
                    else
                    {
                        received = "";
                    }
                }


            }

            private async void ButtonWrite_Click(object sender, RoutedEventArgs e)
            {
                if (serialPort != null)
                {
                        var dataPacket = txtSend.Text.ToString();
                        dataWriterObject = new DataWriter(serialPort.OutputStream);
                        await SendPacket(dataPacket);

                    if (dataWriterObject != null)
                    {
                        dataWriterObject.DetachStream();
                        dataWriterObject = null;
                    }

                }
            }

            private async Task SendPacket(string value)
            {
                var dataPacket = value;

                Task<UInt32> storeAsyncTask;

                if (dataPacket.Length != 0)
                {
                    dataWriterObject.WriteString(dataPacket);

                    storeAsyncTask = dataWriterObject.StoreAsync().AsTask();

                    UInt32 bytesWritten = await storeAsyncTask;


                    if (bytesWritten > 0)
                    {
                        txtMessage.Text = "Value sent correcly";

                    }
                }

                else
                {
                    txtMessage.Text = "No Value Sent";
                }

            }

            private void SetalarmButton_Click(object sender, RoutedEventArgs e)
            {
                AlarmTime.Text = Alarmset.Text;
                //AlarmTime.Text = "Alarm set for:  " + Alarmset.Text;
            }

            private void Startdebug_Click(object sender, RoutedEventArgs e)
            {
                //DispatcherTimerSetup(); //Start debugging on button click

                DateTimeOffset startTime = DateTimeOffset.Now;
             }

            private void Startstopwatch_Click(object sender, RoutedEventArgs e)
            {
            swTicked = swToTick + 1; //stop timer function
            DateTimeOffset stopwatchtimer = DateTimeOffset.Now;
            Boolean toggleBit = true;
            StopwatchTimer_Setup(); //Start timer on button click start timer
            

           // DateTimeOffset startwatchtimer = DateTimeOffset.Now;
               // DateTimeOffset stopwatchtimer = DateTimeOffset.Now;
             }


            private void Stopstopwatch_Click(object sender, RoutedEventArgs e)
            {
            swTicked = swToTick + 1; //stop timer function
            DateTimeOffset stopwatchtimer = DateTimeOffset.Now;
                
             }

            private void StopDebug_Click(object sender, RoutedEventArgs e)
            {
               swTicked = swToTick + 1; //stop timer function
            DateTimeOffset stopTime = DateTimeOffset.Now;

             }

            private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
            {
                //place holder
            }

            private void AlarmTime_TextChanged(object sender, RoutedEventArgs e)
            {
                if (AlarmTime.Text == RTC.Text)
                {
                    Alarmset.Text = "Timer done";
                    AlarmTime.Text = "Alarm! Buzz!";
                    //DateTimeOffset stopwatchtimer = DateTimeOffset.Now;

                }
            }

        private void Alarmset_TextChanged(object sender, TextChangedEventArgs e)
        {
           
                if (Alarmset.Text != RTC.Text)
                {
                    //add error checking here
                }

            }
        }// end main page
    
    }//end clockui

