using System.Text;

namespace SanGuoCharacterEditor.Core.CodeConverters
{
    public class Big5CodeConverter : ICodeConverter
    {
        private static readonly Encoding encoding =
            Encoding.GetEncoding("BIG5",
                EncoderFallback.ReplacementFallback,
                new DecoderReplacementFallback("□"));

        public string Decode(ReadOnlySpan<byte> bytes)
        {
            return encoding.GetString(bytes);
        }

        public byte[] Encode(string text)
        {
            return encoding.GetBytes(text);
        }

        public void Encode(string text, Span<byte> buffer)
        {
            var data = Encode(text);

            int len = Math.Min(data.Length, buffer.Length - 1);
            data.AsSpan(0, len).CopyTo(buffer);
            buffer[len] = 0;
        }
    }
}
