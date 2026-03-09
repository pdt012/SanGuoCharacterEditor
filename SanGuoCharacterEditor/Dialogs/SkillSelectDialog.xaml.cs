using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SanGuoCharacterEditor.Dialogs
{
    /// <summary>
    /// SkillSelectDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SkillSelectDialog : Window
    {
        Label[] skilBoxes => new[] { SkillBox1, SkillBox2, SkillBox3 };

        public SkillSelectDialog(IEnumerable<string> skills)
        {
            InitializeComponent();
            DataContext = this;
            SkillListBox.IsSynchronizedWithCurrentItem = false;

            foreach (string skill in skills)
            {
                Choices.Add(skill);
            }
            collectionView = CollectionViewSource.GetDefaultView(Choices);
            collectionView.Filter = FilterItems;
        }

        private bool FilterItems(object item)
        {
            bool searchMatched;
            if (string.IsNullOrEmpty(searchBar.Text))
                searchMatched = true;
            else
            {
                string? str = item.ToString();
                searchMatched = str != null && str.Contains(searchBar.Text);
            }
            return searchMatched && !SelectedSkills.Contains(item);
        }

        public ObservableCollection<string> Choices { get; } = new();
        public ICollectionView collectionView { get; set; }

        private void SetSkill(int index, string skill)
        {
            skilBoxes[index].Content = skill;
            collectionView.Refresh();
        }

        public IList<string> SelectedSkills
        {
            get
            {
                List<string> skills = new();
                for (int i = 0; i < skilBoxes.Length; i++)
                {
                    skills.Add(skilBoxes[i].Content?.ToString() ?? "");
                }
                return skills;
            }
            set
            {
                for (int i = 0; i < skilBoxes.Length; i++)
                {
                    skilBoxes[i].Content = "";
                }
                for (int i = 0; i < Math.Min(value.Count, skilBoxes.Length); i++)
                {
                    skilBoxes[i].Content = value[i];
                }
                collectionView.Refresh();
            }
        }

        private Control? _currentDragSource = null;

        protected void OnCommit()
        {
            DialogResult = true;
        }

        protected void OnCancel()
        {
            DialogResult = false;
        }

        private void SkillBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void SkillBox_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(string))) return;
            if (e.Data.GetData(typeof(string)) is not string skill) return;
            if (sender is not Label skillBox) return;

            if (_currentDragSource is Label sourceSkillBox)
            {
                if (skillBox.Content is string || skillBox.Content is null)
                {
                    // 交换特技
                    sourceSkillBox.Content = skillBox.Content;
                    skillBox.Content = skill;
                }
            }
            else if (_currentDragSource == SkillListBox)  // 列表到特技槽
            {
                skillBox.Content = skill;
                collectionView.Refresh();
            }
            _currentDragSource = null;
        }

        private void SkillListBox_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(string))) return;
            if (e.Data.GetData(typeof(string)) is not string skill) return;

            if (_currentDragSource is Label sourceSkillBox)  // 特技槽到列表
            {
                sourceSkillBox.Content = "";
                collectionView.Refresh();
            }
            _currentDragSource = null;
        }

        private void SkillBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not Label skillBox) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (skillBox.Content is string skill)
                {
                    _currentDragSource = skillBox;
                    DragDrop.DoDragDrop(skillBox, skill, DragDropEffects.Move);
                }
            }
        }

        private void SkillListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (SkillListBox.SelectedItem is string skill)
                {
                    _currentDragSource = SkillListBox;
                    DragDrop.DoDragDrop(SkillListBox, skill, DragDropEffects.Move);
                }
            }
        }

        private void SkillListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SkillListBox.SelectedItem is not string skill) return;
            var currentSkills = SelectedSkills;
            for (int i = 0; i < currentSkills.Count; i++)
            {
                if (currentSkills[i] == "")
                {
                    SetSkill(i, skill);
                    return;
                }
            }
        }

        private void SearchBar_SearchStarted(object sender, HandyControl.Data.FunctionEventArgs<string> e)
        {
            collectionView.Refresh();
        }
    }
}
