using System.IO.Ports;
using System;
using NLua;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace Luatools
{
    class Program
    {
        private static uint count = 1;
        static void Main(string[] args)
        {
            var uart = new Models.Uart()
            {
                Delay = 10,
            };
            uart.serial.BaudRate = 921600;
            
            if(SerialPort.GetPortNames().Length == 0)//没串口
            {
                Console.WriteLine($"no port");
                return;
            }
            else if(SerialPort.GetPortNames().Length == 1)//一个串口
            {
                uart.serial.PortName = SerialPort.GetPortNames()[1];
            }
            else
            {
                Console.WriteLine($"found ports:");
                for (int i = 0; i < SerialPort.GetPortNames().Length; i++)
                    Console.WriteLine($"{i}.\t{SerialPort.GetPortNames()[i]}");
                Console.Write($"please select your port(type number):");
                string port = Console.ReadLine();
                try
                {
                    int porti = int.Parse(port);
                    uart.serial.PortName = SerialPort.GetPortNames()[porti];
                }
                catch
                {
                    Console.Write($"number not right, exit");
                    return;
                }
            }

            try
            {
                uart.Init();
                uart.serial.Open();
                Console.WriteLine($"port {uart.serial.PortName} opend.");
            }
            catch(Exception e)
            {
                Console.WriteLine($"open uart fail: {e}");
                return;
            }

            uart.DataReceived += (o, d) =>
            {
                foreach (byte[] b in Tools.Host.Decode(d))
                {
                    if (b.Length > 0 && b[0] == 0x80)
                    {
                        List<byte> temp = new List<byte>(b);
                        temp.RemoveRange(0, 3);
                        int index = temp.IndexOf(0);
                        if (index > 0)
                        {
                            temp.RemoveRange(index, temp.Count - index);
                            Console.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss:ffff]")+Encoding.Default.GetString(temp.ToArray()));
                        }
                    }
                }
                sendCount(uart);
            };

            //获取日志
            Task.Run(() =>
            {
                while(true)
                {
                    Task.Delay(100).Wait();
                    //uart.SendData(Tools.Host.Encode(0x86, new byte[] { 0, 1, 8, 0, 0x8f }));
                    sendCount(uart);
                }
            });

            //byte[] test = Hex2Byte("AD-00-03-FF-01-80-7E-AD-00-08-83-6F-7C-05-00-1B-35-02-00-AD-00-34-80-EF-01-5B-49-5D-2D-5B-74-69-63-6B-5D-09-31-33-33-38-35-32-30-38-37-39-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-74-69-63-6B-5D-09-31-00-00-00-00-AD-00-08-83-8A-7C-05-00-0E-3D-02-00-AD-00-30-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-74-63-5D-09-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-00-00-00-00-AD-00-08-83-8B-7C-05-00-5E-3D-02-00-AD-00-3A-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-74-63-5D-09-43-41-4C-4C-20-52-45-41-44-59-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-00-00-00-00-00-00-AD-00-08-83-8C-7C-05-00-AF-3D-02-00-AD-00-3A-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-64-65-66-75-72-63-5D-09-43-41-4C-4C-20-52-45-41-44-59-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-64-65-66-00-00-00-00-00-00-AD-00-08-83-8D-7C-05-00-0D-3E-02-00-AD-00-30-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-74-63-5D-09-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-00-00-00-00-AD-00-08-83-8F-7C-05-00-AE-3E-02-00-AD-00-46-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-74-63-5D-09-2B-43-52-45-47-3A-20-31-2C-22-31-38-34-35-22-2C-22-36-39-36-38-22-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-00-00-00-00-00-00-AD-00-08-83-95-7C-05-00-4F-40-02-00-AD-00-36-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-73-65-6E-64-61-74-5D-09-41-54-2B-43-53-51-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-73-65-6E-00-00-00-00-00-00-AD-00-08-83-9D-7C-05-00-CC-42-02-00-AD-00-30-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-74-63-5D-09-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-00-00-00-00-AD-00-3A-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-74-63-5D-09-2B-43-53-51-3A-20-32-35-2C-30-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-00-00-00-00-00-AD-00-08-83-9E-7C-05-00-10-43-02-00-AD-00-30-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-74-63-5D-09-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-00-00-00-00-AD-00-32-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-74-63-5D-09-4F-4B-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-06-88-00-00-00-00-AD-00-46-80-EF-01-5B-49-5D-2D-5B-6E-65-74-2E-72-73-70-5D-09-41-54-2B-43-53-51-09-74-72-75-65-09-4F-4B-09-2B-43-53-51-3A-20-32-35-2C-30-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-6E-65-74-2E-72-73-70-36-80-00-00-00-00-AD-00-08-83-9F-7C-05-00-83-43-02-00-AD-00-38-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-73-65-6E-64-61-74-5D-09-41-54-2B-43-45-4E-47-3F-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-73-65-6E-00-00-00-00-AD-00-08-83-A1-7C-05-00-05-44-02-00-AD-00-30-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-74-63-5D-09-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-00-00-00-00-AD-00-39-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-74-63-5D-09-2B-43-45-4E-47-3A-31-2C-31-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-30-30-30-00-00-00-00-AD-00-08-83-A2-7C-05-00-49-44-02-00-AD-00-62-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-74-63-5D-09-2B-43-45-4E-47-3A-30-2C-22-30-30-31-32-2C-32-35-2C-39-39-2C-34-36-30-2C-30-30-2C-35-37-2C-32-36-39-38-34-2C-31-30-2C-30-35-2C-36-32-31-33-2C-30-32-22-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-5B-63-00-00-00-00-AD-00-08-83-A4-7C-05-00-F4-44-02-00-AD-00-30-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-74-63-5D-09-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-00-00-00-00-AD-00-32-80-EF-01-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-61-74-63-5D-09-4F-4B-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-72-69-6C-2E-70-72-6F-06-88-00-00-00-00-AD-00-08-83-A5-7C-05-00-3C-45-02-00-AD-00-41-80-EF-01-5B-49-5D-2D-5B-6E-65-74-2E-72-73-70-5D-09-41-54-2B-43-45-4E-47-3F-09-74-72-75-65-09-4F-4B-09-6E-69-6C-00-DB-5E-32-82-E9-7D-06-88-EC-5E-32-82-5B-49-5D-2D-5B-6E-65-74-2E-72-73-70-75-61-6C-00-00-00-00");
            //Console.WriteLine(Tools.Host.Decode(test).Count);

            //Lua state = new Lua();
            //Console.WriteLine(state.DoString("return 123")[0]);

            Console.ReadLine();
        }


        public static void sendCount(Models.Uart uart)
        {
            uart.SendData(Tools.Host.Encode(0xff, new byte[] {
                    4,
                    3,
                    (byte)(count / 256 / 256 / 256 % 256),
                    (byte)(count / 256/256 % 256),
                    (byte)(count/256 % 256),
                    (byte)(count%256)
                }));
            count++;
            if (count >= 256 * 256 * 256)
                count = 1;
        }


        /// <summary>
        /// hex转byte
        /// </summary>
        /// <param name="mHex">hex值</param>
        /// <returns>原始字符串</returns>
        public static byte[] Hex2Byte(string mHex)
        {
            mHex = System.Text.RegularExpressions.Regex.Replace(mHex, "[^0-9A-Fa-f]", "");
            if (mHex.Length % 2 != 0)
                mHex = mHex.Remove(mHex.Length - 1, 1);
            if (mHex.Length <= 0) return new byte[0];
            byte[] vBytes = new byte[mHex.Length / 2];
            for (int i = 0; i < mHex.Length; i += 2)
                if (!byte.TryParse(mHex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber, null, out vBytes[i / 2]))
                    vBytes[i / 2] = 0;
            return vBytes;
        }
    }
}
