using pkHHControlShared.Utils;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using San11FaceEditorShared.Common;
using San11FaceEditorShared.ViewModels;
using SanGuoCharacterEditor.Core.CodeConverters;
using SanGuoCharacterEditor.Core.FormatConverters;
using SanGuoCharacterEditor.Core.Mapper;
using SanGuoCharacterEditor.Core.Models;
using SanGuoCharacterEditor.Services;
using SanGuoCharacterEditor.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SanGuoCharacterEditor.ViewModels
{
    public partial class MainViewModel : ReactiveObject, ICharacterProvider
    {
        public SettingsManager<AppConfig> Config { get; init; }
        public ObservableCollection<SanGuoCharacterViewModel> Characters { get; } = new();

        private readonly Dictionary<string, SanGuoCharacterViewModel> _characterMap = new();

        [Reactive] private SanGuoCharacterViewModel? _selectedItem;

        public ReactiveCommand<SanGuoCharacterViewModel, bool> EditCommand { get; }
        public ReactiveCommand<IEnumerable<object>, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        public Interaction<CharacterEditViewModel, bool?> EditInteraction { get; } = new();

        public MainViewModel()
        {
            var hasSelection = this
                .WhenAnyValue(x => x.SelectedItem)
                .Select(item => item != null);

            EditCommand = ReactiveCommand.CreateFromTask<SanGuoCharacterViewModel, bool>(async vm =>
            {
                return await FuncEditCharacterAsync(vm);
            }, hasSelection);
            DeleteCommand = ReactiveCommand.Create<IEnumerable<object>>(FuncDeleteCharacters, hasSelection);
            AddCommand = ReactiveCommand.CreateFromTask(FuncAddCharacterAsync);

            // 载入配置
            Config = new(new AppConfig(), "config.json");
        }

        public void Init()
        {
            // 码表
            PK22CodeConverter.Init(Config.Value.CodeTablePath);
            // 自动载入全局配置
            GlobalData.Instance.LoadGlobalScenario(Config.Value.GlobalScenarioPath);
            // PK2.2的特技-id对照表
            GlobalData.Instance.LoadPK22Skills(Config.Value.SkillNamesPath);

#if DEBUG
            // 自动加载剧本数据
            List<SanGuoCharacter> characters = CharacterPK22ScenConverter.FromPK22Scenario(Config.Value.LastSenarioPath);
            foreach (SanGuoCharacter character in characters)
            {
                AddOrUpdateCharacter(character);
            }
#endif
        }

        /// <summary>
        /// 检查必备的数据是否已经加载
        /// </summary>
        public bool CheckAppData()
        {
            if (GlobalData.Instance.SkillId2NameDict.Count == 0)
            {
                MessageBoxUtil.Error("请先载入特技-id对照表文件（skill.xml）");
                return false;
            }
            if (GlobalData.Instance.ProvinceId2NameDict.Count == 0)
            {
                MessageBoxUtil.Error("请先载入全局配置文件（Scenario.s11）");
                return false;
            }
            return true;
        }

        public void SetMediaExPath()
        {
            string? openDirName = FileDialogUtil.OpenDirectory("选择mediaEX文件夹", Config.Value.MediaExPath, "SetMediaExPath");
            if (openDirName == null) return;
            if (Path.GetFileName(openDirName) != "mediaEX")
            {
                MessageBoxUtil.Error("请选择mediaEX文件夹！");
            }
            Config.Update(x => x with { MediaExPath = openDirName });
            MessageBoxUtil.Success("设置成功");
        }

        public void LoadCodeTable()
        {
            string? openFileName = FileDialogUtil.OpenFile("选择PK2.2码表文件", "码表xml|enc_3.xml", Config.Value.CodeTablePath, "LoadCodeTable");
            if (openFileName == null) return;
            PK22CodeConverter.Init(Config.Value.CodeTablePath);
            Config.Update(x => x with { CodeTablePath = openFileName });
            MessageBoxUtil.Success("载入成功");
        }

        public void LoadGlobalScenario()
        {
            string? openFileName = FileDialogUtil.OpenFile("选择全局剧本文件", "s11全局剧本|Scenario.s11", Config.Value.GlobalScenarioPath, "LoadGlobalScenario");
            if (openFileName == null) return;
            GlobalData.Instance.LoadGlobalScenario(openFileName);
            Config.Update(x => x with { GlobalScenarioPath = openFileName });
            MessageBoxUtil.Success("载入成功");
        }

        public void LoadSkillNames()
        {
            string? openFileName = FileDialogUtil.OpenFile("选择特技xml", "特技xml|*skill.xml", Config.Value.SkillNamesPath, "LoadSkillNames");
            if (openFileName == null) return;
            GlobalData.Instance.LoadPK22Skills(openFileName);
            Config.Update(x => x with { SkillNamesPath = openFileName });
            MessageBoxUtil.Success("载入成功");
        }

        public void LoadSenario()
        {
            if (!CheckAppData()) return;
            string? openFileName = FileDialogUtil.OpenFile("选择剧本文件", "s11剧本文件|*.s11", Config.Value.LastSenarioPath, "LoadSaveSenario");
            if (openFileName == null) return;
            List<SanGuoCharacter> characters = CharacterPK22ScenConverter.FromPK22Scenario(openFileName);
            foreach (SanGuoCharacter character in characters)
            {
                AddOrUpdateCharacter(character);
            }
            Config.Update(x => x with { LastSenarioPath = openFileName });
            MessageBoxUtil.Success("载入成功");
        }

        public void LoadMakeData()
        {
            if (!CheckAppData()) return;
            string? openFileName = FileDialogUtil.OpenFile("选择创作档文件", "s11创作档文件|MakeData.s11", Config.Value.LastMakeDataPath, "LoadMakeData");
            if (openFileName == null) return;
            List<SanGuoCharacter> characters = CharacterPK22ScenConverter.FromPK22MakeData(openFileName);
            foreach (SanGuoCharacter character in characters)
            {
                AddOrUpdateCharacter(character);
            }
            Config.Update(x => x with { LastMakeDataPath = openFileName });
            MessageBoxUtil.Success("载入成功");
        }

        public void LoadData()
        {
            if (!CheckAppData()) return;
            string? openFileName = FileDialogUtil.OpenFile("选择扩展武将数据文件", "扩展武将文件|*.json", Config.Value.LastJsonDataPath, "LoadSaveData");
            if (openFileName == null) return;
            try
            {
                List<SanGuoCharacter> characters = CharacterJsonConverter.FromJson(openFileName);
                int count = 0;
                foreach (SanGuoCharacter character in characters)
                {
                    AddOrUpdateCharacter(character);
                    count++;
                }
                Config.Update(x => x with { LastJsonDataPath = openFileName });
                MessageBoxUtil.Success($"成功载入 {count} 条数据");
            }
            catch (Exception ex)
            {
                MessageBoxUtil.Error($"文件格式错误！\n{ex.Message}");
            }
        }

        public void SaveData()
        {
            string? saveFileName = FileDialogUtil.SaveFile("选择扩展武将数据文件", "扩展武将文件|*.json", Config.Value.LastJsonDataPath, "LoadSaveData");
            if (saveFileName == null) return;
            try
            {
                CharacterJsonConverter.ToJson(saveFileName, GetAllCharacterModels());
                Config.Update(x => x with { LastJsonDataPath = saveFileName });
                MessageBoxUtil.Success($"成功保存 {GetAllCharacterModels().Count} 条数据");
            }
            catch (Exception ex)
            {
                MessageBoxUtil.Exception(ex);
            }
        }

        public void ImportExcel()
        {
            if (!CheckAppData()) return;
            string? openFileName = FileDialogUtil.OpenFile("选择导入的Excel文件", "Excel文件|*.xlsx", Config.Value.LastExcelDataPath, "ImportExcel");
            if (openFileName == null) return;
            try
            {
                IEnumerable<SanGuoCharacter> characters = CharacterExcelConverter.FromExcel(openFileName);
                int count = 0;
                foreach (SanGuoCharacter character in characters)
                {
                    AddOrUpdateCharacter(character);
                    count++;
                }
                Config.Update(x => x with { LastExcelDataPath = openFileName });
                MessageBoxUtil.Success($"成功导入 {count} 条数据");
            }
            catch (Exception ex)
            {
                MessageBoxUtil.Error($"文件格式错误！\n{ex.Message}");
            }
        }

        public void ExportExcel()
        {
            string? saveFileName = FileDialogUtil.SaveFile("选择导出的Excel文件", "Excel文件|*.xlsx", Config.Value.LastExcelDataPath, "ImportExcel");
            if (saveFileName == null) return;
            try
            {
                CharacterExcelConverter.ToExcel(saveFileName, GetAllCharacterModels());
                Config.Update(x => x with { LastExcelDataPath = saveFileName });
                MessageBoxUtil.Success($"成功导出 {GetAllCharacterModels().Count} 条数据");
            }
            catch (Exception ex)
            {
                MessageBoxUtil.Exception(ex);
            }
        }

        public async Task<bool> FuncEditCharacterAsync(SanGuoCharacterViewModel characterVM, bool isNew = false)
        {
            // 读取当前头像文件
            string faceDir = Path.Combine(Config.Value.MediaExPath, "ExtraCharacters", characterVM.Id);
            BitmapSource? largeFace = ImageFileHelper.LoadBitmapFile(Path.Combine(faceDir, $"{characterVM.Id}.bmp"));
            BitmapSource? smallFaceL = ImageFileHelper.LoadBitmapFile(Path.Combine(faceDir, $"{characterVM.Id}-SL.bmp"));
            BitmapSource? smallFaceR = ImageFileHelper.LoadBitmapFile(Path.Combine(faceDir, $"{characterVM.Id}-SR.bmp"));
            BitmapSource? cutin = ImageFileHelper.LoadBitmapFile(Path.Combine(faceDir, $"{characterVM.Id}-cutin.bmp"));
            Point midFacePos = characterVM.Data.MidFacePos;

            var editViewModel = new CharacterEditViewModel(
                new SanGuoCharacterViewModel(ModelMapper.Clone(characterVM.Data), this),
                new FaceImagesPanelViewModel(),
                isNew
            );
            editViewModel.FaceImagesPanelVM.SetFaceImages(largeFace, smallFaceL, smallFaceR, midFacePos);
            editViewModel.CutinImage = cutin;
            editViewModel.FaceImagesPanelVM.MidFacePosEditMode = true;

            bool? result = await EditInteraction.Handle(editViewModel);
            if (result == true)
            {
                AddOrUpdateCharacter(editViewModel.CharacterVM.Data);
                // 保存头像文件
                if (editViewModel.IsFaceImageModified)
                {
                    if (!Directory.Exists(faceDir))
                        Directory.CreateDirectory(faceDir);
                    if ((BitmapSource?)editViewModel.FaceImagesPanelVM.LargeFace != largeFace)
                        ImageFileHelper.SaveBitmapFile((BitmapSource?)editViewModel.FaceImagesPanelVM.LargeFace, Path.Combine(faceDir, $"{characterVM.Id}.bmp"));
                    if ((BitmapSource?)editViewModel.FaceImagesPanelVM.SmallFaceL != smallFaceL)
                        ImageFileHelper.SaveBitmapFile((BitmapSource?)editViewModel.FaceImagesPanelVM.SmallFaceL, Path.Combine(faceDir, $"{characterVM.Id}-SL.bmp"));
                    if ((BitmapSource?)editViewModel.FaceImagesPanelVM.SmallFaceR != smallFaceR)
                        ImageFileHelper.SaveBitmapFile((BitmapSource?)editViewModel.FaceImagesPanelVM.SmallFaceR, Path.Combine(faceDir, $"{characterVM.Id}-SR.bmp"));
                    if ((BitmapSource?)editViewModel.CutinImage != cutin)
                        ImageFileHelper.SaveBitmapFile((BitmapSource?)editViewModel.CutinImage, Path.Combine(faceDir, $"{characterVM.Id}-cutin.bmp"));
                    characterVM.Data.MidFacePos = editViewModel.FaceImagesPanelVM.MidFacePos;
                }
                return true;
            }
            return false;
        }

        private void FuncDeleteCharacters(IEnumerable<object> characterVMs)
        {
            if (!MessageBoxUtil.Ask($"确认删除 {characterVMs.Count()} 条数据？", "删除人物"))
                return;
            var list = characterVMs.Cast<SanGuoCharacterViewModel>().ToList();
            foreach (var characterVM in list)
            {
                RemoveCharacter(characterVM);
            }
        }

        private async Task FuncAddCharacterAsync()
        {
            SanGuoCharacterViewModel newCharacterVM = new(new(), this);
            await FuncEditCharacterAsync(newCharacterVM, true);
        }

        public int AddOrUpdateCharacter(SanGuoCharacter character)
        {
            if (_characterMap.TryGetValue(character.Id, out SanGuoCharacterViewModel? oldVM))
            {
                // 已存在则原地更新数据
                oldVM.Data = character;

                if (oldVM.Id != character.Id)  // Id改变
                {
                    _characterMap.Remove(oldVM.Id);
                    _characterMap[oldVM.Id] = oldVM;
                }
                return 0;
            }
            else
            {
                // 没有则新增
                SanGuoCharacterViewModel vm = new(character, this);
                Characters.Add(vm);
                _characterMap[vm.Id] = vm;
                return 1;
            }
        }

        public void RemoveCharacter(SanGuoCharacterViewModel vm)
        {
            Characters.Remove(vm);
            _characterMap.Remove(vm.Id);
        }

        public List<SanGuoCharacter> GetAllCharacterModels()
        {
            return Characters.Select(x => x.Data).ToList();
        }

        public SanGuoCharacterViewModel? GetCharacter(string id)
        {
            return _characterMap.TryGetValue(id, out var vm) ? vm : null;
        }

        public ObservableCollection<SanGuoCharacterViewModel> GetCharacterCollection()
        {
            return Characters;
        }
    }
}
