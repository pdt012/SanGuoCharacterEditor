using DynamicData;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SanGuoCharacterEditor.Core.Models;
using SanGuoCharacterEditor.Services;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace SanGuoCharacterEditor.ViewModels
{
    public partial class SanGuoCharacterViewModel : ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        private readonly ICharacterProvider _characterProvider;

        public SanGuoCharacter Data
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        #region 基础信息

        public string Id => Data.Id;
        public string Name => Data.GetName();
        public string IdName => Data.GetIdName();

        #endregion

        #region 人际关系
        public SanGuoCharacterViewModel? Ancestry
        {
            get => _characterProvider.GetCharacter(Data.AncestryId);
            set
            {
                Data.AncestryId = value?.Id ?? "";
                this.RaisePropertyChanged(nameof(Ancestry));
            }
        }

        public SanGuoCharacterViewModel? Father
        {
            get => _characterProvider.GetCharacter(Data.FatherId);
            set
            {
                Data.FatherId = value?.Id ?? "";
                this.RaisePropertyChanged(nameof(Father));
            }
        }

        public SanGuoCharacterViewModel? Mother
        {
            get => _characterProvider.GetCharacter(Data.MotherId);
            set
            {
                Data.MotherId = value?.Id ?? "";
                this.RaisePropertyChanged(nameof(Mother));
            }
        }

        public SanGuoCharacterViewModel? Spouse
        {
            get => _characterProvider.GetCharacter(Data.SpouseId);
            set
            {
                Data.SpouseId = value?.Id ?? "";
                this.RaisePropertyChanged(nameof(Spouse));
            }
        }

        public SanGuoCharacterViewModel? Brother
        {
            get => _characterProvider.GetCharacter(Data.BrotherId);
            set
            {
                Data.BrotherId = value?.Id ?? "";
                this.RaisePropertyChanged(nameof(Brother));
            }
        }

        public ImmutableArray<SanGuoCharacterViewModel> LikedPeople
        {
            get => Data.LikedPersonIdArray.Select(id => _characterProvider.GetCharacter(id)).Where(c => c != null).Cast<SanGuoCharacterViewModel>().ToImmutableArray();
            set
            {
                Data.LikedPersonIdArray = value.Select(c => c.Id).ToImmutableArray();
                this.RaisePropertyChanged(nameof(LikedPeople));
            }
        }
        public ImmutableArray<SanGuoCharacterViewModel> DislikedPeople
        {
            get => Data.DislikedPersonIdArray.Select(id => _characterProvider.GetCharacter(id)).Where(c => c != null).Cast<SanGuoCharacterViewModel>().ToImmutableArray();
            set
            {
                Data.DislikedPersonIdArray = value.Select(c => c.Id).ToImmutableArray();
                this.RaisePropertyChanged(nameof(DislikedPeople));
            }
        }
        #endregion

        #region 能力信息

        [ObservableAsProperty] public partial int StatSum { get; }

        [ObservableAsProperty] public partial int ProficiencySum { get; }

        #endregion

        #region 头像/模型

        #endregion

        #region 剧本所属信息
        public SanGuoCharacterViewModel? ReservedKing
        {
            get => _characterProvider.GetCharacter(Data.ReservedKingId);
            set
            {
                Data.ReservedKingId = value?.Id ?? "";
                this.RaisePropertyChanged(nameof(ReservedKing));
            }
        }

        #endregion

        public ObservableCollection<SanGuoCharacterViewModel> AllCharacters => _characterProvider.GetCharacterCollection();

        public bool IdExists(string id) => _characterProvider.GetCharacter(id) != null;

        public SanGuoCharacterViewModel(SanGuoCharacter data, ICharacterProvider characterProvider)
        {
            Data = data;
            _characterProvider = characterProvider;

            #region ReactiveBindings

            _statSumHelper = this.WhenAnyValue(
                x => x.Data.Stats.Leadership,
                x => x.Data.Stats.Strength,
                x => x.Data.Stats.Intelligence,
                x => x.Data.Stats.Politics,
                x => x.Data.Stats.Charm
                )
                .Select(t => t.Item1 + t.Item2 + t.Item3 + t.Item4 + t.Item5)
                .ToProperty(this, x => x.StatSum);

            _proficiencySumHelper = this.WhenAnyValue(
                x => x.Data.Proficiency.Spearmen,
                x => x.Data.Proficiency.Halberdiers,
                x => x.Data.Proficiency.Crossbowmen,
                x => x.Data.Proficiency.Cavalry,
                x => x.Data.Proficiency.Navy,
                x => x.Data.Proficiency.SiegeUnits
                )
                .Select(t => t.Item1 + t.Item2 + t.Item3 + t.Item4 + t.Item5 + t.Item6)
                .ToProperty(this, x => x.ProficiencySum);

            #endregion

            this.WhenActivated(DoWhenActivated);
        }

        private void DoWhenActivated(CompositeDisposable disposables)
        {
            this.WhenAnyValue(x => x.Data.PackageName, x => x.Data.StringId)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(Id)))
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.Data.FamilyName, x => x.Data.GivenName)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(Name)))
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.Id, x => x.Name)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(IdName)))
                .DisposeWith(disposables);
        }

        public override string ToString()
        {
            return IdName;
        }

    }
}
