using System.IO.Ports;
using System;
using NLua;
using System.Collections.Generic;

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

            var uart = new Models.Uart();
            uart.DataReceived += (o, d) =>
            {

            };

            Console.WriteLine(BitConverter.ToString(Tools.Host.Send(0xff, new byte[]{ 00,0x11, 0xA1, 0x1B, 0xBA })));

            //Lua state = new Lua();
            //Console.WriteLine(state.DoString("return 123")[0]);

            Console.ReadLine();
        }
    }
}
