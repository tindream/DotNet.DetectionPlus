using Paway.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DetectionPlus
{
    public class MessageBase
    {
        /// <summary>
        /// 从机地址(站号，随便指定，00  -- FF 都可以)
        /// </summary>
        public byte Address { get; set; } = 1;
        /// <summary>
        /// 寄存器类型-功能码
        /// </summary>
        public byte Code { get; set; }

        public MessageBase() { }
        public MessageBase(byte code)
        {
            this.Code = code;
        }

        #region 发送消息：消息体>十六进制
        public byte[] Buffer()
        {
            var data = string.Empty;
            Data(this, ref data);
            var msg = $"{Address:X2}{Code:X2}{data}";
            msg = msg.Replace("-", "").Replace(" ", "");
            var buffer = Method.HexStrToByteArr(msg);
            var calc = CRC16(buffer);
            return buffer.Concat(calc).ToArray();
        }
        public static byte[] CRC16(byte[] data)
        {
            int len = data.Length;
            if (len > 0)
            {
                ushort crc = 0xFFFF;

                for (int i = 0; i < len; i++)
                {
                    crc = (ushort)(crc ^ (data[i]));
                    for (int j = 0; j < 8; j++)
                    {
                        crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                    }
                }
                byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
                byte lo = (byte)(crc & 0x00FF);         //低位置

                return new byte[] { lo, hi };
            }
            return new byte[] { 0, 0 };
        }
        private void Data(object obj, ref string data)
        {
            foreach (var item in obj.GetType().PropertiesCache())
            {
                var type = item.PropertyType;
                var value = item.GetValue(obj);
                switch (type.Name)
                {
                    case nameof(Int16):
                        data += BitConverter.ToString(BitConverter.GetBytes((short)value).Reverse().ToArray());
                        break;
                    default:
                        if (value is IList list)
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                data += BitConverter.ToString(BitConverter.GetBytes((short)list[i]).Reverse().ToArray());
                            }
                        }
                        break;
                }
            }
        }

        #endregion
        #region 接收消息：十六进制>消息体
        public void Parse(string data)
        {
            var index = 0;
            data = data.Replace("-", "").Replace(" ", "");
            byte[] buffer = Method.HexStrToByteArr(data);
            Parse(this, buffer, ref index);
            if (buffer.Length != index) throw new Exception($"未定义的参数，长度：{buffer.Length - index}");
        }
        private void Parse(object obj, byte[] buffer, ref int index)
        {
            foreach (var item in obj.GetType().PropertiesCache())
            {
                var type = item.PropertyType;
                switch (type.Name)
                {
                    case nameof(Int16):
                        var temp = GetValue(type, buffer, index);
                        item.SetValue(obj, temp, null);
                        index += Marshal.SizeOf(type);
                        break;
                    default:
                        if (item.GetValue(obj) is IList list)
                        {
                            var childType = list.GenericType();
                            var length = Marshal.SizeOf(childType);
                            while (buffer.Length >= index + length)
                            {
                                var child = GetValue(childType, buffer, index);
                                list.Add(child);
                                index += Marshal.SizeOf(childType);
                            }
                        }
                        break;
                }
            }
        }
        private object GetValue(Type type, byte[] buffer, int index)
        {
            switch (type.Name)
            {
                case nameof(Int16):
                    if (buffer.Length < index + 2) throw new WarningException("参数不足");
                    return BitConverter.ToInt16(buffer.Skip(index).Take(2).Reverse().ToArray(), 0);
            }
            return null;
        }

        #endregion
    }
}
