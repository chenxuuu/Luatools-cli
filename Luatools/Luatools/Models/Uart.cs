using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace Luatools.Models
{
    class Uart
    {
        public SerialPort serial = new SerialPort();
        public event EventHandler<byte[]> DataReceived;
        public event EventHandler<byte[]> UartDataSent;

        //延时
        public int Delay { get; set; } = 50;

        /// <summary>
        /// 初始化串口各个触发函数
        /// </summary>
        public void Init()
        {
            //声明接收到事件
            serial.DataReceived += Serial_DataReceived;
            //serial.BreakState = false;
            serial.DataBits = 8;
            //serial.DtrEnable = false;
            serial.Parity = Parity.None;
            //serial.RtsEnable = false;
            serial.StopBits = StopBits.One;
            serial.ReceivedBytesThreshold = 1;
            serial.ReadTimeout = 10;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据内容</param>
        public void SendData(byte[] data)
        {
            if (data.Length == 0)
                return;
            serial.Write(data, 0, data.Length);
            UartDataSent?.Invoke(this, data);//回调事件
        }

        //接收到事件
        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Task.Delay(Delay).Wait();//等待时间
            if (!serial.IsOpen)//串口被关了，不读了
                return;
            int length = ((SerialPort)sender).BytesToRead;
            byte[] rev = new byte[length];
            ((SerialPort)sender).Read(rev, 0, length);
            if (rev.Length == 0)
                return;
            DataReceived?.Invoke(this, rev);//回调事件
        }
    }
}
