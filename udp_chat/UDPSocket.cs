﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace udp_chat
{
    public class UDPSocket
    {
        private Socket _socket;
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;
        public bool debug = false;

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void Server(int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            //_socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            Receive();
        }

        public void Client(string address, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Connect(IPAddress.Parse(address), port);
            Receive();
        }

        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
                if (debug)
                {
                    Console.WriteLine("SEND: {0}, {1}", bytes, text);
                }
            }, state);
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                try
                {
                    State so = (State)ar.AsyncState;
                    int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                    if (debug)
                    {
                        Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
                    }
                    else
                    {
                        string received = Encoding.ASCII.GetString(so.buffer, 0, bytes);

                        Console.Write(received);
                    }

                    _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
                }
                catch { }

            }, state);
        }
    }
}