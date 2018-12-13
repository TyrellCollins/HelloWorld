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
        private AnalogCalc AnalogCalc = new AnalogCalc(); //will need to modify

        DataWriter dataWriterObject = null;
        DataReader dataReaderObject = null;

        private ObservableCollection<DeviceInformation> listOfDevices;
        private CancellationTokenSource ReadCancellationTokenSource;

        DispatcherTimer Timer = new DispatcherTimer(); //create new instance of dispatch timer
        DispatcherTimer StopwatchTimer = new DispatcherTimer(); //create new instance of dispatch timer

      //Create structs required for dispatch timer to function
        DispatcherTimer dispatcherTimer, stopwatchtime;
        DateTimeOffset startTime, stopTime, lastTime, previous, starttimernow, stopwatchtimer;
        TimeSpan elapsed, span, timertotal;
        Boolean toggleBit, debugActive;
        int returncurrentPacketConverted, convertedPacketslost, VerifiedRx;

        string loststring;
        string received = "";
        int currentPacketConverted, pastPacketConverted, packetsLost, verifiedRx;

        public MainPage()
        {
            this.InitializeComponent();
            listOfDevices = new ObservableCollection<DeviceInformation>();
            ListAvailablePorts();
            DataContext = this;
            Timer.Tick += Timer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 1); //1 second interval
            Timer.Start(); //start timer
            string currentPacket;
        }

        int timesTicked, swTicked = 1;
        int timesToTick, swToTick = 100000000; // only necessary if stopping time

        public void DispatcherTimerSetup()
        {

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1); //smoother count

            startTime = DateTimeOffset.Now;

            lastTime = startTime;
            //DebugConsole.Text += "Calling dispatcherTimer.Start()\n";  //for debugging purposes
            dispatcherTimer.Start();
        }

        void DispatcherTimer_Tick(object sender, object e)
        {

            DateTimeOffset time = DateTimeOffset.Now;
            TimeSpan span = time - lastTime;

            lastTime = time;
            timesTicked++;

            if (timesTicked > timesToTick)
            {
                stopTime = time;

                //DebugConsole.Text += "Calling dispatcherTimer.Stop()\n";// for debugging end dispatch timer

                dispatcherTimer.Stop();
                //IsEnabled should now be false after calling stop
                //DebugConsole.Text += "dispatcherTimer.IsEnabled = " + dispatcherTimer.IsEnabled + "\n"; // for debugging

                span = stopTime - startTime;

                //DebugConsole.Text = "Stop Time: " + span.ToString(); // for debugging

                //DebugConsole.Text += "Total Time Start-Stop: " + span.ToString() + "\n"; // for debugging
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            string text = Alarmset.Text;
            DateTimeOffset present = DateTimeOffset.Now;
            TimeSpan elapsed = present - starttimernow;

            RTC.Text = DateTime.Now.ToString("h:mm:ss tt"); //Displays system time as a string in textblock           
                if (text == RTC.Text)
                {
                    Alarmset.Text = "Timer done";
                    AlarmTime.Text = "Alarm! Buzz!";
                }

            while (toggleBit == true && debugActive == false)
            //while (toggleBit == true)
            {
                Timertimer.Text = elapsed.ToString("h:mm:ss tt"); //test will edit
            }

            while (debugActive == true)
            {
                Timertimer.Text = "Debugging"; //test will edit
            }

            string currentPacket = txtPacketNum.Text;


            swTicked++;

            if (swTicked > swToTick)
            {
                toggleBit = false;
                stopwatchtimer = present;

                StopwatchTimer.Stop();

                timertotal = stopwatchtimer - starttimernow;

                Timertimer.Text = "Time Interval:  " + timertotal.ToString(); // for debugging

                if (swToTick == 1)
                {
                    Timertimer.Text = "End Time: " + timertotal.ToString(); // for debugging
                }
            }
        }
    
        public void StopwatchTimer_Setup()
        {
            StopwatchTimer = new DispatcherTimer();
            StopwatchTimer.Interval = new TimeSpan(0, 0, 1);
            starttimernow = DateTimeOffset.Now;
            StopwatchTimer.Start();
        }

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

        private void AlarmTime_TextChanged(object sender, RoutedEventArgs e)
        {

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

        private void Clear_Alarm_Click(object sender, RoutedEventArgs e)
        {
            Alarmset.Text = "Enter time here";
            AlarmTime.Text = "Enter a new time for alarm to sound";
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


                        /* string currentPacket = txtPacketNum.Text;
                        // ConvertTo (currentPacket);
                        pastPacketConverted = currentPacketConverted;
                        currentPacket = txtPacketNum.Text; 
                         currentPacketConverted = Convert.ToInt32(currentPacket);
                        */

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

                                    txtBuzzerval.Text = AnalogCalc.GetBuzzer(an0);
                                        txtRval.Text = AnalogCalc.GetRed(an1);
                                        txtGval.Text = AnalogCalc.GetGreen(an2);
                                        txtBval.Text = AnalogCalc.GetBlue(an3);
                                         txtLightvalue.Text = AnalogCalc.LightDetect(an4);
                                        txtTogglev.Text = AnalogCalc.BuzzerToggle(an5);
/*                                    returncurrentPacketConverted = Convert.ToInt32(currentPacket);

                                    if (returncurrentPacketConverted == currentPacketConverted)
                                    {
                                        verifiedRx++;
                                    }

                                    else
                                        {
                                        packetsLost++;
                                        loststring = Convert.ToString(packetsLost);
                                        txtnumPacketslost.Text = loststring;
                                        //txtnumPacketslost.Text = convertedPacketslost;
                                    }
                                    */
                                    
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
                AlarmTime.Text = "Alarm set for: " + Alarmset.Text;
             }

        private void Startdebug_Click(object sender, RoutedEventArgs e)
        { }

            /*
                private void Startdebug_Click(object sender, RoutedEventArgs e)
                {
                  debugActive = true; 
                    if (debugActive == true)
                    {
                        DispatcherTimerSetup(); //Start debugging on button click
                        swTicked = swToTick + 1; //stop timer function
                        DateTimeOffset startTime = DateTimeOffset.Now;
                    DebugConsole.Text = " Elapsed Time:  " + timertotal.ToString(); // for debugging
                    }
                 }

        */
            private void Startstopwatch_Click(object sender, RoutedEventArgs e)
        {
            debugActive = false;
            if (debugActive == false)
            {
                {
                    swTicked = swToTick + 1; //stop timer function
                    DateTimeOffset stopwatchtimer = DateTimeOffset.Now;
                    Boolean toggleBit = true;
                    StopwatchTimer_Setup(); //Start timer on button click start timer
                }
            }
        }
        private void Stopstopwatch_Click(object sender, RoutedEventArgs e)
        {
            if (debugActive == false)
            {
                swTicked = 1; //stop timer function
            }
        }

        private void StopDebug_Click(object sender, RoutedEventArgs e)
        { }
            /*
            private void StopDebug_Click(object sender, RoutedEventArgs e)
            {
                if (debugActive == true)
                {
                    DateTimeOffset stopTime = DateTimeOffset.Now;
                    swTicked = 1; //stop timer function

                    debugActive = false;
                }
            }
            */
            private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
            {
                //place holder
            }
        
        }// end main page
    
    }//end clockui

