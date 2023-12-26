using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using ExtendedSerialPort;
using System.IO.Ports;
using System.Windows.Threading;
using System.Diagnostics;

namespace robotInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool btnClickFlag = false;
        private bool btnClickFlagClear = false;

        private ReliableSerialPort serialPort1;
        private bool isSerialPortAvailable = false;
        private DispatcherTimer timerDisplay;
        private Robot robot = new Robot();
        private SerialProtocolManager UARTProtocol = new SerialProtocolManager();


        public MainWindow()
        {
            InitializeComponent();
            InitializeSerialPort();

            btnEnvoyer.Background = Brushes.Beige;
            btnClear.Background = Brushes.Beige;



            timerDisplay = new DispatcherTimer();
            timerDisplay.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerDisplay.Tick += TimerDisplay_Tick;
            timerDisplay.Start();

            UARTProtocol.setRobot(robot);
        }

        private void TimerDisplay_Tick(object? sender, EventArgs e)
        {
            /*if (robot.receivedText != "")
            {
                textBoxReception.Text += robot.receivedText;
                robot.receivedText = "";
            }*/
            label_IRExtremeGauche.Content = "IR Extreme Gauche: {value} cm".Replace("{value}", robot.distanceTelemetreLePen.ToString());
            label_IRGauche.Content = "IR Gauche: {value} cm".Replace("{value}", robot.distanceTelemetreGauche.ToString());
            label_IRCentre.Content = "IR Centre: {value} cm".Replace("{value}", robot.distanceTelemetreCentre.ToString());
            label_IRDroit.Content = "IR Droit: {value} cm".Replace("{value}", robot.distanceTelemetreDroit.ToString());
            label_IRExtremeDroit.Content = "IR Extreme Droit: {value} cm".Replace("{value}", robot.distanceTelemetreMelenchon.ToString());

            label_CONDroit.Content = "Vitesse Droite: {value}%".Replace("{value}", robot.consigneDroite.ToString());
            label_CONGauche.Content = "Vitesse Gauche: {value}%".Replace("{value}", robot.consigneGauche.ToString());

            while (robot.stringListReceived.Count != 0)
            {
                // byte current = robot.byteListReceived.Dequeue();
                textBoxReception.Text += robot.stringListReceived.Dequeue();
                /* textBoxReception.Text += Convert.ToChar(current).ToString();*/
            }
        }

        private void InitializeSerialPort()
        {
            string comPort = "COM4";
            if(SerialPort.GetPortNames().Contains(comPort)){
                serialPort1 = new ReliableSerialPort(comPort, 115200, Parity.None, 8, StopBits.One);
                serialPort1.OnDataReceivedEvent += SerialPort1_DataReceived;
                try
                {
                    serialPort1.Open();
                    isSerialPortAvailable = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error opening serial port: " + ex.Message);
                }
            }
        }

        public void SerialPort1_DataReceived(object? sender, DataReceivedArgs e) {
            // robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            for (int i = 0; i < e.Data.Length; i++)
            {
                UARTProtocol.DecodeMessage(e.Data[i]);
                // robot.byteListReceived.Enqueue(e.Data[i]);
            }
        }

        private bool sendMessage(bool key) // key = true si appuie sur Enter et false autrement
        {
            if (textBoxEmission.Text == "\r\n" || textBoxEmission.Text == "") return false;

            byte[] payload=new byte[textBoxEmission.Text.Length];

            for (int i = 0; i < textBoxEmission.Text.Length; i++)
                payload[i] = (byte)textBoxEmission.Text[i];


            // a décommmenterrrr
          //  serialPort1.SendMessage(this,payload);
            textBoxEmission.Text = "";
            return true;
        }

        private void btnEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            if (btnClickFlag)
            {
                btnEnvoyer.Background = Brushes.RoyalBlue;
            }
            else
            {
                btnEnvoyer.Background = Brushes.Beige;
            }
            btnClickFlag = !btnClickFlag;

            sendMessage(false);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (btnClickFlagClear)
            {
                btnClear.Background = Brushes.RoyalBlue;
            }
            else
            {
                btnClear.Background = Brushes.Beige;
            }
            btnClickFlagClear = !btnClickFlagClear;

            textBoxReception.Text = "";
        }

        private void textBoxEmission_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!sendMessage(true)) {
                    textBoxEmission.Text = "";
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {/*
            byte[] byteList = new byte[20];
            for (int i = 19; i >= 0; i--)
                byteList[i]= (byte) (2*i);
            
            serialPort1.Write(byteList, 0, 20);*/
         ////   serialPort1.Write(UARTProtocol.UartEncode((int)SerialProtocolManager.CommandID.TEXT, 7, Encoding.ASCII.GetBytes("Bonjour")), 0, 13);
        }

        private void led_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            var ckb = (CheckBox)sender;
            byte numeroLed = 0;
            var etat = Convert.ToByte(ckb.IsChecked ?? false);

            switch (ckb.Name)
            {
                case "checkBoxLed1":
                    numeroLed = 0;
                    ckb.Foreground = etat == 1 ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.Black);
                    break;

                case "checkBoxLed2":
                    numeroLed = 1;
                    ckb.Foreground = etat == 1 ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
                    break;

                case "checkBoxLed3":
                    numeroLed = 2;
                    ckb.Foreground = etat == 1 ? new SolidColorBrush(Colors.Orange) : new SolidColorBrush(Colors.Black);
                    break;
            }
            byte[] payload = { numeroLed, etat };
            Debug.WriteLine(payload[0].ToString());
            Debug.WriteLine(payload[1].ToString());

            // a decommentater
            // serialPort1.Write(UARTProtocol.UartEncode((int)SerialProtocolManager.CommandID.LED, 2, payload), 0, 8);
        }

    }
}
