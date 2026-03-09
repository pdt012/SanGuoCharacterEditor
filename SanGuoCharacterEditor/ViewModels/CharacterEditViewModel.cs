using ReactiveUI;
using ReactiveUI.SourceGenerators;
using San11FaceEditorShared.ViewModels;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Media;

namespace SanGuoCharacterEditor.ViewModels
{
    public partial class CharacterEditViewModel : ReactiveObject, IActivatableViewModel
    {
        public bool IsNew { get; set; }

        public SanGuoCharacterViewModel CharacterVM { get; }

        public FaceImagesPanelViewModel FaceImagesPanelVM { get; }

        [Reactive]
        private ImageSource? _cutinImage;

        [Reactive]
        private bool _isFaceImageModified = false;

        public ViewModelActivator Activator => CharacterVM.Activator;

        public CharacterEditViewModel(SanGuoCharacterViewModel characterVM, FaceImagesPanelViewModel faceImagesPanelVM, bool isNew)
        {
            IsNew = isNew;
            CharacterVM = characterVM;
            FaceImagesPanelVM = faceImagesPanelVM;
            FaceImagesPanelVM.ImageChanged += () => { IsFaceImageModified = true; };

            this.WhenActivated(disposables =>
            {
                FaceImagesPanelVM.ImageChanged += OnImageChanged;
                Disposable.Create(() => FaceImagesPanelVM.ImageChanged -= OnImageChanged)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.CutinImage)
                    .Skip(1)
                    .Subscribe(_ => OnImageChanged())
                    .DisposeWith(disposables);
            });
        }

        private void OnImageChanged()
        {
            IsFaceImageModified = true;
        }
    }
}
