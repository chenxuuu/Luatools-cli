using System.IO.Ports;
using System;

namespace Luatools
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach(string p in SerialPort.GetPortNames())
            {
                Console.WriteLine(p);
            }
        }
    }
}
