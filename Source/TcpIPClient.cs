using System;
using System.Net.Sockets;
using System.Text;

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
    class TcpIPClient
    {
        string ip;
        int port;
        TcpClient server;
        NetworkStream ns = null;
        StateObject state = null;

        public StringBuilder data_receive = null;

        public TcpIPClient(string ip, int port)
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
            server.Client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback((new TcpIPClient(this.ip, this.port)).OnReceive), state);

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

                        data_receive = new StringBuilder(state.sb.ToString());
                        Console.WriteLine($"data tcpip received: {data_receive.ToString()}");

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
