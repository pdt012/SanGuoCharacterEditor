namespace SanGuoCharacterEditor.Core.CodeConverters
{
    public interface ICodeConverter
    {
        string Decode(ReadOnlySpan<byte> bytes);

        byte[] Encode(string text);

        /// <summary>
        /// unicode字符串转码二进制字符串，考虑缓冲区长度，末尾添\0
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="buffer">缓冲块</param>
        void Encode(string text, Span<byte> buffer);

        public void Encode(string text, byte[] buffer)
        {
            Encode(text, buffer.AsSpan());
        }
    }
}
