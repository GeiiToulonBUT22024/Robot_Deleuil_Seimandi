﻿using System;
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
using Syncfusion.UI.Xaml.Gauges;

using System.Numerics;

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



#pragma warning disable CS8618 
        public MainWindow()
#pragma warning restore CS8618
        {
            InitializeComponent();
            InitializeSerialPort();
            InitializeLedStates();

            timerDisplay = new DispatcherTimer();
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // ATTENTION TIMER MODIFIER POUR TESTTT
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            timerDisplay.Interval = new TimeSpan(0, 0, 0, 0, 500); // 100 -> 500
            timerDisplay.Tick += TimerDisplay_Tick;
            timerDisplay.Start();

            UARTProtocol.setRobot(robot);
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowState = WindowState.Maximized;

        }

        private void TimerDisplay_Tick(object? sender, EventArgs e)
        {
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Random rnd = new Random();
            robot.distanceTelemetreMelenchon = (float)(rnd.NextDouble() * 100 + 10);
            robot.distanceTelemetreGauche = (float)(rnd.NextDouble() * 100 + 10);
            robot.distanceTelemetreCentre = (float)(rnd.NextDouble() * 100 + 10);
            robot.distanceTelemetreDroit = (float)(rnd.NextDouble() * 100 + 10);
            robot.distanceTelemetreLePen = (float)(rnd.NextDouble() * 100 + 10);

            robot.consigneGauche = (float)(rnd.NextDouble() * 200 - 100);
            robot.consigneDroite = (float)(rnd.NextDouble() * 200 - 100);


            updateSpeedGauges();
            updateTelemetreGauges();
            updateTelemetreBoxes();

            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            /*if (robot.receivedText != "")
            {
                textBoxReception.Text += robot.receivedText;
                robot.receivedText = "";
            }*/

            /*
            label_IRExtremeGauche.Content = "IR Extreme Gauche: {value} cm".Replace("{value}", robot.distanceTelemetreLePen.ToString());
            label_IRGauche.Content = "IR Gauche: {value} cm".Replace("{value}", robot.distanceTelemetreGauche.ToString());
            label_IRCentre.Content = "IR Centre: {value} cm".Replace("{value}", robot.distanceTelemetreCentre.ToString());
            label_IRDroit.Content = "IR Droit: {value} cm".Replace("{value}", robot.distanceTelemetreDroit.ToString());
            label_IRExtremeDroit.Content = "IR Extreme Droit: {value} cm".Replace("{value}", robot.distanceTelemetreMelenchon.ToString());

           // label_CONDroit.Content = "Vitesse Droite: {value}%".Replace("{value}", robot.consigneDroite.ToString());
            //label_CONGauche.Content = "Vitesse Gauche: {value}%".Replace("{value}", robot.consigneGauche.ToString());
            */

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
            else MessageBox.Show("Port doesn't exist");
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
        {
            /*
            byte[] byteList = new byte[20];
            for (int i = 19; i >= 0; i--)
                byteList[i]= (byte) (2*i);
            
            serialPort1.Write(byteList, 0, 20);*/
            ////   serialPort1.Write(UARTProtocol.UartEncode((int)SerialProtocolManager.CommandID.TEXT, 7, Encoding.ASCII.GetBytes("Bonjour")), 0, 13);
        }

        private Vector2 getBTTranslationVector(double angle, double scaleCoef, double distance) {
            Vector2 translateVector = new Vector2();
            translateVector.X = (float)(Math.Cos((Math.PI / 180) * (270 + angle)) * distance * scaleCoef);
            translateVector.Y = (float)(Math.Sin((Math.PI / 180) * (270 + angle)) * distance * scaleCoef);
            return translateVector;
        }
        private void updateTelemetreBoxes() {
            var scaleCoef = 1.5;

            var angle = -47.703;
            TransformGroup customTGELeft = new TransformGroup();
            customTGELeft.Children.Add(new RotateTransform(angle));
            Vector2 translationVector = getBTTranslationVector(angle, scaleCoef, robot.distanceTelemetreMelenchon);
            customTGELeft.Children.Add(new TranslateTransform(translationVector.X, translationVector.Y));

            boxTeleELeft.RenderTransform = customTGELeft;

            // -------------------------------------------------
            angle = -24.786;
            TransformGroup customTGLeft = new TransformGroup();
            customTGLeft.Children.Add(new RotateTransform(angle));
            translationVector = getBTTranslationVector(angle, scaleCoef, robot.distanceTelemetreGauche);
            customTGLeft.Children.Add(new TranslateTransform(translationVector.X, translationVector.Y));

            boxTeleLeft.RenderTransform = customTGLeft;

            // -------------------------------------------------

            angle = 0;
            TransformGroup customTGCenter = new TransformGroup();
            translationVector = getBTTranslationVector(angle, scaleCoef, robot.distanceTelemetreCentre);
            customTGCenter.Children.Add(new TranslateTransform(translationVector.X, translationVector.Y));

            boxTeleCenter.RenderTransform = customTGCenter;

            // -------------------------------------------------
            angle = 24.786;

            TransformGroup customTGRight = new TransformGroup();
            customTGRight.Children.Add(new RotateTransform(angle));
            translationVector = getBTTranslationVector(angle, scaleCoef, robot.distanceTelemetreDroit);
            customTGRight.Children.Add(new TranslateTransform(translationVector.X, translationVector.Y));

            boxTeleRight.RenderTransform = customTGRight;

            // -------------------------------------------------
            angle = 47.703;

            TransformGroup customTGERight = new TransformGroup();
            customTGERight.Children.Add(new RotateTransform(angle));
            translationVector = getBTTranslationVector(angle, scaleCoef, robot.distanceTelemetreLePen);
            customTGERight.Children.Add(new TranslateTransform(translationVector.X, translationVector.Y));

            boxTeleERight.RenderTransform = customTGERight;
        }


        private void updateSpeedGauges()
        {
            updateGaugePointer(LeftGauge, robot.consigneGauche);
            updateGaugePointer(RightGauge, robot.consigneDroite);
        }

        private void updateGaugePointer(SfCircularGauge gauge, double value)
        {
            if (gauge.Scales[0].Pointers.Count == 0)
            {
                CircularPointer circularPointer = new CircularPointer
                {
                    PointerType = PointerType.NeedlePointer,
                    Value = value,
                    NeedleLengthFactor = 0.5,
                    NeedlePointerType = NeedlePointerType.Triangle,
                    PointerCapDiameter = 12,
                    NeedlePointerStroke = new BrushConverter().ConvertFrom("#757575") as SolidColorBrush,
                    KnobFill = new BrushConverter().ConvertFrom("#757575") as SolidColorBrush,
                    KnobStroke = new BrushConverter().ConvertFrom("#757575") as SolidColorBrush,
                    NeedlePointerStrokeThickness = 7
                };

                gauge.Scales[0].Pointers.Add(circularPointer);
            }
            else
            {
                (gauge.Scales[0].Pointers[0] as CircularPointer).Value = value;
            }
        }

        private void updateTelemetreGauges()
        {
            telemetreMelenchonRange.EndValue = robot.distanceTelemetreMelenchon;
            telemetreMelenchonRange.RangeStroke = ChooseColor(robot.distanceTelemetreMelenchon);
            textTelemetreMelenchon.Text = robot.distanceTelemetreMelenchon.ToString("F0") + " cm";

            telemetreGaucheRange.EndValue = robot.distanceTelemetreGauche;
            telemetreGaucheRange.RangeStroke = ChooseColor(robot.distanceTelemetreGauche);
            textTelemetreGauche.Text = robot.distanceTelemetreGauche.ToString("F0") + " cm";

            telemetreCentreRange.EndValue = robot.distanceTelemetreCentre;
            telemetreCentreRange.RangeStroke = ChooseColor(robot.distanceTelemetreCentre);
            textTelemetreCentre.Text = robot.distanceTelemetreCentre.ToString("F0") + " cm";

            telemetreDroitRange.EndValue = robot.distanceTelemetreDroit;
            telemetreDroitRange.RangeStroke = ChooseColor(robot.distanceTelemetreDroit);
            textTelemetreDroit.Text = robot.distanceTelemetreDroit.ToString("F0") + " cm";

            telemetreLePenRange.EndValue = robot.distanceTelemetreLePen;
            telemetreLePenRange.RangeStroke = ChooseColor(robot.distanceTelemetreLePen);
            textTelemetreLePen.Text = robot.distanceTelemetreLePen.ToString("F0") + " cm";
        }

        private Brush ChooseColor(float distance)
        {
            if (distance < 20)
            {
                return Brushes.Red; // Rouge pour les valeurs inférieures à 20
            }
            else if (distance < 40)
            {
                return Brushes.Orange; // Orange pour les valeurs entre 20 et 40
            }
            else
            {
                return Brushes.Green; // Vert pour les valeurs supérieures à 40
            }
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

            TextBlock? textBlockAssociated = null;

            switch (numeroLed)
            {
                case 0:
                    newColor = isLedOn ? Brushes.Black : Brushes.White;
                    textColor = isLedOn ? Brushes.White : Brushes.Black;
                    textBlockAssociated = textBlockLed1; // Supposons que textBlockLed1 est le TextBlock associé à ellipseLed1
                    break;
                case 1:
                    newColor = isLedOn ? Brushes.Black : Brushes.Blue;
                    textColor = isLedOn ? Brushes.Blue : Brushes.White;
                    textBlockAssociated = textBlockLed2; // De même pour textBlockLed2 et ellipseLed2
                    break;
                case 2:
                    newColor = isLedOn ? Brushes.Black : Brushes.Orange;
                    textColor = isLedOn ? Brushes.Orange : Brushes.White;
                    textBlockAssociated = textBlockLed3; // De même pour textBlockLed3 et ellipseLed3
                    break;
            }

            ellipse.Fill = newColor;
            if (textBlockAssociated != null)
            {
                textBlockAssociated.Foreground = textColor;
            }

            // Obtenez la référence au TextBlock correspondant
            var textBlock = FindTextBlockForLed(ellipse);

            if (textBlock != null)
            {
                textBlock.Foreground = textColor;
            }


            Debug.WriteLine(numeroLed.ToString());
            Debug.WriteLine(etat.ToString());

            // Mise à jour des voyants
            UpdateVoyants();

            // Envoyer la commande au port série
            if(isSerialPortAvailable)
            {
                byte[] rawData = UARTProtocol.UartEncode(new SerialCommandLED(numeroLed, etat));
                serialPort1.Write(rawData, 0, rawData.Length);
            }
                
        }

        private TextBlock? FindTextBlockForLed(Ellipse ellipse)
        {
            if (ellipse.Parent is Grid grid)
            {
                // La position de l'ellipse dans la grille
                int column = Grid.GetColumn(ellipse);

                foreach (var child in grid.Children)
                {
                    if (child is TextBlock textBlock && Grid.GetColumn(textBlock) == column)
                    {
                        return textBlock;
                    }
                }
            }
            return null;
        }

        private void UpdateTextBlockColor(TextBlock textBlock, SolidColorBrush color)
        {
            if (textBlock != null)
            {
                textBlock.Foreground = color;
            }
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
