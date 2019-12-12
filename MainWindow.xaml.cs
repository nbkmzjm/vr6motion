using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Timers;

namespace vr6Motion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String[] ports;
        SerialPort port;
        bool isConnected = false;

        int deadZone = 0;
        int PWMrev = 0;
        int PWMmin = 0;
        int PWMmax = 0;
        int maxLimit = 0;
        int clipInput = 0;
        int stepValue;
        int Kp = 0;
        int Ki = 0;
        int Kd = 0;
        int Ks = 0;

        int Feedback = 0;
        int Target = 0;

        System.Timers.Timer aTimer = new System.Timers.Timer();
        System.Timers.Timer rndTimer = new System.Timers.Timer();



        private void SerialDataProcess(char[] data)
        {

            //Debug.Write("0: ");
            //Debug.Write(data[0]);
            //Debug.Write("=");
            //Debug.WriteLine((int)data[0]);
            //Debug.Write("1: ");
            //Debug.Write(data[1]);
            //Debug.Write("=");
            //Debug.WriteLine((int)data[1]);
            //Debug.Write("2: ");
            //Debug.Write(data[2]);
            //Debug.Write("=");
            //Debug.WriteLine((int)data[2]);

            //Debug.WriteLine("=====");


            //byte[] bytes = Encoding.ASCII.GetBytes(data);
            //Debug.WriteLine(char.ToString((char)bytes[0]));
            switch (data[0])
            {

                case 'A':
                    Feedback = (int)data[1] * 4;
                    FeedBackBarValue.Value = Feedback;
                    Target = (int)data[2] * 4;
                    TargetBarValue.Value = Target;
                    break;
                case 'B':
                    Feedback = (int)data[1] * 4;
                    FeedBackBarValue.Value = Feedback;
                    Target = (int)data[2] * 4;
                    TargetBarValue.Value = Target;
                    break;

                case 'D':
                    Kp = (int)data[1] * 256 + (int)data[2];
                    KpValue.Content = Kp.ToString();
                    break;
                case 'E':
                    Kp = (int)data[1] * 256 + (int)data[2];
                    KpValue.Content = Kp.ToString();
                    break;

                case 'G':
                    Ki = (int)data[1] * 256 + (int)data[2];
                    KiValue.Content = Ki.ToString();
                    break;
                case 'H':
                    Ki = (int)data[1] * 256 + (int)data[2];
                    KiValue.Content = Ki.ToString();
                    break;

                case 'J':
                    Kd = (int)data[1] * 256 + (int)data[2];
                    KdValue.Content = Kd.ToString();
                    break;
                case 'K':
                    Kd = (int)data[1] * 256 + (int)data[2];
                    KdValue.Content = Kd.ToString();
                    break;

                case 'M':
                    Ks = (int)data[1] * 256 + (int)data[2];
                    KsValue.Content = Ks.ToString();
                    break;
                case 'N':
                    Ks = (int)data[1] * 256 + (int)data[2];
                    KsValue.Content = Ks.ToString();
                    break;

                case 'P':
                    PWMmin = (int)data[1];
                    PWMminValue.Content = PWMmin.ToString();
                    PWMmax = (int)data[2];
                    PWMmaxValue.Content = PWMmax.ToString();
                    break;
                case 'Q':
                    PWMmin = (int)data[1];
                    PWMminValue.Content = PWMmin.ToString();
                    PWMmax = (int)data[2];
                    PWMmaxValue.Content = PWMmax.ToString();
                    break;

                case 'S':
                    maxLimit = (int)data[1];
                    MaxLimitValue.Content = maxLimit.ToString();
                    clipInput = (int)data[2];
                    ClipInputValue.Content = clipInput.ToString();
                    break;
                case 'T':
                    maxLimit = (int)data[1];
                    MaxLimitValue.Content = maxLimit.ToString();
                    clipInput = (int)data[2];
                    ClipInputValue.Content = clipInput.ToString();
                    break;

                case 'V':
                    deadZone = (int)data[1];
                    DeadZoneValue.Content = deadZone.ToString();
                    PWMrev = (int)data[2];
                    PWMrevValue.Content = PWMrev.ToString();
                    break;
                case 'W':
                    deadZone = (int)data[1];
                    DeadZoneValue.Content = deadZone.ToString();
                    PWMrev = (int)data[2];
                    PWMrevValue.Content = PWMrev.ToString();
                    break;





            }


        }
        public MainWindow()
        {
            InitializeComponent();


            getAvailablePorts();
            enabledControls(false);
            comboxMotor.IsEnabled = false;
            MotorOff.IsEnabled = false;
           


        }

        private void getAvailablePorts()
        {

            ports = SerialPort.GetPortNames();
            if (ports.Length < 1)
            {
                MessageBox.Show("Please check devive USB connection!!");
                this.Close();
            }

            foreach (string port in ports)
            {
                comboxSelectPort.Items.Add(port);
            }
        }
        private void connectPortBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ConnectToArdu();
        }

        private delegate void UpdateUiTextDelegate(char[] data);

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(500);
            SerialPort sp = (SerialPort)sender;
            //string data = sp.ReadExisting();


            //Debug.Print("data:-----------");
            //Debug.Print(data);
            Debug.WriteLine("data:-----------");
            int i = sp.BytesToRead;
            Debug.WriteLine(i);
            Debug.WriteLine("---");
            int bufferEnd = -1;
            char rxByte;
            char[] rxBufDebug = new char[10];

            char[] rxBuffer = new char[4];

            //int[] data = new int[2];
            //data[0] = i;
            //data[1] = (char)sp.ReadByte();
            while (i > 0)

            {
                //byte[] data = new byte[i];
                //sp.Read(data, 0, i);

                if (port.IsOpen)
                {
                    for (int x = 0; x < i; x++)
                    {
                        //try
                        //{
                        rxByte = (char)sp.ReadByte();
                        if (rxByte.ToString() == "{" && bufferEnd == -1)
                        {
                            int y = 0;
                            while (rxByte.ToString() != "}")
                            {
                                rxByte = (char)sp.ReadByte();
                                rxBufDebug[y] = rxByte;
                                y++;
                            }
                            string data = new string(rxBufDebug);
                            Debug.Print("*****");
                            Debug.Print(data);
                            Debug.Print("");
                            Debug.Print("*****");
                        }

                        if (bufferEnd == -1)
                        {
                            if (rxByte.ToString() != "[")
                            {
                                bufferEnd = -1;
                            }
                            else
                            {
                                bufferEnd = 0;
                            }
                        }
                        else
                        {
                            rxBuffer[bufferEnd] = rxByte;
                            bufferEnd++;
                            if (bufferEnd > 3)
                            {
                                if (rxBuffer[3].ToString() == "]")
                                {

                                    Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(SerialDataProcess), rxBuffer);
                                }
                                bufferEnd = -1;
                            }
                        }

                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine("exxxxxxxxxxxxxxxxxxxxxxxxxx");
                        //    port.Dispose();
                        //}

                        //Debug.Write(rxByte);
                        //Debug out put from Arduino when find byte "{" and rxBuffer is not in processing
                        

                    }
                    
                }
            }
        }


        private void ConnectToArdu()
        {
            if (!isConnected)
            {
                isConnected = true;
                comboxMotor.IsEnabled = true;
                MotorOff.IsEnabled = true;

                //string selectedPort = comboxSelectPort.SelectedItem.ToString();

                port = new SerialPort("COM14", 19200, Parity.None, 8, StopBits.One);
                port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                port.Encoding = Encoding.GetEncoding(28591);
                
                try {
                    port.Open();
                }
                catch (Exception ex)
                {
                   
                }
                
                //port.Write("START\n");
                aTimer.Elapsed += new ElapsedEventHandler(saveParam);
                aTimer.Interval = 3000;

                rndTimer.Elapsed += new ElapsedEventHandler(generateNumber);
                rndTimer.Interval = 300;

                connectPortBtn.Content = "Disconnect";
                comboxSelectPort.IsEnabled = false;
                
            }
            else
            {
                DisconnectToArdu();
                this.Close();
            }
        }

        private void DisconnectToArdu()

        {
            aTimer.Stop();
            isConnected = false;
            enabledControls(false);
            comboxSelectPort.IsEnabled = true;
            MotorOff.IsEnabled = true;
            connectPortBtn.Content = "Connect";
            port.Write("[mo0]");
            sendOneVal("A", 350);
            sendOneVal("B", 350);
            


        }

        private void saveParam(object source, ElapsedEventArgs e)
        {
            Debug.WriteLine("SAVE!");
            port.Write("[sav]");
            aTimer.Stop();
        }

        private void activateTimer()
        {
            if (aTimer.Enabled != true)
            {

                aTimer.Enabled = true;

            }
            else if (aTimer.Enabled == true)
            {
                aTimer.Stop();
                aTimer.Start();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isConnected)
            {
                DisconnectToArdu();
            }
        }

        private void enabledControls(bool state)
        {

            DeadZoneN.IsEnabled = state;
            DeadZoneP.IsEnabled = state;
            ClipInputN.IsEnabled = state;
            ClipInputP.IsEnabled = state;
            FpwmN.IsEnabled = state;
            FpwmP.IsEnabled = state;
            FpidP.IsEnabled = state;
            FpidN.IsEnabled = state;
            KpP.IsEnabled = state;
            KpN.IsEnabled = state;
            KiP.IsEnabled = state;
            KiN.IsEnabled = state;
            KdP.IsEnabled = state;
            KdN.IsEnabled = state;
            KsP.IsEnabled = state;
            KsN.IsEnabled = state;
            StepP.IsEnabled = state;
            StepN.IsEnabled = state;
            MaxLimitP.IsEnabled = state;
            MaxLimitN.IsEnabled = state;
            PWMrevP.IsEnabled = state;
            PWMrevN.IsEnabled = state;
            PWMmaxP.IsEnabled = state;
            PWMmaxN.IsEnabled = state;
            PWMminP.IsEnabled = state;
            PWMminN.IsEnabled = state;
            TargetSlider.IsEnabled = state;
            ActivateSlider.IsEnabled = state;



        }

        private void led13CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //DeadZoneValue.Text = "2";
        }

        private void led13CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //port.Write("[V21]");
        }

        private void UpdFirstParam(int param1, int param2, string val1, string val2, string PorN)
        {
            activateTimer();
            stepValue = Int32.Parse(StepValue.Content.ToString());

            string val = comboxMotor.SelectedValue.ToString();
            int updValue = 0;

            if (PorN == "P")
            {
                updValue = param1 + stepValue;
            }
            else if (PorN == "N")
            {
                updValue = param1 - stepValue;
            }
            if (param2 == -1)
            {

                if (val == "1")
                {
                    sendOneVal(val1, updValue);
                    port.Write("[rd" + val1 + "]");
                }
                else if (val == "2")
                {
                    sendOneVal(val2, updValue);
                    port.Write("[rd" + val2 + "]");
                }
            }
            else
            {

                if (val == "1")
                {
                    sendTwoVal(val1, updValue, param2);
                    port.Write("[rd" + val1 + "]");
                }
                else if (val == "2")
                {
                    sendTwoVal(val2, updValue, param2);
                    port.Write("[rd" + val2 + "]");
                }
            }
        }

        private void UpdSecondParam(int param1, int param2, string val1, string val2, string PorN)
        {
            activateTimer();
            stepValue = Int32.Parse(StepValue.Content.ToString());
            int updValue = 0;
            if (PorN == "P")
            {
                updValue = param2 + stepValue;
            }
            else if (PorN == "N")
            {
                updValue = param2 - stepValue;
            }

            string val = comboxMotor.SelectedValue.ToString();

            if (val == "1")
            {
                sendTwoVal(val1, param1, updValue);
                port.Write("[rd" + val1 + "]");
            }
            else if (val == "2")
            {
                sendTwoVal(val2, param1, updValue);
                port.Write("[rd" + val2 + "]");
            }
        }

        private void DeadZoneP_Click(object sender, RoutedEventArgs e)
        {
            //int dz = 65;
            //char x= (char)dz;
            //DeadZoneValue.Text = DeadZoneValue.Text + 1;
            //String sendData = "[" + char.ToString(x) + char.ToString(x) + char.ToString(x) + "]";

            UpdFirstParam(deadZone, PWMrev, "V", "W", "P");
        }

        private void sendTwoVal(string header, int val1, int val2)
        {
            String sendData;
            if (val1 < 0) { val1 = 0; }
            if (val1 > 255) { val1 = 255; }
            if (val2 < 0) { val2 = 0; }
            if (val2 > 255) { val2 = 255; }
            char valOne = (char)val1;
            char valTwo = (char)val2;
            string valOneString = char.ToString(valOne);
            string valTwoString = char.ToString(valTwo);

            sendData = "[" + header + valOneString + valTwoString + "]";
            port.Write(sendData);
        }


        private void sendOneVal(string header, int val)
        {
            String sendData;
            if (val < 0) { val = 0; }
            if (val > 1024) { val = 1024; }
            int high = (int)(val / 256);
            int low = (int)val - (256 * high);

            sendData = "[" + header + char.ToString((char)high) + char.ToString((char)low) + "]";
            port.Write(sendData);
        }


        private void DeadZoneN_Click(object sender, RoutedEventArgs e)
        {

            UpdFirstParam(deadZone, PWMrev, "V", "W", "N");

        }


        

        

        private void ClipInputN_Click(object sender, RoutedEventArgs e)
        {
            UpdSecondParam(maxLimit, clipInput, "S", "T", "N");
        }

        private void ClipInputP_Click(object sender, RoutedEventArgs e)
        {
            UpdSecondParam(maxLimit, clipInput, "S", "T", "P");
        }

        private void selectOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string val = comboxMotor.SelectedValue.ToString();
            TargetSlider.Value = 350;
            if (val == "1")
            {

                port.Write("[rdA]");
                port.Write("[rdD]");
                port.Write("[rdG]");
                port.Write("[rdJ]");
                port.Write("[rdM]");
                port.Write("[rdP]");
                port.Write("[rdS]");
                port.Write("[rdV]");
                port.Write("[en1]");
                port.Write("[mo1]");
                enabledControls(true);

            }
            else if (val == "2")
            {
                port.Write("[rdB]");
                port.Write("[rdE]");
                port.Write("[rdH]");
                port.Write("[rdK]");
                port.Write("[rdN]");
                port.Write("[rdQ]");
                port.Write("[rdT]");
                port.Write("[rdW]");
                port.Write("[en2]");
                port.Write("[mo2]");
                enabledControls(true);
            }
            else
            {
                
                port.Write("[mo0]");
                enabledControls(false);
            }
        }

        private void MaxLimitP_Click(object sender, RoutedEventArgs e)
        {
            UpdFirstParam(maxLimit, clipInput, "S", "T", "P");
        }

        private void MaxLimitN_Click(object sender, RoutedEventArgs e)
        {
            UpdFirstParam(maxLimit, clipInput, "S", "T", "N");
        }

        private void PWMrevP_Click(object sender, RoutedEventArgs e)
        {
            UpdSecondParam(deadZone, PWMrev, "V", "W", "P");
        }


        private void PWMrevN_Click(object sender, RoutedEventArgs e)
        {
            UpdSecondParam(deadZone, PWMrev, "V", "W", "N");
        }

        private void StepP_Click(object sender, RoutedEventArgs e)
        {
            stepValue = Int32.Parse(StepValue.Content.ToString());
            if (stepValue == 1)
            {
                stepValue = Int32.Parse(StepValue.Content.ToString()) + 4;
            }
            else
            {
                stepValue = Int32.Parse(StepValue.Content.ToString()) + 5;
            }

            StepValue.Content = stepValue.ToString();
        }

        private void StepN_Click(object sender, RoutedEventArgs e)
        {
            stepValue = Int32.Parse(StepValue.Content.ToString()) - 5;
            if (stepValue < 0) { stepValue = 1; }
            StepValue.Content = stepValue.ToString();
        }

        private void KpP_Click(object sender, RoutedEventArgs e)
        {
            UpdFirstParam(Kp, -1, "D", "E", "P");

        }

        private void KpN_Click(object sender, RoutedEventArgs e)
        {

            UpdFirstParam(Kp, -1, "D", "E", "N");

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string val = comboxMotor.SelectedValue.ToString();
            if (val == "1")
            {
                sendOneVal("A", (int)TargetSlider.Value);
                port.Write("[rdA]");
            }
            else if (val == "2")
            {
                sendOneVal("B", (int)TargetSlider.Value);
                port.Write("[rdB]");
            }

        }

        private void KiP_Click(object sender, RoutedEventArgs e)
        {
            UpdFirstParam(Ki, -1, "G", "H", "P");
        }

        private void KiN_Click(object sender, RoutedEventArgs e)
        {
            UpdFirstParam(Ki, -1, "G", "H", "N");
        }

        private void KdP_Click(object sender, RoutedEventArgs e)
        {
            UpdFirstParam(Kd, -1, "J", "K", "P");
        }

        private void KdN_Click(object sender, RoutedEventArgs e)
        {
            UpdFirstParam(Kd, -1, "J", "K", "N");
        }

        private void KsN_Click(object sender, RoutedEventArgs e)
        {
            UpdFirstParam(Ks, -1, "M", "N", "N");
        }

        private void KsP_Click(object sender, RoutedEventArgs e)
        {
            UpdFirstParam(Ks, -1, "M", "N", "P");
        }

        private void PWMminP_Click(object sender, RoutedEventArgs e)
        {
            UpdFirstParam(PWMmin, PWMmax, "P", "Q", "P");
        }

        private void PWMminN_Click(object sender, RoutedEventArgs e)
        {
            UpdFirstParam(PWMmin, PWMmax, "P", "Q", "N");
        }

        private void PWMmaxP_Click(object sender, RoutedEventArgs e)
        {
            UpdSecondParam(PWMmin, PWMmax, "P", "Q", "P");
        }

        private void PWMmaxN_Click(object sender, RoutedEventArgs e)
        {
            UpdSecondParam(PWMmin, PWMmax, "P", "Q", "N");
        }


        private void MotorOff_Click(object sender, RoutedEventArgs e)
        {
            port.Write("[mo0]");
            enabledControls(false);
            comboxMotor.Text = "OFF";
            
            rndTimer.Stop();
            ActivateSlider.Content = "Move";
        }

        private void ActivateSlider_Click(object sender, RoutedEventArgs e)
        {

            
            if (rndTimer.Enabled == true)
            {
                rndTimer.Stop();
                ActivateSlider.Content = "Move";

            }
            else
            {
                rndTimer.Start();
                ActivateSlider.Content = "Stop";
            }
            

        }
        private delegate void UpdateTextHandler(int updatedNumber);
        private void generateNumber(object source, ElapsedEventArgs e)
        {
            Random rnd = new Random();
            int number = rnd.Next(200, 500);
            Debug.WriteLine(number);

            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateTextHandler(update_textBox), number);


            

        }

        void update_textBox(int updatedNumber)

        {

            TargetSlider.Value = updatedNumber;
            string val = comboxMotor.SelectedValue.ToString();
            if (val == "1")
            {
                sendOneVal("A", updatedNumber);
                port.Write("[rdA]");
            }
            else if (val == "2")
            {
                sendOneVal("B", updatedNumber);
                port.Write("[rdB]");
            }

        }
    }
}
