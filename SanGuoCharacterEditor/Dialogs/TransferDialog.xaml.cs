using System.Collections;
using System.Windows;

namespace SanGuoCharacterEditor.Dialogs
{
    /// <summary>
    /// TransferDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TransferDialog : Window
    {
        public TransferDialog()
        {
            InitializeComponent();
        }

        public int MaxSelections { set => transfer.MaxSelections = value; get => transfer.MaxSelections; }

        public bool SortTransferredItems { set => transfer.SortTransferredItems = value; get => transfer.SortTransferredItems; }

        public IList ItemsSource { set => transfer.ItemsSource = value; get => transfer.ItemsSource; }

        public IEnumerable TransferredItems { set => transfer.TransferredItems = value; get => transfer.TransferredItems; }
    }
}
