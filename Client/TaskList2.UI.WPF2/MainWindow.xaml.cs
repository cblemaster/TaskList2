// Business rules:
// 1. Completed tasks cannot be edited
// 2. 

using System.Windows;
using System.Windows.Controls;
using TaskList2.Services.Models;
using TaskList2.UI.WPF2.Controls;

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
            this.taskListView.lvTaskList.SelectionChanged += this.lvTaskList_SelectionChanged;

            this.btnAddFolder.Click += this.btnAddFolder_Click;
            this.addFolderView.btnAdd.Click += this.btnAdd_Click;
            this.addFolderView.btnCancel.Click += this.btnCancel_Click;
        }        
        #endregion

        #region fields
        private readonly MainWindowViewModel _context = new();
        #endregion

        #region ui_events
        // TODO: Get this into xaml as binding
        private void lvFolderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.taskListView.lvTaskList.ItemsSource = (this.folderListView.lvFolderList.SelectedItem as Folder)!.Tasks;
            this.taskListView.lvTaskList.SelectedItem = null;
        }

        private void lvTaskList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // first need to re-enable the task detail view controls
            //  since the controls might have been previously 
            //  disabled for displaying a completed task
            this.EnableTaskDetailViewControls();
            
            // TODO: Get this into xaml as binding
            Task? t = this.taskListView.lvTaskList.SelectedItem as Task;
            //if (t != null)
            //{
                this.taskDetailsView.DataContext = t;
                if (t != null && t.IsComplete)
                    this.DisableTaskDetailViewControls();
            //}
        }

        private void btnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            this.grdMain.Visibility = Visibility.Collapsed;
            this.grdAddFolderView.Visibility = Visibility.Visible;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.grdMain.Visibility = Visibility.Visible;
            this.grdAddFolderView.Visibility = Visibility.Collapsed;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.grdMain.Visibility = Visibility.Visible;
            this.grdAddFolderView.Visibility = Visibility.Collapsed;
        }
        
        
        
        
        #endregion

        #region methods
        private void EnableTaskDetailViewControls()
        {
            TaskDetailsView tlv = this.taskDetailsView;
            tlv.tbTaskName.IsEnabled = true;
            tlv.dpDueDate.IsEnabled = true;
            tlv.cboRecurrence.IsEnabled = true;
            tlv.cbIsImportant.IsEnabled = true;
            tlv.cbIsComplete.IsEnabled = true;
            tlv.tbNote.IsEnabled = true;
        }

        private void DisableTaskDetailViewControls()
        {
            TaskDetailsView tlv = this.taskDetailsView;
            tlv.tbTaskName.IsEnabled = false;
            tlv.dpDueDate.IsEnabled = false;
            tlv.cboRecurrence.IsEnabled = false;
            tlv.cbIsImportant.IsEnabled = false;
            tlv.cbIsComplete.IsEnabled = false;
            tlv.tbNote.IsEnabled = false;
        }
        #endregion
    }
}
