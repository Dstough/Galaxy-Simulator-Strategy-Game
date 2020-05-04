using GalaxyServer.Networking;
using System.Net;

namespace Assets.Scripts.Networking.Framework
{
    public class UdpComponent
    {

        public IPEndPoint EndPoint;

        private readonly int _id;

        public UdpComponent(int id) => _id = id;

        public void Connect(IPEndPoint endPoint) => EndPoint = endPoint;

        public void HandleData(Packet packetData)
        {
            var packetLength = packetData.ReadInt();
            var packetBytes = packetData.ReadBytes(packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (var packet = new Packet(packetBytes))
                {
                    var packetId = packet.ReadInt();
                    //TODO: Handle Packet
                }
            });
        }

        public void Disconnect()
        {
            EndPoint = null;
        }
    }
}
