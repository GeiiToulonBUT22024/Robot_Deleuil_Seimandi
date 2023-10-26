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

namespace robotInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool btnClickFlag = false;
        private ReliableSerialPort serialPort1;

        public MainWindow()
        {
            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM6", 115200, Parity.None, 8, StopBits.One);
            serialPort1.Open();
        }
        
        private bool sendMessage(bool key) // key = true si appuie sur Enter et false autrement
        {
            if (textBoxEmission.Text == "\r\n" || textBoxEmission.Text == "") return false;

            serialPort1.WriteLine(textBoxEmission.Text);
            // RichTextBox.Text += "Reçu: " + textBoxEmission.Text + (key ? "" : "\n");
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
