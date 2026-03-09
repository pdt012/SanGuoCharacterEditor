using HandyControl.Tools;
using pkHHControlShared.Utils;
using ReactiveUI;
using San11FaceEditorShared.Common;
using SanGuoCharacterEditor.Controls;
using SanGuoCharacterEditor.Core.Models;
using SanGuoCharacterEditor.ViewModels;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SanGuoCharacterEditor.Dialogs
{
    /// <summary>
    /// CharacterEditDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CharacterEditDialog : ReactiveWindow<CharacterEditViewModel>
    {
        public CharacterEditDialog()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);
            });
        }

        public void Commit()
        {
            string? errorMsg = Validation();
            if (string.IsNullOrEmpty(errorMsg))
                DialogResult = true;
            else
                MessageBoxUtil.Error(errorMsg);
        }

        public void Cancel()
        {
            DialogResult = false;
        }

        private string? Validation()
        {
            var character = ViewModel.CharacterVM.Data;
            if (character.PackageName.Length == 0)
                return "包名不可以为空！";
            else if (character.StringId.Length < 2)
                return "字符ID至少2个字符！";
            else if (ViewModel.IsNew && ViewModel.CharacterVM.IdExists(character.Id))
                return "ID重复！";
            return null;
        }

        private void ButtonEditSkills_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SkillSelectDialog dialog = new(GlobalData.Instance.SkillName2IdDict.Keys)
            {
                Owner = WindowHelper.GetActiveWindow(),
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
                SelectedSkills = ViewModel.CharacterVM.Data.Skills
            };
            if (dialog.ShowDialog() == true)
            {
                ViewModel.CharacterVM.Data.Skills = dialog.SelectedSkills.ToImmutableArray();
            }
        }

        private void ButtonEditLike_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TransferDialog dialog = new()
            {
                Title = "选择武将",
                Owner = this,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
                MaxSelections = 5,
                SortTransferredItems = false,
                ItemsSource = ViewModel!.CharacterVM.AllCharacters,
                TransferredItems = ViewModel.CharacterVM.LikedPeople,
            };
            if (dialog.ShowDialog() == true)
            {
                ViewModel!.CharacterVM.LikedPeople = dialog.TransferredItems.Cast<SanGuoCharacterViewModel>().ToImmutableArray();
            }
        }

        private void ButtonEditDislike_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TransferDialog dialog = new()
            {
                Title = "选择武将",
                Owner = this,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
                MaxSelections = 5,
                SortTransferredItems = false,
                ItemsSource = ViewModel!.CharacterVM.AllCharacters,
                TransferredItems = ViewModel.CharacterVM.LikedPeople,
            };
            if (dialog.ShowDialog() == true)
            {
                ViewModel!.CharacterVM.LikedPeople = dialog.TransferredItems.Cast<SanGuoCharacterViewModel>().ToImmutableArray();
            }
        }

        private void cutin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                string? cutinPath = FileDialogUtil.OpenFile("选择暴击图", $"图片文件|*.bmp;*.jpg;*.png", "", "SelectExtraCutin");
                if (cutinPath == null) return;
                BitmapImage? cutinImage = ImageFileHelper.LoadBitmapFile(cutinPath);
                ViewModel.CutinImage = cutinImage;
            }
        }

        private void BtnDeleteCutin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.CutinImage = null;
        }
    }
}
