using System;
using System.Net;
using System.Net.Sockets;

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
                    string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    if(hubconnection == false)
                    {
                        Console.WriteLine($">> {dataFromClient}");
                        string serverResponse = "Recieved request " + requestCount.ToString();
                        byte[] sendBytes = System.Text.Encoding.ASCII.GetBytes(serverResponse);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();
                        Console.WriteLine($"--> {serverResponse}");
                    }
                    else
                    {
                        string name = dataFromClient.Substring(0, dataFromClient.IndexOf('|'));
                        string type = dataFromClient.Substring(dataFromClient.IndexOf('|') + 1, dataFromClient.LastIndexOf('|') - dataFromClient.IndexOf('|') - 1);
                        string value;
                        if (type == "STR")
                            value = dataFromClient.Substring(dataFromClient.LastIndexOf('|') + 1, dataFromClient.Length - dataFromClient.LastIndexOf('|') - 1);
                        else if(type == "INT")
                        {

                        }
                    }
                    
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
