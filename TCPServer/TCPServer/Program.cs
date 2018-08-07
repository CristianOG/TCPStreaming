using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPServer
{
    class Program
    {
        static TcpListener server = null;

        static void Main(string[] args)
        {
            try
            {
                server = new TcpListener(IPAddress.Parse("127.0.0.1"), 13000);
                server.Start();

                Console.WriteLine("Listening... ");

                // Enter the listening loop.
                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");

                    string[] waitingConnection = new string[] { "Waiting for connection.", "Waiting for connection..", "Waiting for connection..." };
                    int cont = 0;
                    while (!server.Pending())
                    {
                        Console.Clear();
                        Console.WriteLine(waitingConnection[cont]);
                        
                        if (cont >= waitingConnection.Length - 1)
                        {
                            cont = 0;
                        }
                        else
                        {
                            cont++;
                        }
                        Thread.Sleep(500);
                    }
                    
                    Console.Clear();

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    Console.WriteLine("Waiting for client");

                    string request = string.Empty;
                    while (client.Connected)
                    {
                        if (stream.DataAvailable)
                        {
                            request = ReadText(stream);
                            switch (request)
                            {
                                case "hello":
                                    SendInt(stream, 1);
                                    break;

                                case "get movie size":
                                    SendInt(stream, 104);
                                    break;

                                case "get movie":

                                    byte[] file = new byte[104];
                                    var bytes = BitConverter.GetBytes(1);
                                    for (int i = 0; i < file.Length; i += 4)
                                    {
                                        file[i] = bytes[0];
                                        file[i + 1] = bytes[1];
                                        file[i + 2] = bytes[2];
                                        file[i + 3] = bytes[3];
                                    }

                                    SendBytes(stream, file);

                                    //client.Close();
                                    break;
                            }
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        private static string ReadText(NetworkStream stream)
        {
            var buffer = new byte[1024];
            int i;
            string response = string.Empty;

            // Loop to receive all the data sent by the client.
            while (stream.DataAvailable)
            {
                i = stream.Read(buffer, 0, buffer.Length);
                response += System.Text.Encoding.ASCII.GetString(buffer, 0, i);
            }

            // Translate data bytes to a ASCII string.
            
            Console.WriteLine("");
            Console.WriteLine("Received text: {0}", response);

            return response;
        }

        private static void SendText(NetworkStream stream, string message)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            // Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", message);
        }

        private static void SendInt(NetworkStream stream, int message)
        {
            byte[] data = BitConverter.GetBytes(message);

            // Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", message);
        }

        private static void SendBytes(NetworkStream stream, byte[] bytes)
        {
            // Send the message to the connected TcpServer. 
            stream.Write(bytes, 0, bytes.Length);

            Console.WriteLine("Sent: bytes");
        }
    }
}
