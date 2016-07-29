using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient tcpClient;
        private Stream stream;

        public MainWindow()
        {
            InitializeComponent();

            Initiate();
        }

        private void SendText(object sender, RoutedEventArgs e)
        {
            Write();
        }

        private void Initiate()
        {
            tcpClient = new TcpClient();
            tcpClient.Connect("192.168.1.13", 1337);
            stream = tcpClient.GetStream();

            Thread startListener = new Thread(Listen);
            startListener.SetApartmentState(ApartmentState.STA);
            startListener.Start();
        }

        private void Write()
        {
            String textToSend = text.Text;

            TextBlock tb = new TextBlock();
            tb.Text = ">> " + textToSend;

            messages.Children.Add(tb);

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(textToSend);

            stream.Write(ba, 0, ba.Length);
        }

        private void Listen()
        {
            while (true)
            {
                byte[] bb = new byte[100];
                int k = stream.Read(bb, 0, 100);

                String message = "";

                for (int i = 0; i < k; i++)
                {
                    message = message + Convert.ToChar(bb[i]).ToString();
                }

                //MessageBox.Show(message);

                messages.Dispatcher.Invoke(
                    new AddTextBlock(this.AddText),
                    new object[] { message }
                );
            }
        }

        private delegate void AddTextBlock(String message);

        private void AddText(String message)
        {
            TextBlock tb = new TextBlock();
            tb.Text = "<< " + message;

            messages.Children.Add(tb);
        }
    }
}
