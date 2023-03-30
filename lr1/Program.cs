using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace lr1
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine($"Enter the listening port:");
            int listeningPort = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine($"Enter the port to send:");
            int sendingPort = Convert.ToInt32(Console.ReadLine());

            Task recieveTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    using (Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                    {
                        var localIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), listeningPort);
                        udpSocket.Bind(localIP);

                        byte[] data = new byte[1024];

                        EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

                        var result = udpSocket.ReceiveFrom(data, ref remoteIp);
                        var recievedData = Encoding.UTF8.GetString(data, 0, result);

                        int ind1 = recievedData.IndexOf('_');
                        int ind2 = recievedData.LastIndexOf('_');

                        var message = recievedData.Substring(0, ind1);
                        var size = Convert.ToInt32(recievedData.Substring(ind1 + 1, ind2 - ind1 - 1));
                        var date = Convert.ToDateTime(recievedData.Substring(ind2 + 1));
                        var success = message.Length == size;


                        Console.WriteLine($"The message came successfully: {success}");
                        Console.WriteLine($"Sent at {date}");
                        Console.WriteLine($"Message received: {message}\n");
                    }
                }
            });
            Task sendTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    using (var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                    {
                        string message = Console.ReadLine();
                        message += $"_{message.Length}_{DateTime.Now}";
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        EndPoint remotePoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), sendingPort);
                        udpSocket.SendTo(data, remotePoint);
                    }
                }
            });

            recieveTask.Wait();
            sendTask.Wait();
        }
    }
}
