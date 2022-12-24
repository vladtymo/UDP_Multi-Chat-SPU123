using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ChatServer
{
    public class ChatServer
    {
        private IPEndPoint? clientEndPoint = null;
        private UdpClient server;

        private List<string> history = new List<string>();
        private HashSet<IPEndPoint> members = new HashSet<IPEndPoint>();

        public ChatServer(short port)
        {
            //IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            server = new UdpClient(port);
        }

        public void SendMessage(string message)
        {
            history.Add(message);

            byte[] data = Encoding.UTF8.GetBytes(message);

            foreach (var member in members)
            {
                server.SendAsync(data, data.Length, member);
            }
        }
        public void Join(IPEndPoint endPoint)
        {
            members.Add(endPoint);
        }
        public void Leave(IPEndPoint endPoint)
        {
            members.Remove(endPoint);
        }

        public void Start()
        {
            while (true)
            {
                Console.WriteLine("Waiting for the request...");

                byte[] request = server.Receive(ref clientEndPoint);

                string message = Encoding.UTF8.GetString(request);
                Console.WriteLine($"Got a message: {message} from {clientEndPoint}");

                switch (message)
                {
                    case "<join>":
                        Join(clientEndPoint);
                        break;

                    case "<leave>":
                        Leave(clientEndPoint);
                        break;

                    default:
                        SendMessage(message);
                        break;
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            ChatServer server = new ChatServer(3344);
            server.Start();
        }
    }
}