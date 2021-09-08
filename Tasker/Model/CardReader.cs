using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace Tasker.Model
{
    public class CardReader
    {
        SerialPort port;
        Thread readThread;
        public CardReader()
        {
            SerialPortProgram();
        }      
        private void SerialPortProgram()
        {
            port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            port.ReadTimeout = 1500;
            port.WriteTimeout = 1500;

            // Attach a method to be called when there
            // is data waiting in the port's buffer 
            port.DataReceived += port_DataReceived;
            // Begin communications 
            port.Open();
            port.WriteLine("$");

        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string message = port.ReadExisting();
                Log.logThis($"card readed: {message}");
                //MessageBox.Show(message);
                long ID = long.Parse(message);
                NewCardRead?.Invoke(ID);
            }
            catch
            {
                NewCardRead?.Invoke(0);
            }            
        }
        public Action<long> NewCardRead;
    }

}
