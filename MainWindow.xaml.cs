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
        public MainWindow()
        {
            InitializeComponent();


            getAvailablePorts();
            enabledControls(false);
            foreach (string port in ports)
            {
                comboxSelectPort.Items.Add(port);
            }


        }

        private void getAvailablePorts()
        {
            
            ports = SerialPort.GetPortNames();
            if (ports.Length<1)
            {
                MessageBox.Show("Please check devive USB connection!!");
                this.Close();
            }
        }

        private void connectPort_Click(object sender, RoutedEventArgs e)
        {
        }
        private delegate void UpdateUiTextDelegate(string text);

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(500);
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadTo("]");
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(SerialDataProcess), data);
        }

        int deadZone = 0;
        int PWMrev = 0;
        
        private void SerialDataProcess(String data)
        {
            Debug.WriteLine(data);

            byte[] bytes = Encoding.ASCII.GetBytes(data);
            //Debug.WriteLine(char.ToString((char)bytes[0]));
            if (char.ToString((char)bytes[0]) == "[")
            {
                switch (char.ToString((char)bytes[1]))
                {
                    case "V":
                        deadZone = (int)bytes[2];
                        Debug.Write(deadZone);

                        DeadZoneValue.Content = deadZone.ToString();
                        PWMrev = (int)bytes[3];
                        Debug.Write(PWMrev);
                        PWMrevValue.Content = PWMrev.ToString();
                        break;
                }

            }

        }


        private void ConnectToArdu()
        {
            if (!isConnected)
            {
                isConnected = true;

                enabledControls(true);
                //string selectedPort = comboxSelectPort.SelectedItem.ToString();
                
                port = new SerialPort("COM7", 19200, Parity.None, 8, StopBits.One);
                port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                port.Open();
                //port.Write("START\n");
                connectPortBtn.Content = "Disconnect";
                comboxSelectPort.IsEnabled = false;
            }
            else
            {
                DisconnectToArdu();
            }
        }

        private void DisconnectToArdu()
        {
            isConnected = false;
            enabledControls(false);
            comboxSelectPort.IsEnabled = true;
            connectPortBtn.Content = "Connect";
            port.Close();
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
            led13CheckBox.IsEnabled = state;
            DeadZoneN.IsEnabled = state;
            DeadZoneP.IsEnabled = state;
            ClipInputN.IsEnabled = state;
            ClipInputP.IsEnabled = state;
        }
        
        private void led13CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //DeadZoneValue.Text = "2";
        }

        private void led13CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //port.Write("[V21]");
        }

        

        private void DeadZoneP_Click(object sender, RoutedEventArgs e)
        {
            //int dz = 65;
            //char x= (char)dz;
            //DeadZoneValue.Text = DeadZoneValue.Text + 1;
            //String sendData = "[" + char.ToString(x) + char.ToString(x) + char.ToString(x) + "]";


            stepValue = Int32.Parse(StepValue.Content.ToString());
            int updDeadZone = deadZone + stepValue;
            sendDataNum("V", updDeadZone, PWMrev);
            port.Write("[sav]");
            port.Write("[rdV]");
        }

        private void sendDataNum(string header, int val1, int val2)
        {
            String sendData;
            char valOne = (char)val1;
            char valTwo = (char)val2;

            sendData = "["+header + char.ToString(valOne) + char.ToString(valTwo) + "]";
            port.Write(sendData);
        }

        int stepValue;
        private void DeadZoneN_Click(object sender, RoutedEventArgs e)
        {

            
            
            
            stepValue = Int32.Parse(StepValue.Content.ToString());
            int updDeadZone = deadZone - stepValue;
            if (updDeadZone < 0) { updDeadZone = 0; }
            sendDataNum("V", updDeadZone, PWMrev);
            port.Write("[sav]");
            port.Write("[rdV]");

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void connectPortBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ConnectToArdu();
        }

        private void connectPortBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (connectPortBtn.Content == "Disconnect")
            {
                //port.Write("[rdD]");
                //port.Write("[rdb]");
                //port.Write("[rdc]");
                
                //for (int txVal = 65; txVal < 91; txVal ++)
                //{
                //    char txValChar = (char)txVal;
                //    string txValString = char.ToString(txValChar);
                //    port.Write("[rd" + txValString + "]");
                //    Debug.Write("[rd" + txValString + "]");
                //}
            }
        }

        private void ClipInputN_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClipInputP_Click(object sender, RoutedEventArgs e)
        {

        }

        private void selectOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string val = comboxMotor.SelectedValue.ToString();
            
            if(val == "1")
            {
                //port.Write("[rdA]");
                //port.Write("[rdD]");
                //port.Write("[rdJ]");
                //port.Write("[rdM]");
                //port.Write("[rdP]");
                //port.Write("[rdS]");
                port.Write("[rdV]");

            }
        }

        private void MaxLimitP_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MaxLimitN_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PWMrevP_Click(object sender, RoutedEventArgs e)
        {
            stepValue = Int32.Parse(StepValue.Content.ToString());
            int updDeadZone = deadZone + stepValue;
            sendDataNum("V", updDeadZone, PWMrev);
            port.Write("[sav]");
            port.Write("[rdV]");
        }

        private void PWMrevN_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
