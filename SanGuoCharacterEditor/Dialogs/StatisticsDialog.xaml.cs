using LiveCharts;
using LiveCharts.Wpf;
using SanGuoCharacterEditor.Core.Models;
using System.Windows;

namespace SanGuoCharacterEditor.Dialogs
{
    /// <summary>
    /// StatisticsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticsDialog : Window
    {
        private IReadOnlyCollection<SanGuoCharacter> AllCharacterModels { get; init; }

        public StatisticsDialog(IReadOnlyCollection<SanGuoCharacter> allCharacters)
        {
            AllCharacterModels = allCharacters;
            InitializeComponent();
            StatSum();
        }

        public SeriesCollection SeriesCollection { get; set; } = new();
        public string[] XLabels { get; set; }  // X轴标签

        public void Init()
        {
            InitializeComponent();

            // 设置窗口的数据上下文
            DataContext = this;
        }

        public void StatSum()
        {
            // 初始化一个长度为20的数组用于存储各区间的计数
            int[] binCounts = new int[24];

            // 遍历数据，统计每个区间的样本量
            foreach (var character in AllCharacterModels)
            {
                var value = character.Stats.Leadership;
                int binIndex = value / 5;  // 根据数值计算属于哪个区间
                if (binIndex == 24) binIndex = 23; // 处理120的情况
                binCounts[binIndex]++;
            }

            // 创建X轴的标签，每个标签对应一个区间
            XLabels = Enumerable.Range(0, 24)
                                .Select(i => $"{i * 5}+")
                                .ToArray();

            // 创建柱状图数据
            SeriesCollection = new()
            {
                new ColumnSeries
                {
                    Title = "人数",
                    Values = new ChartValues<int>(binCounts)
                }
            };

            // 设置窗口的数据上下文
            DataContext = this;
        }
    
    }
}
