using System.IO.Ports;
using System;
using NLua;

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

            
            Lua state = new Lua();
            Console.WriteLine(state.DoString("return 123")[0]);

            Console.ReadLine();
        }
    }
}
