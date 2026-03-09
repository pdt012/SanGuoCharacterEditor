namespace SanGuoCharacterEditor.Core.CodeConverters
{
    public interface ICodeConverter
    {
        string Decode(ReadOnlySpan<byte> bytes);

        byte[] Encode(string text);

        void Encode(string text, byte[] buffer);
    }
}
