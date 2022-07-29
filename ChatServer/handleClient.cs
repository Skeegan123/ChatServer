using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace ChatServer
{
    internal class HandleClient
    {
        TcpClient clientSocket;
        string username;
        Hashtable clientList;

        public void StartClient(TcpClient clientSocket, string username, Hashtable clientList)
        {
            this.clientSocket = clientSocket;
            this.username = username;
            this.clientList = clientList;
            Thread ctThread = new Thread(GetMessage);
            ctThread.Start();
        }

        private void GetMessage()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            Byte[] sendBytes = null;
            string serverResponse = null;
            string message = null;
            string rCount = null;

            while (true)
            {
                try
                {
                    requestCount++;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    message = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    message = message.Substring(0, message.IndexOf("$"));
                    if (message == "/disconnect")
                    {
                        Console.WriteLine("Client " + username + " disconnected");
                        Server.Broadcast("Client " + username + " disconnected", username, false);
                        this.clientList.Remove(username);
                        continue;
                    }

                    Console.WriteLine(this.username + " wrote: " + message);
                    rCount = requestCount.ToString();

                    Server.Broadcast(message, username, true);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
