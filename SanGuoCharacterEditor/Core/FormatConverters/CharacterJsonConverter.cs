using SanGuoCharacterEditor.Core.Models;
using System.IO;
using System.Text.Json;

namespace SanGuoCharacterEditor.Core.FormatConverters
{
    public class CharacterJsonConverter
    {
        public static void ToJson(string jsonPath, List<SanGuoCharacter> character)
        {
            using FileStream fileStream = new(jsonPath, FileMode.Create, FileAccess.Write);
            JsonSerializer.Serialize(fileStream, character, new JsonSerializerOptions { WriteIndented = false });
        }

        public static List<SanGuoCharacter> FromJson(string jsonPath)
        {
            using FileStream fileStream = File.OpenRead(jsonPath);
            var obj = JsonSerializer.Deserialize<List<SanGuoCharacter>>(fileStream);
            if (obj is List<SanGuoCharacter> t)
                return t;
            else
                return new List<SanGuoCharacter>();
        }
    }
}
