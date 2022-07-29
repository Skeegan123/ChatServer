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
    internal class Server
    {
        public static Hashtable clientList = new Hashtable();
        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Any, 8635);
            TcpClient clientSocket = default(TcpClient);
            int counter;

            serverSocket.Start();

            Console.WriteLine("Chat Server Started");
            counter = 0;
            
            while (true)
            {
                counter = counter + 1;
                clientSocket = serverSocket.AcceptTcpClient();

                byte[] bytesFrom = new byte[10025];
                string username = null;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                username = System.Text.Encoding.ASCII.GetString(bytesFrom);
                username = username.Substring(0, username.IndexOf("$"));

                clientList.Add(username, clientSocket);

                Broadcast(username + " Joined ", username, false);
                Console.WriteLine(username + " connected to chat room");

                HandleClient client = new HandleClient();
                client.StartClient(clientSocket, username, clientList);
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }
        
        public static void Broadcast(string message, string username, bool flag)
        {
            foreach (DictionaryEntry client in clientList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)client.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();

                Byte[] broadcastBytes = null;
                if (flag == true)
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(username + " says : " + message);
                }
                else
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(message);
                }
                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }
    }
}
