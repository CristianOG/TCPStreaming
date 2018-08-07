using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TCPClient
{
    class Program
    {
        static NetworkStream stream;
        static TcpClient client;
        static int port = 13000;
        static string ip = "127.0.0.1";

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to a TCP Client App.");
            Console.WriteLine("Press enter to establish a connection with the TCP Server.");
            Console.Read();

            while (!Connect())
            {
                Console.WriteLine("Retrying connection.");
                Thread.Sleep(500);
            }

            // Handshake
            WriteText("hello");
            int responseCode = ReadInt();
            if (responseCode == 1)
            {
                ShowMenu();
            }
            else
            {
                Console.WriteLine("Error on handshake.");
                ReadConsole();
            }

            // Close everything.
            //stream.Close();
            //client.Close();


            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

        private static int ReadConsole()
        {
            FlushKeyboard();
            return Console.Read();
        }

        private static void FlushKeyboard()
        {
            while (Console.In.Peek() != -1)
                Console.In.Read();
        }

        private static void WaitForData()
        {
            string[] loading = new string[] { "Loading.", "Loading..", "Loading..." };
            int pos = 0;
            while (!stream.DataAvailable)
            {
                Console.Clear();
                Console.WriteLine(loading[pos]);

                if (pos >= loading.Length - 1)
                {
                    pos = 0;
                }
                else
                {
                    pos++;
                }
                Thread.Sleep(100);
            }
        }

        private static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome, chose and option.");
            Console.WriteLine("1. Get file");
            //Console.WriteLine("2. Get file");
            FlushKeyboard();

            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case '1':
                    RequestFile();
                    break;

                case '2':
                    break;
            }

        }

        private static void RequestFile()
        {
            WriteText("get movie size");
            int length = ReadInt();

            if (length > 0)
            {
                WriteText("get movie");

                byte[] data = new byte[length];
                int offset = 0;
                int chunckSize = 100;
                byte[] bytes = new byte[chunckSize];
                int bytesRead = 0;
                while ((bytesRead = ReadBytes(bytes, 0)) != 0)
                { 
                    if (offset + bytes.Length > length)
                    {
                        int j = 0;
                        for (int i = offset; i < length; i++)
                        {
                            data[i] = bytes[j];
                            j++;
                        }
                        break;
                    }
                    else
                    {
                        bytes.CopyTo(data, offset);
                        offset += chunckSize;
                        bytes = new byte[chunckSize];
                    }
                    
                }
            }
        }

        private static bool Connect()
        {
            try
            {
                // Create a TcpClient.
                client = new TcpClient(ip, port);

                // Get a client stream for reading and writing.
                stream = client.GetStream();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            return true;
        }

        private static string ReadText()
        {
            var buffer = new byte[1024];
            int i;
            string response = string.Empty;

            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                
            }

            // Translate data bytes to a ASCII string.
            response = System.Text.Encoding.ASCII.GetString(buffer, 0, i);
            Console.WriteLine("");
            Console.WriteLine("Received text: {0}", response);

            return response;
        }

        private static int ReadInt()
        {
            var buffer = new byte[sizeof(int)];
            int i;

            // Loop to receive all the data sent by the client.
            while (stream.DataAvailable)
            {
                i = stream.Read(buffer, 0, buffer.Length);
            }

            int response = -1;
            try
            {
                response = BitConverter.ToInt32(buffer, 0);
            }
            catch { }

            //Console.WriteLine("");
            //Console.WriteLine("Received int: {0}", response);

            return response;
        }

        private static int ReadBytes(byte[] buffer, int offset = 0)
        {
            int i = 0;

            // Loop to receive all the data sent by the client.
            if (stream.DataAvailable)
            {
                i = stream.Read(buffer, offset, buffer.Length);
            }

            Console.WriteLine("");
            Console.WriteLine("byte chunk received: {0}", buffer.Length);

            return i;
        }

        private static byte[] ReadAllBytes(int chunkSize)
        {
            var buffer = new byte[chunkSize];
            int i;

            // Loop to receive all the data sent by the client.
            while (stream.DataAvailable)
            {
                i = stream.Read(buffer, 0, buffer.Length);
            }

            Console.WriteLine("");
            Console.WriteLine("byte chunk received: {0}", buffer.Length);

            return buffer;
        }

        private static float ReadFloat()
        {
            var buffer = new byte[sizeof(float)];
            int i;

            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
            {

            }

            float response = -1;
            try
            {
                response = BitConverter.ToSingle(buffer, 0);
            }
            catch { }

            Console.WriteLine("");
            Console.WriteLine("Received float: {0}", response);

            return response;
        }

        private static void WriteText(string message)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            // Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", message);
            
            WaitForData();
        }
    }
}
