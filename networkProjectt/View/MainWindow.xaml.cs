using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.Win32;
using networkProjectt.Model;
using networkProjectt.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace networkProjectt
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel(this);
        }
        public  void ConnectToServer()
        {
            int attempts = 0;

            while (!MainViewModel.ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                   MainViewModel. ClientSocket.Connect(IPAddress.Loopback,MainViewModel. PORT);
                }
                catch (SocketException)
                {
                }
            }

            MessageBox.Show("Connected");
        }
        public  void RequestLoop()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    ReceiveResponse();
                }
            });
        }
    
        public void ReceiveResponse()
        {
            var buffer = new byte[1000000];
            int received =MainViewModel. ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            var text = Encoding.Unicode.GetString(data);
                IntegrateToView(text);
            IntegrateImageToView();
        }
     
        public  void IntegrateToView(string text)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Button spt1 = new Button();
                spt1.Content = "you: " + text;
                spt1.FontSize = 14;
                if(text.Length > 20)
                {
                    spt1.Content = null;
                }
                spt1.IsEnabled = false;
                list.Items.Add(spt1);
            });
        }
        public  void IntegrateImageToView()
        {
            Task.Run(() =>
            {
            App.Current.Dispatcher.Invoke(() =>
            {
               MainViewModel. spt1 = new System.Windows.Controls.Image();
                MainViewModel. spt1.Width = 60;
                MainViewModel. spt1.Height = 60;
                MainViewModel. spt1.IsEnabled = false;
                MainViewModel. spt1.Source = new BitmapImage(new Uri(MainViewModel.Path2));
                list.Items.Add(MainViewModel.spt1);
            });
            });  
        }

        
        private void button_Click(object sender, RoutedEventArgs e)
        {
            ConnectToServer();
            RequestLoop();
        }
    }
}
