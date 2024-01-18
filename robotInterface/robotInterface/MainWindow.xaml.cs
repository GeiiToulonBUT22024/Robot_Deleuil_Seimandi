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
            InitializeLedStates();

            timerDisplay = new DispatcherTimer();
            timerDisplay.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerDisplay.Tick += TimerDisplay_Tick;
            timerDisplay.Start();

            UARTProtocol.setRobot(robot);
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowState = WindowState.Maximized;

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


                                                                                         // A décommmenter en vrai
          //  serialPort1.SendMessage(this,payload);
            textBoxEmission.Text = "";
            return true;
        }

        private void btnEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            if (btnClickFlag)
            {
              //  btnEnvoyer.Background = Brushes.RoyalBlue;
            }
            else
            {
             //   btnEnvoyer.Background = Brushes.Beige;
            }
            btnClickFlag = !btnClickFlag;

            sendMessage(false);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            
            if (btnClickFlagClear)
            {
               // btnClear.Background = Brushes.RoyalBlue;
            }
            else
            {
               // btnClear.Background = Brushes.Beige;
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
        private void EllipseLed_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var ellipse = sender as Ellipse;
            if (ellipse != null)
            {
                byte numeroLed = Convert.ToByte(ellipse.Tag);
                bool isLedOn = ellipse.Fill.ToString() == Brushes.Black.ToString(); 

                ToggleLed(ellipse, numeroLed, !isLedOn);
            }
        }

        private void ToggleLed(Ellipse ellipse, byte numeroLed, bool isLedOn)
        {
            var etat = Convert.ToByte(isLedOn);
            SolidColorBrush newColor = Brushes.Black;
            SolidColorBrush textColor = Brushes.White; 

            switch (numeroLed)
            {
                case 0:
                    newColor = isLedOn ? Brushes.Black : Brushes.White;
                    textColor = isLedOn ? Brushes.White : Brushes.Black;
                    break;
                case 1:
                    newColor = isLedOn ? Brushes.Black : Brushes.Blue;
                    textColor = isLedOn ? Brushes.Blue : Brushes.White;
                    break;
                case 2:
                    newColor = isLedOn ? Brushes.Black : Brushes.Orange;
                    textColor = isLedOn ? Brushes.Orange : Brushes.White;
                    break;
            }

            ellipse.Fill = newColor;

            // Mise à jour de la couleur du texte du TextBlock associé
            if (ellipse.Parent is Grid grid)
            {
                var textBlock = grid.Children[1] as TextBlock;
                if (textBlock != null)
                {
                    textBlock.Foreground = textColor;
                }
            }

            byte[] payload = { numeroLed, etat };
            Debug.WriteLine(payload[0].ToString());
            Debug.WriteLine(payload[1].ToString());

            // Mise à jour des voyants
            UpdateVoyants();

            // Envoyer la commande au port série
            // serialPort1.Write(UARTProtocol.UartEncode((int)SerialProtocolManager.CommandID.LED, 2, payload), 0, 8);
        }




        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove(); // Permet de déplacer la fenêtre
        }


        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Minimise la fenêtre
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal; // Restaure la fenêtre si elle est maximisée
            }
            else
            {

                this.WindowState = WindowState.Maximized; // Maximise la fenêtre si elle n'est pas maximisée
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Ferme la fenêtre
        }

        private void InitializeLedStates()
        {
            // Définir l'état initial de chaque LED --> en vrai il faudrait récuperer les infos des leds depuis le robot au chargement
            ellipseLed1.Fill = Brushes.White;
            ellipseLed2.Fill = Brushes.Blue;
            ellipseLed3.Fill = Brushes.Orange;

            UpdateVoyants();
        }

        // Gestion des couleurs des leds
                private void UpdateVoyants()
        {
            voyantLed1.Fill = ellipseLed1.Fill == Brushes.Black ? Brushes.Black : Brushes.White;
            voyantLed2.Fill = ellipseLed2.Fill == Brushes.Black ? Brushes.Black : Brushes.Blue;
            voyantLed3.Fill = ellipseLed3.Fill == Brushes.Black ? Brushes.Black : Brushes.Orange;
        }


    }
}
