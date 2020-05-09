using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ConnectedClient
{
	readonly TcpClient connection;
	readonly byte[] buffer = new byte[NetworkConfig.BUFFER_SIZE];
	NetworkStream Stream
	{
		get => connection.GetStream();
	}


	public ConnectedClient(TcpClient client)
	{
		connection = client;
		connection.NoDelay = true;
		if(NetworkManager.instance.isServer)
		{
			Stream.BeginRead(buffer, 0, NetworkConfig.BUFFER_SIZE, OnRead, null);
		}
	}

	internal void EndConnect(IAsyncResult arg)
	{
		connection.EndConnect(arg);
		Stream.BeginRead(buffer, 0, NetworkConfig.BUFFER_SIZE, OnRead, null);
	}

	private void OnRead(IAsyncResult arg)
	{
		var length = Stream.EndRead(arg);
		if(length <= 0)
		{
			NetworkManager.instance.OnDisconnect(this);
			return;
		}

		var message = Encoding.UTF8.GetString(buffer, 0, length);
		NetworkManager.instance.MessageToDisplay.text += message + Environment.NewLine;

		if (NetworkManager.instance.isServer)
		{
			foreach(var client in NetworkManager.instance.clientList)
			{
				client.Send(message);
			}
		}

		Stream.BeginRead(buffer, 0, NetworkConfig.BUFFER_SIZE, OnRead, null);
	}

	internal void Send(string message)
	{
		var buffer = Encoding.UTF8.GetBytes(message);
		Stream.Write(buffer, 0, buffer.Length);
	}

	internal void Close()
	{
		connection.Close();
	}
}