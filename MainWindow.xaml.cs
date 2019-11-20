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

        int deadZone = 0;
        int PWMrev = 0;
        int stepValue;
        int Kp = 0;
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
        private delegate void UpdateUiTextDelegate(int[] text);

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            //Thread.Sleep(500);
            SerialPort sp = (SerialPort)sender;
            //string data = sp.ReadExisting();
            

            //Debug.Print("data:-----------");
            //Debug.Print(data);
            //Debug.WriteLine("data:-----------");

            int i = sp.BytesToRead;
            Debug.WriteLine(i);
            Debug.WriteLine("---");
            int bufferEnd = -1;
            char rxByte;
            char[] rxBufDebug = new char[10];

            char [] rxBuffer = new char[4];

            int[] data = new int[2];
            data[0] = i;
            data[1] = (char)sp.ReadByte();
            //while (i > 0)

            //{
            //    //byte[] data = new byte[i];
            //    //sp.Read(data, 0, i);
            //    for (int x = 0; x<i ; x++)
            //    {

            rxByte = (char)sp.ReadByte();
            //        //Debug out put from Arduino when find byte "{" and rxBuffer is not in processing
            //        if (rxByte.ToString() == "{" && bufferEnd == -1)
            //        {
            //            int y = 0;
            //            while (rxByte.ToString() != "}")
            //            {
            //                rxByte = (char)sp.ReadByte();
            //                rxBufDebug[y] = rxByte;
            //                y++;
            //            }
            //            string data = new string(rxBufDebug);
            //            Debug.WriteLine(data);
            //            Debug.WriteLine("*********");
            //        }

            //        if (bufferEnd == -1)
            //        {
            //            if (rxByte.ToString() != "[")
            //            {
            //                bufferEnd = -1;
            //            }
            //            else
            //            {
            //                bufferEnd = 0;
            //            }
            //        }
            //        else
            //        {
            //            rxBuffer[bufferEnd] = rxByte;
            //            bufferEnd++;
            //            if (bufferEnd > 3)
            //            {
            //                if (rxBuffer[3].ToString() == "]")
            //                {
            //                    Debug.Write("0: ");
            //                    Debug.WriteLine(rxBuffer[0]);
            //                    deadZone = (int)rxBuffer[0];
            //                    TextBoxX. .Text = deadZone.ToString();
            //                    Debug.Write("1: ");
            //                    Debug.WriteLine(rxBuffer[1]);
            //                    Debug.Write("2: ");
            //                    Debug.WriteLine(rxBuffer[2]);
            //                    Debug.Write("3: ");
            //                    Debug.WriteLine(rxBuffer[3]);

            //                    Debug.WriteLine("==============");
            //                }
            //                bufferEnd = -1;
            //            }
            //        }

            //    }


            //    //Debug.WriteLine(data[1]);
            //    //Debug.WriteLine(data[2]);
            //    //Debug.WriteLine(data[3]);
            //}

            //string data = sp.ReadTo("]");
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(SerialDataProcess), data);
        }

        

        private void SerialDataProcess(int[] data)
        {
            Debug.WriteLine("data:");
            Debug.WriteLine(data[0]);
            Debug.WriteLine(data[1]);
           

            //    byte[] bytes = Encoding.ASCII.GetBytes(data);
            //    //Debug.WriteLine(char.ToString((char)bytes[0]));
            //    if (char.ToString((char)bytes[0]) == "[")
            //    {
            //        switch (char.ToString((char)bytes[1]))
            //        {
            //            case "V":
            //                deadZone = (int)bytes[2];
            //                DeadZoneValue.Content = deadZone.ToString();
            //                Debug.Write((int)bytes[2]);
            //                Debug.Write((int)bytes[3]);
            //                PWMrev = (int)bytes[3];

            //                PWMrevValue.Content = PWMrev.ToString();
            //                break;
            //            case "D":
            //                Kp = (int)bytes[2] * 256 + (int)bytes[3];
            //                KpValue.Content = Kp.ToString();
            //                break;

            //        }

            //    }

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
            int updValue = deadZone + stepValue;
            sendTwoVal("V", updValue, PWMrev);
            port.Write("[sav]");
            port.Write("[rdV]");
        }

        private void sendTwoVal(string header, int val1, int val2)
        {
            String sendData;
            char valOne = (char)val1;
            char valTwo = (char)val2;

            sendData = "["+header + char.ToString(valOne) + char.ToString(valTwo) + "]";
            port.Write(sendData);
        }


        private void sendOneVal(string header, decimal val)
        {
            String sendData;
            int high= (int)Math.Round(val/256, 0);
            int low = (int)val - (256 * high);
            

            
            sendData = "[" + header + char.ToString((char)high) + char.ToString((char)low) + "]";
            port.Write(sendData);
        }

        
        private void DeadZoneN_Click(object sender, RoutedEventArgs e)
        {




            stepValue = Int32.Parse(StepValue.Content.ToString());
            int updValue = deadZone - stepValue;
            if (updValue < 0) { updValue = 0; }
            sendTwoVal("V", updValue, PWMrev);
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
                port.Write("[rdV]");
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
            int updValue = PWMrev + stepValue;
            sendTwoVal("V", deadZone, updValue);
            port.Write("[sav]");
            port.Write("[rdV]");
        }


     

        private void PWMrevN_Click(object sender, RoutedEventArgs e)
        {
            stepValue = Int32.Parse(StepValue.Content.ToString());
            int updValue = PWMrev - stepValue;
            if (updValue < 0) { updValue = 0; }
            sendTwoVal("V", deadZone, updValue);
            port.Write("[sav]");
            port.Write("[rdV]");
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
            stepValue = Int32.Parse(StepValue.Content.ToString());
            int deltaVal = Kp + stepValue;
            sendOneVal("D", deltaVal);
            port.Write("[sav]");
            port.Write("[rdD]");

        }

        private void KpN_Click(object sender, RoutedEventArgs e)
        {
            stepValue = Int32.Parse(StepValue.Content.ToString());
            int deltaVal = Kp - stepValue;
            sendOneVal("D", deltaVal);
            port.Write("[sav]");
            port.Write("[rdD]");
        }
    }
}
