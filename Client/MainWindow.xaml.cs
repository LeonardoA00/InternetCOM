﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;

namespace Client
{
    public partial class MainWindow : Window
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        
        //ENVIROMENT CONSTANTS
        const string ServerIP = "127.0.0.1"; //192.168.178.157
        const int port = 8888;
        const int buffersize = 10000;

        public MainWindow()
        {
            InitializeComponent();
            servermsg("Client Started");
            try 
            { 
                clientSocket.Connect(ServerIP, port);
                clientSocket.ReceiveBufferSize = buffersize;
                ConnectionStatus.Content = "Client Socket Program - Server Connected ...";
            } 
            catch (Exception ex)
            {
                ConnectionStatus.Content = "Unable to connecto to the server... Restart the app";
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            NetworkStream serverStream = clientSocket.GetStream();
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(ClientTextBox.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            byte[] inStream = new byte[buffersize];
            serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
            string returndata = System.Text.Encoding.ASCII.GetString(inStream);
            returndata = returndata.Substring(0, returndata.IndexOf('\0'));
            servermsg(returndata);
            ClientTextBox.Text = "";
            ClientTextBox.Focus();
        }

        public void servermsg(string mesg)
        {
            ServerTextBox.Text += ">> " + mesg + Environment.NewLine;
        }
    }
}
