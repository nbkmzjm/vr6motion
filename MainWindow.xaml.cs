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
        
        private void SerialDataProcess(String data)
        {
            Debug.WriteLine(data);

            byte[] bytes = Encoding.ASCII.GetBytes(data);
            Debug.WriteLine(char.ToString((char)bytes[0]));
            if (char.ToString((char)bytes[0]) == "[")
            {
                switch (char.ToString((char)bytes[1]))
                {
                    case "D":
                        DeadZoneValue1.Text = bytes[2].ToString();
                        Debug.WriteLine(bytes[2].ToString());
                        DeadZoneValue2.Text = bytes[3].ToString();
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
                
                port = new SerialPort("COM14", 9600, Parity.None, 8, StopBits.One);
                port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                port.Open();
                port.Write("START\n");
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
            DeadZone1N.IsEnabled = state;
            DeadZone1P.IsEnabled = state;
            DeadZone2N.IsEnabled = state;
            DeadZone2P.IsEnabled = state;
        }
        
        private void led13CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //DeadZoneValue.Text = "2";
        }

        private void led13CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //port.Write("[V21]");
        }

        

        private void DeadZone1P_Click(object sender, RoutedEventArgs e)
        {
            //int dz = 65;
            //char x= (char)dz;
            //DeadZoneValue.Text = DeadZoneValue.Text + 1;
            //String sendData = "[" + char.ToString(x) + char.ToString(x) + char.ToString(x) + "]";
            sendDataNum("A", 78, 79);
            port.Write("[sav]");
            port.Write("[rdD]");
        }

        private void sendDataNum(string header, int val1, int val2)
        {
            String sendData;
            char valOne = (char)val1;
            char valTwo = (char)val2;

            sendData = "["+header + char.ToString(valOne) + char.ToString(valTwo) + "]";
            port.Write(sendData);
        }


        private void DeadZone1N_Click(object sender, RoutedEventArgs e)
        {
            port.Write("led13of\n");
            
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
                port.Write("[rdD]");
            }
        }

        private void DeadZone2N_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeadZone2P_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
