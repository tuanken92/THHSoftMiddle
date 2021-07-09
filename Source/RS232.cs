using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THHSoftMiddle.Source
{
    public class RS232
    {
        /*string comport;
        uint baudrate;*/
        string data_receive;
        private SerialPort serialPort;

        public string Data_receive { get => data_receive; set => data_receive = value; }

        public RS232(string comport, int baudrate)
        {
            serialPort = new SerialPort();
            serialPort.PortName = comport;
            serialPort.BaudRate = baudrate;
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        public RS232(Param_COM param_com)
        {
            serialPort = new SerialPort();
            serialPort.PortName = param_com.Comport;
            serialPort.BaudRate = param_com.Baudrate;
            serialPort.DataReceived += SerialPort_DataReceived;
        }


        public bool Get_State()
        {
            return serialPort.IsOpen;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Data_receive = this.serialPort.ReadExisting();
            //Console.WriteLine($"data rs232 received: {Data_receive}");
        }

        ~RS232()
        {
            Close();
        }

        public bool Open()
        {
            bool open_result = false;
            if (!serialPort.IsOpen)
            {

                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    open_result = true;
                }
            }
            else
            {
                open_result = true;
            }

            return open_result;
        }

        public bool Close()
        {
            bool close_result = false;
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            else
            {
                return true;
            }

            if (!serialPort.IsOpen)
                close_result = true;
            
            return close_result;
        }

        public bool SendData(string data)
        {
            bool send_result = false;
            //care data
            if (string.IsNullOrWhiteSpace(data))
                return send_result;

            //send
            if (serialPort.IsOpen) 
            {
                serialPort.Write(data);
                send_result = true;
            }

            //return
            return send_result;
        }
    }

}
