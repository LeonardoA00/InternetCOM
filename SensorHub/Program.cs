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
        System.Net.Sockets.TcpClient socket = new System.Net.Sockets.TcpClient();

        //ENVIROMENT CONSTANTS
        const string ServerIP = "127.0.0.1"; //192.168.178.157
        const int port = 8888;
        const int buffersize = 64;

        static void Main(string[] args)
        {
            Data mydata = new Data("temp", 25000);
            Console.WriteLine(mydata.Value);


        }

        int Transmit()
        {
            return 0;
        }
    }
}
