using SanGuoCharacterEditor.ViewModels;
using System.Collections.ObjectModel;

namespace SanGuoCharacterEditor.Services
{
    public interface ICharacterProvider
    {
        SanGuoCharacterViewModel? GetCharacter(string id);

        ObservableCollection<SanGuoCharacterViewModel> GetCharacterCollection();
    }
}
