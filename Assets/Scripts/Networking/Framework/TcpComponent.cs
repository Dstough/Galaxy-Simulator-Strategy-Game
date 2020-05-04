using GalaxyServer.Networking;
using System;
using System.Net.Sockets;

namespace Assets.Scripts.Networking.Framework
{
    public class TcpComponent
    {
        public TcpClient Socket;

        private readonly int _id;
        private NetworkStream _stream;
        private Packet _receivedData;
        private byte[] _receiveBuffer;

        public TcpComponent(int id) => _id = id;

        public void Connect(TcpClient socket)
        {
            Socket = socket;
            Socket.ReceiveBufferSize = Constants.DATA_BUFFER_SIZE;
            Socket.SendBufferSize = Constants.DATA_BUFFER_SIZE;

            _stream = Socket.GetStream();
            _receiveBuffer = new byte[Constants.DATA_BUFFER_SIZE];
            _receivedData = new Packet();

            _stream.BeginRead(_receiveBuffer, 0, Constants.DATA_BUFFER_SIZE, ReceiveCallback, null);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (Socket == null)
                    return;

                _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data to player {_id} via TCP: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var byteLength = _stream.EndRead(result);

                if (byteLength <= 0)
                {
                    Disconnect();
                    return;
                }

                var data = new byte[byteLength];

                Array.Copy(_receiveBuffer, data, byteLength);

                _receivedData.Reset(HandleData(data));

                _stream.BeginRead(_receiveBuffer, 0, Constants.DATA_BUFFER_SIZE, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving TCP data: {ex}");
                Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            var packetLength = 0;

            _receivedData.SetBytes(data);

            if (_receivedData.UnreadLength() >= 4)
            {
                packetLength = _receivedData.ReadInt();

                if (packetLength <= 0)
                    return true;
            }

            while (packetLength > 0 && packetLength <= _receivedData.UnreadLength())
            {
                var packetBytes = _receivedData.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (var packet = new Packet(packetBytes)) 
                    { 
                        var packetId = packet.ReadInt();
                        //TODO: Handle Packet.
                    }
                });

                packetLength = 0;

                if (_receivedData.UnreadLength() >= 4)
                {
                    packetLength = _receivedData.ReadInt();

                    if (packetLength <= 0)
                        return true;
                }
            }

            if (packetLength <= 1)
                return true;

            return false;
        }

        public void Disconnect()
        {
            Socket.Close();
            _stream = null;
            _receivedData = null;
            _receiveBuffer = null;
            Socket = null;
        }
    }
}
