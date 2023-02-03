using System.Windows;
using TaskList2.Services.Models;
using TaskList2.UI.WPF2.Controls;
using System.Windows.Controls;

namespace TaskList2.UI.WPF2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // TODO: Get this into xaml as binding
            this._context = (this.DataContext as MainWindowViewModel)!;
            this.folderListView.lvFolderList.ItemsSource = this._context.Folders;
            this.folderListView.lvFolderList.SelectionChanged += this.lvFolderList_SelectionChanged;
            this.taskListView.lvTaskList.SelectedItem = null;
        }

        private MainWindowViewModel _context = new();

        private void lvFolderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Get this into xaml as binding
            this.taskListView.lvTaskList.ItemsSource = (this.folderListView.lvFolderList.SelectedItem as Folder)!.Tasks;
            this.taskListView.lvTaskList.SelectionChanged += this.lvTaskList_SelectionChanged;
        }

        private void lvTaskList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Get this into xaml as binding
            this.taskDetailsView.DataContext = this.taskListView.lvTaskList.SelectedItem as Task;
        }
        
    }
}
