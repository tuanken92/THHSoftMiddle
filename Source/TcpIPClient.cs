using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace THHSoftMiddle.Source
{
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();


        public void Release()
        {
            if (workSocket != null)
            {
                Console.WriteLine("free socket...");
                workSocket.Dispose();
            }
            Array.Clear(buffer, 0, buffer.Length);
            sb.Clear();
        }
    }
    [Serializable]
    class TcpIPClient
    {
        string ip;
        int port;
        IPEndPoint IP;
        Socket client;
        bool enable_run;
        string data_receive;
        Thread thread_read_data;
        public string Data_receive { get => data_receive; set => data_receive = value; }

        public TcpIPClient(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
                       
            Console.WriteLine($"TCP Client (0): port = {port}, ip = {ip}");
        }

        public bool Connect()
        {
            bool connect_result = false;
           
            IP = new IPEndPoint(IPAddress.Parse(ip), port);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            try 
            {
                client.Connect(IP);
            } 
            catch {
                Console.WriteLine("Can't to the Server...");
                return false;
            }
            
            enable_run = true;
            thread_read_data = new Thread(Receive);
            thread_read_data.IsBackground = true;
            thread_read_data.Start();

            connect_result = client.Connected;
            
            Console.WriteLine("Connected to the Server...");

            return connect_result;
        }

        public bool Get_State()
        {
            return client.Connected;
        }

        
        public bool Disconnect()
        {
            client.Close();
            return !client.Connected;
        }

        public bool SendData(string data)
        {
            bool send_data = false;
            if (!string.IsNullOrEmpty(data))
            {
                if (client.Connected)
                {
                    var x = client.Send(Serialize(data));
                    if (x > 0)
                        send_data = true;
                }
            }

            return send_data;
        }
        
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }

        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        // Convert an object to a byte array
        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }



        void Receive()
        {
            try
            {
                while(client.Connected && enable_run)
                {
                    byte[] data = new byte[1024 * 5000];
                    int k = client.Receive(data);
                    data_receive = Encoding.ASCII.GetString(data, 0, k);

                    Console.WriteLine(data_receive);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception = " + e.Message);
            }
            
        }
    }

    class TcpIPClient2
    {
        string ip;
        int port;
        TcpClient server;
        NetworkStream ns;
        StateObject state;

        string data_receive;

        public string Data_receive { get => data_receive; set => data_receive = value; }

        public TcpIPClient2(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            this.state = new StateObject();

            Console.WriteLine($"TCP Client (0): port = {port}, ip = {ip}");
        }

        public bool Connect()
        {
            bool connect_result = false;
            try
            {
                server = new TcpClient(this.ip, this.port);
                this.state = new StateObject();
            }
            catch (SocketException)
            {
                Console.WriteLine("Unable to connect to server");
                return connect_result;
            }

            Console.WriteLine("Connected to the Server...");
            ns = server.GetStream();

            state.workSocket = server.Client;
            server.Client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback((new TcpIPClient2(this.ip, this.port)).OnReceive), state);

            if (state.workSocket.Connected)
            {
                connect_result = true;
            }
            return connect_result;
        }

        public bool Get_State()
        {
            if (state.workSocket != null)
                return state.workSocket.Connected;
            else
                return false;

        }

        public String Get_Data()
        {
            return Data_receive;
        }
        public void OnReceive(IAsyncResult ar)
        {


            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int bytesRead;

            if (handler.Connected)
            {

                // Read data from the client socket. 
                try
                {
                    bytesRead = handler.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        // There  might be more data, so store the data received so far.
                        state.sb.Remove(0, state.sb.Length);
                        state.sb.Append(Encoding.ASCII.GetString(
                                         state.buffer, 0, bytesRead));

                        Data_receive = state.sb.ToString();
                        Console.WriteLine($"data tcpip received: {Data_receive}");

                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(OnReceive), state);

                    }
                }

                catch (SocketException socketException)
                {
                    //WSAECONNRESET, the other side closed impolitely
                    if (socketException.ErrorCode == 10054 || ((socketException.ErrorCode != 10004) && (socketException.ErrorCode != 10053)))
                    {
                        handler.Close();
                    }
                }

                // Eat up exception....Hmmmm I'm loving eat!!!
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message + "\n" + exception.StackTrace);

                }

            }
        }

        public bool Disconnect()
        {
            state.workSocket.Close();
            ns.Close();
            server.Close();
            Console.WriteLine("Disconnect to server");
            if (!state.workSocket.Connected)
            {
                return true;
            }
            return false;
        }

        public bool SendData(string data)
        {
            bool send_data = false;
            if (!string.IsNullOrEmpty(data))
            {
                if (server.Connected)
                {
                    ns.Write(Encoding.ASCII.GetBytes(data), 0, data.Length);
                    ns.Flush();
                    send_data = true;
                }
            }

            return send_data;
        }
    }
}
