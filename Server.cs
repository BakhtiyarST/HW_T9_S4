﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp2
{
	internal class Server
	{
		private static bool flagContinue=true;
		private static CancellationTokenSource cts = new CancellationTokenSource();
		private static CancellationToken ct = cts.Token;
		
		public static async Task AcceptMsg()
		{
			Console.WriteLine("The server is ready for accepting messages.");
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
			UdpClient udpClient = new UdpClient(12345);

			Task breakingTask = Task.Run(() =>
			{
				Console.ReadKey();
				flagContinue = false;
			});

			while (!ct.IsCancellationRequested)
			{
				byte[] buffer = udpClient.Receive(ref endPoint);
				string data = Encoding.UTF8.GetString(buffer);

				await Task.Run(async() =>
				{
					Message msgResponse;
					Message msg = Message.FromJson(data);

					Console.WriteLine(msg.ToString());
					string textData = msg.Text;
					
					if (textData.ToLower() == "exit")
					{
						msgResponse = new Message("Server", "Exit message received, so server is exiting");
						cts.Cancel();
					}
					else
						msgResponse = new Message("Server", "Message accepted on server side");
					string msgResponseJson = msgResponse.ToJson();
					byte[] dataResponse = Encoding.UTF8.GetBytes(msgResponseJson);
					await udpClient.SendAsync(dataResponse, dataResponse.Length, endPoint);
				});
			}
		}
	}
}