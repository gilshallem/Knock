﻿/*
 *  A Windows port knocking client
 *  Web site: http://sebastien.warin.fr
 *  Copyright (C) 2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 *	
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

namespace Knock
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    class Program
    {
        private static readonly byte[] packetData = new byte[] { 0x00, 0x23, 0xf8, 0x4e, 0x45, 0x7e, 0xf4, 0x6d, 0x04, 0x39, 0xd2, 0xeb, 0x08, 0x00, 0x45, 0x00, 0x00, 0x34, 0x13, 0xea, 0x40, 0x00, 0x80, 0x06, 0x61, 0x19, 0xc0, 0xa8, 0x01, 0x22, 0x5d, 0x59, 0x66, 0x9d, 0x65, 0x48, 0x00, 0x50, 0x05, 0xe6, 0x36, 0xbb, 0x00, 0x00, 0x00, 0x00, 0x80, 0x02, 0x20, 0x00, 0x27, 0xde, 0x00, 0x00, 0x02, 0x04, 0x04, 0xec, 0x01, 0x03, 0x03, 0x08, 0x01, 0x01, 0x04, 0x02 };
        private enum Protocol
        {
            TCP,
            UDP
        }

        static void Main(string[] args)
        {
            
            IPAddress address = null;
            Protocol protocol;

            if (args.Length >= 3 && Enum.TryParse(args[1],true,out protocol) && IPAddress.TryParse(args[0], out address))
            {
        
                string protocolName = args[1].ToUpper();
                foreach (var arg in args.Skip(2))
                {
                    int port = 0;
                    if (int.TryParse(arg, out port))
                    {
                        Console.WriteLine($"Sending {protocolName} packet to {address}:{port}");
                        EndPoint ep = new IPEndPoint(address, port);
                        if (protocol == Protocol.UDP)
                        {
                            pingUDP(ep);
                        }
                        else
                        {
                            pingTCP(ep);
                        }
                        
                    }
                }
                Console.WriteLine("Done");
            }
            else
            {
                Console.WriteLine("usage: knock.exe <IP> <TCP or UDP> <port> [port] ...");
                Console.WriteLine("example: knock.exe TCP 192.168.0.1 123 456 789");
            }
        } 
        private static void pingTCP(EndPoint endPoint)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var res = socket.BeginConnect(endPoint, null, null);
            res.AsyncWaitHandle.WaitOne(100, true);
            if (socket.Connected)
            {
                socket.EndConnect(res);
            }
            socket.Close();
            
        }
        private static void pingUDP(EndPoint endPoint)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SendTo(packetData, endPoint);
            socket.Close();
        }
    }
    
}
