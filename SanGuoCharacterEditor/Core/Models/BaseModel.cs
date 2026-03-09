using ReactiveUI;

namespace SanGuoCharacterEditor.Core.Models
{
    public class BaseModel : ReactiveObject
    {
        public virtual object DeepClone()
        {
            return MemberwiseClone();
        }
    }
}
