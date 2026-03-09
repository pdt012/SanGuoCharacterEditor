using System.Text;
using System.Xml;

namespace SanGuoCharacterEditor.Core.CodeConverters
{
    public class PK22CodeConverter : ICodeConverter
    {
        private const byte ASCII_MAX = 0x7F;
        // ushort -> Unicode
        private static readonly char?[] bytes2uniMap = new char?[65536];
        // Unicode -> ushort
        private static readonly Dictionary<char, ushort> uni2bytesMap = new();

        /// <summary>
        /// 码表初始化
        /// </summary>
        public static void Init(string codeTablePath)
        {
            Array.Clear(bytes2uniMap, 0, bytes2uniMap.Length);
            uni2bytesMap.Clear();

            XmlDocument doc = new();
            doc.Load(codeTablePath);

            XmlElement? root = doc.DocumentElement;
            XmlNodeList? nodeList = root?.SelectNodes("ch");

            if (nodeList == null)
                return;

            foreach (XmlNode node in nodeList)
            {
                var attrs = node.Attributes;
                if (attrs == null)
                    continue;

                string? k = attrs["key"]?.Value;
                string? v = attrs["value"]?.Value;

                if (k == null)
                    continue;

                ushort code = Convert.ToUInt16(k, 16);

                if (!string.IsNullOrEmpty(v))
                {
                    char c = v[0];
                    bytes2uniMap[code] = c;
                    uni2bytesMap[c] = code;
                }
                else
                {
                    bytes2uniMap[code] = null;
                }
            }
        }

        public string Decode(ReadOnlySpan<byte> bytes)
        {
            StringBuilder result = new(bytes.Length);
            int i = 0;
            while (i < bytes.Length)
            {
                byte b = bytes[i];
                if (b == 0)
                    break;
                if (b <= ASCII_MAX)
                {
                    result.Append((char)b);
                }
                else
                {
                    if (i + 1 >= bytes.Length)  // 意外结尾
                    {
                        result.Append('□');
                        break;
                    }
                    byte b2 = bytes[i + 1];
                    ushort code = (ushort)((b << 8) | b2);
                    char? c = bytes2uniMap[code];
                    result.Append(c ?? '□');
                    i++;
                }
                i++;
            }
            return result.ToString();
        }

        public byte[] Encode(string text)
        {
            List<byte> result = new(text.Length * 2);
            foreach (char c in text)
            {
                if (c <= ASCII_MAX)
                {
                    result.Add((byte)c);
                }
                else if (uni2bytesMap.TryGetValue(c, out ushort code))
                {
                    result.Add((byte)(code >> 8));
                    result.Add((byte)(code & 0xFF));
                }
                else
                {
                    result.Add((byte)'?');
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// unicode字符串转码二进制字符串，考虑缓冲区长度，末尾添\0
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="buffer">缓冲字节数组</param>
        public void Encode(string text, byte[] buffer)
        {
            if (buffer.Length == 0)
                return;
            byte[] data = Encode(text);

            int len = Math.Min(data.Length, buffer.Length - 1);
            Array.Copy(data, buffer, len);
            buffer[len] = 0;  // 末尾添\0
        }
    }
}