using System;
using System.Net.Sockets;

namespace SensorHub
{
 
    class Data
    {
        private string _name;
        private byte[] _numeric_value = new byte[4];
        private byte[] _string_value = new byte[64];
        private Type _type;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public byte[] Value
        {
            get 
            {
                if (_type == typeof(string))
                    return _string_value;
                else //is a int or float
                    return _numeric_value;
            }
        }

        public string Type
        {
            get
            {
                if (_type == typeof(int))
                    return "INT";
                if (_type == typeof(float))
                    return "FLT";
                if (_type == typeof(string))
                    return "STR";
                return "VOID";
            }
        }
        public int SetValue(int value)
        {
            _type = typeof(int);
            _numeric_value = BitConverter.GetBytes(value);
            return 0;
        }
        public int SetValue(float value)
        {
            _type = typeof(float);
            _numeric_value = BitConverter.GetBytes(value);
            return 0;
        }
        public int SetValue(string value)
        {
            _type = typeof(string);
            _string_value = System.Text.Encoding.ASCII.GetBytes(value);
            return 0;
        }

        public Data(string name, int value) { Name = name; SetValue(value); }
        public Data(string name, float value) { Name = name; SetValue(value); }
        public Data(string name, string value) { Name = name; SetValue(value); }

    }

    class Program
    {
        static System.Net.Sockets.TcpClient socket;

        //ENVIROMENT CONSTANTS
        const string ServerIP = "127.0.0.1"; //192.168.178.157
        const int port = 8888;
        const int buffersize = 1024;

        static int Main(string[] args)
        {
            EndOfString = Convert.ToByte('|');
            EndOfTransmission = Convert.ToByte('$');

            if (Connect() != 0)
                return -1;

            Data[] datas = GetDatas();
            foreach (Data d in datas)
            {
                if (Transmit(d) != 0)
                {
                    Console.WriteLine("Transmission failed");
                    return -2;
                }

            }

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


        static byte EndOfString;
        static byte EndOfTransmission;
        static int Transmit(Data data)
        {
            // | End of string
            // $ End of Transmission
            // Packets are like this
            //
            //    name|type|value$
            //
            // These constants are converted in bytes and converted once at the start of the program

            NetworkStream stream = socket.GetStream();
            byte[] nameb = System.Text.Encoding.ASCII.GetBytes(data.Name);
            byte[] typeb = System.Text.Encoding.ASCII.GetBytes(data.Type);
            
            //Creating the packets
            int index = 0;
            byte[] outstream = new byte[nameb.Length + 1 + typeb.Length + 1 + data.Value.Length + 1];
            nameb.CopyTo(outstream, index);
            index = nameb.Length;
            outstream[index] = EndOfString;
            index++;
            typeb.CopyTo(outstream, index);
            index += typeb.Length;
            outstream[index] = EndOfString;
            index++;
            data.Value.CopyTo(outstream, index);
            index += data.Value.Length;
            outstream[index] = EndOfTransmission;

            stream.Write(outstream, 0, outstream.Length);
            stream.Flush();

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
