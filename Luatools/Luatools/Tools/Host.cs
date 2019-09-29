using System;
using System.Collections.Generic;
using System.Text;

namespace Luatools.Tools
{
    class Host
    {

        /// <summary>
        /// 发送数据前先处理
        /// </summary>
        /// <param name="id">命令id</param>
        /// <param name="data">数据内容</param>
        /// <returns>处理完的一包数据</returns>
        public static byte[] Send(byte id,byte[] data)
        {
            //先转为list
            List<byte> temp = new List<byte>(data);
            //最终结果存这里
            List<byte> result = new List<byte>
            {
                0xad,
                (byte)((data.Length+1) / 256),
                (byte)((data.Length+1) % 256),
                id
            };
            
            for (int i = 0; i < temp.Count; i++)
            {
                id ^= temp[i];
                switch (temp[i])
                {
                    case 0x11:
                    case 0x13:
                    case 0x5c:
                        byte t = temp[i];
                        temp.RemoveAt(i);
                        temp.InsertRange(i, new List<byte> { 0x5c, (byte)~t });
                        i++;
                        break;
                }
            }
            switch (id)
            {
                case 0x11:
                case 0x13:
                case 0x5c:
                    temp.AddRange(new List<byte> { 0x5c, (byte)~id });
                    break;
                default:
                    temp.Add(id);
                    break;
            }
            result.AddRange(temp);

            return result.ToArray();
        }
    }
}
