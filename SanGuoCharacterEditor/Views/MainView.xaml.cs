using HandyControl.Tools;
using ReactiveUI;
using SanGuoCharacterEditor.Dialogs;
using SanGuoCharacterEditor.ViewModels;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;

namespace SanGuoCharacterEditor.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : ReactiveUserControl<MainViewModel>
    {
        public MainView()
        {
            InitializeComponent();

            ViewModel = new();

            this.WhenActivated(DoWhenActivated);

            try
            {
                ViewModel.Init();
            }
            catch { }
        }

        private void DoWhenActivated(CompositeDisposable disposables)
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext)
                .DisposeWith(disposables);

            ViewModel.EditInteraction.RegisterHandler(ctx =>
            {
                // 这样打开对话框可以避免ReactiveUI的响应失效问题
                return Observable.Start(() =>
                {
                    var editViewModel = ctx.Input;
                    CharacterEditDialog dialog = new()
                    {
                        Owner = WindowHelper.GetActiveWindow(),
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        ViewModel = editViewModel
                    };
                    ctx.SetOutput(dialog.ShowDialog());
                },
                RxApp.MainThreadScheduler);
            });
        }

        private async void datatable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (datatable.SelectedItem is not SanGuoCharacterViewModel characterVM)
                return;

            await ViewModel.EditCommand.Execute(characterVM);
            //ViewModel.FuncEditCharacter(characterVM);
        }

        private void MenuSetMediaExPath_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetMediaExPath();
        }

        private void MenuLoadCodeTable_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadCodeTable();
        }

        private void MenuLoadGlobalScenario_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadGlobalScenario();
        }

        private void MenuLoadSkillNames_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadSkillNames();
        }

        private void MenuLoadSenario_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadSenario();
        }

        private void MenuLoadMakeData_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadMakeData();
        }

        private void MenuLoadData_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadData();
        }

        private void MenuSaveData_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveData();
        }

        private void MenuImportExcel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImportExcel();
        }

        private void MenuExportExcel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ExportExcel();
        }

        private void MenuStatistics_Click(object sender, RoutedEventArgs e)
        {
            StatisticsDialog statisticsDialog = new(ViewModel.GetAllCharacterModels())
            {
                Owner = WindowHelper.GetActiveWindow(),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            statisticsDialog.ShowDialog();
        }
    }
}
