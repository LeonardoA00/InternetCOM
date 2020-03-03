using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {

        const int port = 8888;

        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(port);
            serverSocket.Start();
            Console.WriteLine("...Server started");
            TcpClient clientSocket = default(TcpClient);
            clientSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine("...Accept connection from client");

            int requestCount = 0;
            while(true)
            {
                try
                {
                    requestCount++;
                    NetworkStream networkStream = clientSocket.GetStream();

                    byte[] bytesFrom = new byte[10025];
                    int size = (int)clientSocket.ReceiveBufferSize;
                    if (size == 65536)
                        continue;
                    networkStream.Read(bytesFrom, 0, size);
                    string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine($">> {dataFromClient}");

                    string serverResponse = "Recieved request " + requestCount.ToString();
                    byte[] sendBytes = System.Text.Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine($"--> {serverResponse}");
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
