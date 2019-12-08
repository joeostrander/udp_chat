/* 
 *  Based on code from:  https://gist.github.com/darkguy2008/413a6fea3a5b4e67e5e0d96f750088a9 
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace udp_chat
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                show_usage();
                return;
            }


            string server_ip;
            int port_server;
            int port_client;

            if (!args[0].Contains(":"))
            {
                show_usage();
                Console.WriteLine("Bad server:port");
                return;
            }

            string[] server_and_port = args[0].Split(':');

            server_ip = server_and_port[0];

            if (!Int32.TryParse(server_and_port[1], out port_server))
            {
                show_usage();
                Console.WriteLine("Bad server port");
                return;
            }

            if (!Int32.TryParse(args[1], out port_client))
            {
                show_usage();
                Console.WriteLine("Bad listen port");
                return;
            }

            UDPSocket s = new UDPSocket();
            s.Server(port_server);
            s.debug = false;

            UDPSocket c = new UDPSocket();
            c.Client(server_ip, port_client);
            c.debug = false;

            while (true)
            {
                var key = Console.ReadKey();
                //Debug.WriteLine("Send key:  {0}", (int)key.KeyChar);
                if ( key.Key == ConsoleKey.Enter)
                {
                    c.Send("\n");
                    Console.WriteLine();
                }
                else
                {
                    c.Send(key.KeyChar.ToString());
                }
            }

        }

        static void show_usage()
        {
            Console.WriteLine("Joe Ostrander");
            Console.WriteLine("2019.12.08");
            Console.WriteLine();
            Console.WriteLine("Usage:  udp_chat.exe <server:port> <listen port>");
            Console.WriteLine();

        }
    }
}

