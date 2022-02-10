using networkProjectt.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace networkProjectt.Helper
{
   public class SendHelper
    {
        public static void SendString(string text = null, byte[] photo = null)
        {
            if (photo == null)
            {
                byte[] buffer = Encoding.Unicode.GetBytes(text);

                MainViewModel.ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
            else if (text == null && photo != null)
            {
                MainViewModel.ClientSocket.Send(photo, 0, photo.Length, SocketFlags.None);
            }
        }
        public static void SendImage(byte[] src)
        {
            MainViewModel.ClientSocket.Send(src, 0, src.Length, SocketFlags.None);
        }
    }
}
