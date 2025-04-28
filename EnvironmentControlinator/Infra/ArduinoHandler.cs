using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace EnvironmentControlinator.Infra
{
    public static class ArduinoHandler
    {
        private static SerialPort serialPort;

        static ArduinoHandler()
        {
            var ports = SerialPort.GetPortNames();

            if (ports.Length == 0)
            {
                Console.WriteLine("Nenhuma porta encontrada para conexão com a estante");
                return;
            }
            serialPort = new SerialPort();

            if (serialPort.IsOpen == false)
            {
                serialPort.PortName = "COM3";
            }
        }

        private static object _lock = new object();

        public static void SendMessage(String message)
        {
            lock (_lock)
            {
                serialPort.Open();

                serialPort.Write(message);

                serialPort.Close();
            }
        }
    }
}
