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
            Data tmp = new Data("test", 3.5f);
            Console.WriteLine(tmp.ToString());
            byte[] arr = Data.Pack(tmp);
            Data tmp2 = Data.UnPack(arr);
            Console.WriteLine(tmp2.ToString());
            Console.ReadLine();
            if (Connect() != 0)
                return -1;

            Data[] datas = GetDatas();
            Transmit(datas);

            Console.WriteLine("Done...");
            Console.ReadLine();
            return 0;
        }

        static Data[] GetDatas()
        {
            Data[] res = new Data[3];
            res[0] = new Data("temperature", 23.5f);
            res[1] = new Data("pressure", 25);
            res[2] = new Data("light", "day");

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
            try
            {
                socket.Connect(ServerIP, port);
                socket.ReceiveBufferSize = buffersize;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to connecto to the server... Restart the app");
                Console.WriteLine(ex.ToString());
                return -1;
            }
            return 0;
        }
    }
}
