using System;
using System.Net.Sockets;

namespace SensorHub
{
    class Program
    {
        static System.Net.Sockets.TcpClient socket;

        //ENVIROMENT CONSTANTS
        const string ServerIP = "127.0.0.1"; //192.168.178.157
        const int port = 8888;
        const int buffersize = 1024;

        static int Main(string[] args)
        {
            Console.WriteLine("--- SensorHub ---");
            Console.Title = "SensorHub";

            //Connection phase. Calls the function to connect and after failed attempts request the user to go on
            while(Connect() != 0)
            {
                Console.WriteLine("Could not connect to the server. Try again? (y/n)");
                bool exit = !ynToBool(Console.ReadLine());
                if (exit)
                    return -1;
            }
            Console.WriteLine($"Connected succesfully to {ServerIP}:{port}");

            //Gets data and transmits it
            Console.WriteLine("\nSending data...");
            while (true)
            {
                Data[] datas = GetDatas();
                Transmit(datas);
                System.Threading.Thread.Sleep(200);
            }

            Console.WriteLine("Done...");
            Console.ReadLine();
            return 0;
        }

        static Data[] GetDatas()
        {
            Data[] res = new Data[2];
            Random gen = new Random();

            res[0] = new Data("TMP", gen.Next(19, 23) + (float)gen.NextDouble(), "°C");
            res[1] = new Data("PRS", gen.Next(800, 1500), "kPa");

            return res;
        }

        static int Transmit(Data[] data)
        {
            foreach (Data d in data)
            {
                if (Transmit(d) != 0)
                    return -1;
                System.Threading.Thread.Sleep(200);
                //Find ideal idle time
            }

            return 0;
        }

        static int Transmit(Data data)
        {
            try
            {
                NetworkStream stream = socket.GetStream();

                //Creating the packets
                byte[] outstream = Data.Pack(data);
                stream.Write(outstream, 0, outstream.Length);
                stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }
            return 0;
        }

        static int Connect()
        {
            socket = new System.Net.Sockets.TcpClient();

            bool connected = false;
            int attempts = 0;
            char[] loading_bar = { '|', '/', '-', '\\'};

            Console.Write("Connecting... ");
            while (!connected && attempts < 20)
            {
                Console.Write($"({loading_bar[attempts % 4]})");
                try
                {
                    socket.Connect(ServerIP, port);
                    socket.ReceiveBufferSize = buffersize;

                    connected = true;
                }
                catch (Exception) { }
                //System.Threading.Thread.Sleep(100);
                Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);

                attempts++;
            }
            Console.WriteLine();

            if (connected)
                return 0;
            else
                return -1;
        }

        static bool ynToBool(string response)
        {
            if (response == "y" || response == "Y")
                return true;
            else
                return false;
        }
    }
}
