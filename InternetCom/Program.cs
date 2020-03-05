using System;
using System.Net;
using System.Net.Sockets;
using SensorHub;

namespace Server
{
    class Program
    {

        const int port = 8888;
        const int buffersize = 1024;
        static bool hubconnection;

        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to a hub? (y/n):");
            string input = Console.ReadLine();
            if (input == "y")
                hubconnection = true;
            else
                hubconnection = false;

            TcpListener serverSocket = new TcpListener(port);
            serverSocket.Start();
            Console.WriteLine("...Server started");
            TcpClient clientSocket = default(TcpClient);
            clientSocket = serverSocket.AcceptTcpClient();
            clientSocket.ReceiveBufferSize = buffersize;
            Console.WriteLine("...Accept connection from client");

            int requestCount = 0;
            while(true)
            {
                try
                {
                    requestCount++;
                    NetworkStream networkStream = clientSocket.GetStream();

                    byte[] bytesFrom = new byte[buffersize];
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    
                    if(hubconnection == false)
                    {
                        string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                        Console.WriteLine($">> {dataFromClient}");
                        string serverResponse = "Recieved request " + requestCount.ToString();
                        byte[] sendBytes = System.Text.Encoding.ASCII.GetBytes(serverResponse);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        Console.WriteLine($"--> {serverResponse}");
                    }
                    else
                    {
                        Data data = Data.UnPack(bytesFrom);
                        Console.WriteLine(data.ToString());
                    }

                    networkStream.Flush();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine(">> Server stopped");
            Console.ReadLine();
        }
    }
}
