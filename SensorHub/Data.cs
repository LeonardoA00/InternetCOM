using System;
using System.Collections.Generic;
using System.Text;

namespace SensorHub
{
    public class Data
    {
        private const int _string_byte_len = 64;
        private const int _numerical_byte_len = 4;

        private string _name;
        private string _unit;
        private byte[] _numeric_value = new byte[_numerical_byte_len];
        private byte[] _string_value = new byte[_string_byte_len];
        private Type _type;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
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
        public object FormattedValue
        {
            get
            {
                if (_type == typeof(string))
                    return System.Text.Encoding.ASCII.GetString(_string_value);
                else if (_type == typeof(int))
                    return BitConverter.ToInt32(_numeric_value);
                else if (_type == typeof(float))
                    return BitConverter.ToSingle(_numeric_value);
                else
                    return "ERR";
            }
        }

        public override string ToString()
        {
            return Name + " | " + Type + " | " + FormattedValue + " " + Unit;
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

        public Data(string name, int value, string unit) { Name = name; SetValue(value); Unit = unit; }
        public Data(string name, float value, string unit) { Name = name; SetValue(value); Unit = unit; }
        public Data(string name, string value, string unit) { Name = name; SetValue(value); Unit = unit; }
        public Data(string name, Type type, byte[] value, string unit) 
        { 
            Name = name; 
            _type = type;
            Unit = unit;
            if (type == typeof(string)) 
                value.CopyTo(_string_value, 0); 
            else 
                value.CopyTo(_numeric_value, 0); 
        }

        public static byte EndOfString = Convert.ToByte('|');
        public static byte EndOfTransmission = Convert.ToByte('$');

        public static byte[] Pack(Data data)
        {
            // | End of string
            // $ End of Transmission
            // Packets are like this
            //
            //    name|type|unit|value$
            //
            // These constants are converted in bytes and converted once at the start of the program
            byte[] nameb = System.Text.Encoding.ASCII.GetBytes(data.Name);
            byte[] typeb = System.Text.Encoding.ASCII.GetBytes(data.Type);
            byte[] unitb = System.Text.Encoding.ASCII.GetBytes(data.Unit);

            int index = 0;
            byte[] outstream = new byte[nameb.Length + 1 + typeb.Length + 1 + unitb.Length + 1 + data.Value.Length + 1];
            
            nameb.CopyTo(outstream, index);
            index = nameb.Length;
            outstream[index] = EndOfString;
            index++;
            
            typeb.CopyTo(outstream, index);
            index += typeb.Length;
            outstream[index] = EndOfString;
            index++;
            
            unitb.CopyTo(outstream, index);
            index += unitb.Length;
            outstream[index] = EndOfString;
            index++;
            
            data.Value.CopyTo(outstream, index);
            index += data.Value.Length;
            outstream[index] = EndOfTransmission;

            return outstream;
        }

        private static int LastIndexOf(byte[] array, byte b)
        {
            int index = -1;
            for (int i = 0; i < array.Length; i++)
                if (array[i] == b)
                    index = i;
            return index;
        }

        private static int nIndexOf(byte[] array, byte b, int n)
        {
            int count = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == b)
                {
                    count++;
                    if (count == n)
                        return i;
                }
            }
            return -1;
        }
        
        private static int FirstIndexOf(byte[] array, byte b)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == b)
                    return i;
            return -1;
        }

        //Unpack a formatted byte array into a data object
        public static Data UnPack(byte[] packet)
        {
            //cutstart is the index of the first element to cut, cutcount is the dimension of the new cutted array
            int cutstart = 0, cutcount = FirstIndexOf(packet, EndOfString);
            string name = System.Text.Encoding.ASCII.GetString(packet, cutstart, cutcount);
            
            cutstart = cutcount + 1;
            cutcount = nIndexOf(packet, EndOfString, 2) - cutstart;
            string type = System.Text.Encoding.ASCII.GetString(packet, cutstart, cutcount);
            Type t = typeof(void);
            if (type == "STR")
                t = typeof(string);
            else if (type == "INT")
                t = typeof(int);
            else if (type == "FLT")
                t = typeof(float);
            
            cutstart = cutstart + cutcount + 1;
            cutcount = LastIndexOf(packet, EndOfString) - cutstart;
            string unit = System.Text.Encoding.ASCII.GetString(packet, cutstart, cutcount);

            cutstart = cutstart + cutcount + 1;
            cutcount = LastIndexOf(packet, EndOfTransmission) - cutstart;
            byte[] value;
            if (t == typeof(string))
                value = new byte[_string_byte_len];
            else
                value = new byte[_numerical_byte_len];

            for (int i = cutstart, j = 0; j < cutcount; i++, j++)
                value[j] = packet[i];

            Data res = new Data(name, t, value, unit);
            return res;
        }
    }
}
