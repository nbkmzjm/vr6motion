﻿using System;
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
            String serialData = sp.ReadExisting();
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(SerialDataProcess), serialData);
            //SerialDataProcess(serialData);
            Debug.Write(serialData);

        }



        private void SerialDataProcess(String serialdata)
        {
           
            DeadZoneValue.Text = serialdata;
            

        }


        private void ConnectToArdu()
        {
            if (!isConnected)
            {
                isConnected = true;

                enabledControls(true);
                //string selectedPort = comboxSelectPort.SelectedItem.ToString();
                
                port = new SerialPort("COM4", 500000, Parity.None, 8, StopBits.One);
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
            port.Write("#STOP\n");
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
            if (port.IsOpen)
            {
                port.Write("led13on\n");
                //Debug.Write("checked");
                //port.Write("[V45]");
            }
        }

        private void DeadZoneN_Click(object sender, RoutedEventArgs e)
        {
            if (port.IsOpen)
            {
                port.Write("led13of\n");
                //Debug.Write("unchecked");
                //port.Write("[V45]");
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }



        private void connectPortBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.Write("u");
           
        }

        private void connectPortBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Debug.Write("d");
        }

        private void connectPortBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            Debug.Write("d");
        }

        private void connectPortBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ConnectToArdu();
        }

        private void connectPortBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (connectPortBtn.Content == "Disconnect")
            {
                port.Write("START\n");
            }
        }
    }
}