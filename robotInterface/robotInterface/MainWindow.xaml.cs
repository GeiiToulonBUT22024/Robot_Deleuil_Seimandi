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
        }

        private void TimerDisplay_Tick(object? sender, EventArgs e)
        {
            if (robot.receivedText != "")
            {
                textBoxReception.Text += robot.receivedText;
                robot.receivedText = "";
            }
        }

        private void InitializeSerialPort()
        {
            string comPort = "COM6";
            if(SerialPort.GetPortNames().Contains(comPort)){
                serialPort1 = new ReliableSerialPort("COM6", 115200, Parity.None, 8, StopBits.One);
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
            robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
        }

        private bool sendMessage(bool key) // key = true si appuie sur Enter et false autrement
        {
            if (textBoxEmission.Text == "\r\n" || textBoxEmission.Text == "") return false;

            byte[] payload=new byte[textBoxEmission.Text.Length];

            for (int i = 0; i < textBoxEmission.Text.Length; i++)
                payload[i] = (byte)textBoxEmission.Text[i];
            serialPort1.SendMessage(this,payload);
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
    }
}
