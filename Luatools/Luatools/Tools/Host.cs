using System;
using System.Collections.Generic;

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
        public static byte[] Encode(byte id, byte[] data)
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

        //上次收到的残留数据
        private static List<byte> lastData = new List<byte>();
        /// <summary>
        /// 解析收到的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns>数据列表</returns>
        public static List<byte[]> Decode(byte[] data, bool check = false)
        {
            List<byte[]> result = new List<byte[]>();
            List<byte> clean = new List<byte>();//待清理掉多余数据
            clean.AddRange(lastData);
            clean.AddRange(data);
            if (clean.IndexOf(0xad) < 0)
            {
                lastData.Clear();
                return result;
            }
            clean.RemoveRange(0, clean.IndexOf(0xad));//切掉开头多余的数据

            for (int i = 0; i < clean.Count; i++)//反转义处理
            {
                if (clean[i] == 0x5c)
                {
                    clean[i] = (byte)~clean[i + 1];
                    clean.RemoveAt(i + 1);
                }
            }

            while (true)
            {
                if(clean.Count < 4)//长度过短
                    break;

                if (clean.IndexOf(0xad) < 0)
                {
                    lastData.Clear();
                    return result;
                }
                clean.RemoveRange(0, clean.IndexOf(0xad));//切掉开头多余的数据

                int len = clean[1] * 256 + clean[2];//获取包长度
                if (clean.Count < 4 + len)//长度过短
                    break;

                List<byte> temp = new List<byte>(clean.GetRange(3, len + 1));//临时存储
                clean.RemoveRange(0, len + 4);//去掉取出的数据

                if (check)//是否需要校验
                {
                    byte id = temp[0];//id
                    for (int i = 1; i < temp.Count - 1; i++)//检查校验
                        id ^= temp[i];
                    if (id == temp[temp.Count - 1])//符合校验
                    {
                        temp.RemoveAt(temp.Count - 1);
                        result.Add(temp.ToArray());
                    }
                }
                else
                {
                    temp.RemoveAt(temp.Count - 1);
                    result.Add(temp.ToArray());
                }
            }

            lastData = clean;//剩下的扔到缓存里

            return result;
        }
    }
}
