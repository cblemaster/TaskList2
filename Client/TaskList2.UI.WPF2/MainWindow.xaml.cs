using System.Windows;
using System.Windows.Controls;
using TaskList2.Services.Models;

namespace TaskList2.UI.WPF2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region ctor
        public MainWindow()
        {
            InitializeComponent();
            // TODO: Get this into xaml as binding
            this._context = (this.DataContext as MainWindowViewModel)!;
            this.folderListView.lvFolderList.ItemsSource = this._context.Folders;
            this.folderListView.lvFolderList.SelectionChanged += this.lvFolderList_SelectionChanged;
            this.taskListView.lvTaskList.SelectedItem = null;
        }
        #endregion

        #region fields
        private readonly MainWindowViewModel _context = new();
        #endregion

        #region ui_events
        private void lvFolderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Get this into xaml as binding
            this.taskListView.lvTaskList.ItemsSource = (this.folderListView.lvFolderList.SelectedItem as Folder)!.Tasks;
            this.taskListView.lvTaskList.SelectionChanged += this.lvTaskList_SelectionChanged;
        }

        private void lvTaskList_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
            // TODO: Get this into xaml as binding
            this.taskDetailsView.DataContext = this.taskListView.lvTaskList.SelectedItem as Task;
        #endregion
    }
}
