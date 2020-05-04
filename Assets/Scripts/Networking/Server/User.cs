using Assets.Scripts.Networking.Framework;
using System.IO;
using System.Net.Sockets;
namespace Assets.Scripts.Networking.Server
{
    public class User
    {
        public TcpComponent Tcp;
        public UdpComponent Udp;

        private int _id;
        private NetworkStream _stream;

        public User(int id)
        {
            _id = id;
            Tcp = new TcpComponent(id);
            Udp = new UdpComponent(id);
        }
    }
}
