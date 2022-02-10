using Microsoft.Win32;
using networkProjectt.Command;
using networkProjectt.Helper;
using networkProjectt.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
namespace networkProjectt.ViewModel
{
    class MainViewModel : BaseViewModel
    {
        public static readonly Socket ClientSocket = new Socket
   (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static System.Windows.Controls.Image spt1;
        public static int PORT = 27001;
        public DispatcherTimer DispatcherTimer { get; set; } = new DispatcherTimer();
        public RelayCommand CloseCommand { get; set; }
        public List<byte> voices { get; set; }
        public int Count { get; set; }
        public string VoicePath { get; set; }
        public  static Image image { get; set; }
        public static TcpClient Client { get; set; }
        private ObservableCollection<Imagee> allImages;
        public  ObservableCollection<Imagee> AllImages
        {
            get { return allImages; }
            set { allImages = value; OnPropertyChanged(); }
        }
        public static List<Imagee> Images { get; set; }
        private ObservableCollection<Voice> allVoice;
        public ObservableCollection<Voice> AllVoice
        {
            get { return allVoice; }
            set { allVoice = value; OnPropertyChanged(); }
        }
        public static List<Voice> Voices { get; set; }
        private MediaPlayer mediaPlayer = new MediaPlayer();
        public MainWindow MainWindow { get; set; }
        public RelayCommand VoiceCommand { get; set; }
        public RelayCommand VoiceClick { get; set; }
        public static RelayCommand PhotoCommand { get; set; }
        public RelayCommand VideoCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand SendCommand { get; set; }
        public static string Path2 { get; set; }
        public static string Path3 { get; set; }
        public MainViewModel(MainWindow mainwindow)
        {
            MainWindow = mainwindow;
            CloseCommand = new RelayCommand((s) =>
            {
                mainwindow.window.Close();
            });
            SendCommand=new RelayCommand ((s)=>{
                Task.Run(() =>
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Button spt1 = new Button();
                        spt1.Content = "me: " + mainwindow.messtxtb.Text;
                        spt1.FontSize = 14;
                        spt1.IsEnabled = false;

                   mainwindow.list.Items.Add(spt1);
                        string request = mainwindow.messtxtb.Text.ToString();
                        mainwindow. messtxtb.Text = String.Empty;
                       SendHelper. SendString(request);
                        MainViewModel.image.Visibility = Visibility.Hidden;
                        MainViewModel.Path2 = null;
                        mainwindow.list.Items.Remove(MainViewModel.image);
                    });
                });
            });
            VideoCommand = new RelayCommand((s) =>
            {
                mediaPlayer.Stop();
            });  
           
            int count = 0;
       
             VoiceCommand = new RelayCommand((s) =>
            {
                //++count;
              
                App.Current.Dispatcher.Invoke(() =>
                {
                   
                   
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                        mediaPlayer.Open(new Uri( openFileDialog.FileName));
                        mediaPlayer.Play();
                        Button button = new Button();
                        button.Width = 100;
                        button.Height = 40 ;
                        button.Content = "mp3";
                        button.Background = Brushes.Lime;
                        mainwindow.list.Items.Add(button);
                    }
                });
            });
            PhotoCommand = new RelayCommand((s) => {
                Task.Run(() =>
                {
                App.Current.Dispatcher.Invoke(() =>
                      {
                        OpenFileDialog open = new OpenFileDialog();
                          open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
                          if (open.ShowDialog() == true)
                    {
                        MainViewModel.Path2 = open.FileName;
                           image = new System.Windows.Controls.Image();
                              image.Width = 60;
                              image.Height = 60;
                              image.IsEnabled = false;
                          image.Source = new BitmapImage(new Uri(MainViewModel.Path2));
                              mainwindow.list.Items.Add(image);
                              var imageBytes = ImageConvert.GetBytesOfImage(MainViewModel.Path2);
                              if (!ClientSocket.Connected)
                              {
                                  MainWindow.ConnectToServer();
                              }
                              string jsonString = JsonConvert.SerializeObject(imageBytes);
                              byte[] buffer = Encoding.ASCII.GetBytes(jsonString);
                              var path = ImageConvert.GetImagePath(imageBytes, 0);
                             ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                              Images.Add(new Imagee
                              {
                                  ImagePath = path
                              });
                              AllImages = new ObservableCollection<Imagee>(Images);
                              if (path == "ok")
                              {
                                  ClientSocket.Shutdown(SocketShutdown.Both);
                                  ClientSocket.Dispose();
                              }
                              image.Source=null;
                              Path2 = String.Empty ;
                          }
                      });
            });

        });
    } 
    }
}


