// Business rules:
// 1. Completed tasks cannot be edited
// 2. 

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            DependencyObject container = null!;

            //hide rename and delete buttons in folderlistview
            //they may still be visibile from the last selected item
            if (e.RemovedItems.Count == 1 && e.RemovedItems[0] is Folder)
            {
                container = this.folderListView.lvFolderList.ItemContainerGenerator.ContainerFromItem(e.RemovedItems[0] as Folder);
                this.SetVisibilityForButtonInContainer(container, "btnRenameFolder", Visibility.Collapsed);
                this.SetVisibilityForButtonInContainer(container, "btnDeleteFolder", Visibility.Collapsed);
            }

            Folder selectedFolder = (this.folderListView.lvFolderList.SelectedItem as Folder)!;
            this.taskListView.lvTaskList.ItemsSource = selectedFolder.Tasks;
            this.taskListView.lvTaskList.SelectedItem = null;

            //show rename and delete buttons in folderlistview
            container = this.folderListView.lvFolderList.ItemContainerGenerator.ContainerFromItem(selectedFolder);
            if (selectedFolder.IsRenameable)
                this.SetVisibilityForButtonInContainer(container, "btnRenameFolder", Visibility.Visible);
            if (selectedFolder.IsDeleteable)
                this.SetVisibilityForButtonInContainer(container, "btnDeleteFolder", Visibility.Visible);
        }

        private void lvTaskList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // first need to re-enable the task detail view controls
            //  since the controls might have been previously 
            //  disabled for displaying a completed task
            this.EnableOrDisableTaskDetailViewControls(isEnabling: true);
            
            // TODO: Get this into xaml as binding
            Task? t = this.taskListView.lvTaskList.SelectedItem as Task;
            //if (t != null)
            //{
                this.taskDetailsView.DataContext = t;
                if (t != null && t.IsComplete)
                    this.EnableOrDisableTaskDetailViewControls(isEnabling: false);
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
        private void EnableOrDisableTaskDetailViewControls(bool isEnabling = true)
        {
            TaskDetailsView tlv = this.taskDetailsView;
            if (isEnabling)
            {
                
                tlv.tbTaskName.IsEnabled = true;
                tlv.dpDueDate.IsEnabled = true;
                tlv.cboRecurrence.IsEnabled = true;
                tlv.cbIsImportant.IsEnabled = true;
                tlv.cbIsComplete.IsEnabled = true;
                tlv.tbNote.IsEnabled = true;
                return;
            }
            tlv.tbTaskName.IsEnabled = false;
            tlv.dpDueDate.IsEnabled = false;
            tlv.cboRecurrence.IsEnabled = false;
            tlv.cbIsImportant.IsEnabled = false;
            tlv.cbIsComplete.IsEnabled = false;
            tlv.tbNote.IsEnabled = false;
        }

        private void SetVisibilityForButtonInContainer(DependencyObject container, string buttonName, Visibility visibility)
        {
            List<Control> children = AllChildren(container);
            foreach (Control child in children)
            {
                if (child is Button && child.Name == buttonName)
                {
                    child.Visibility = visibility;
                }
            }
        }
        
        private List<Control> AllChildren(DependencyObject parent)
        {
            List<Control> controlList = new();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is Control)
                {
                    controlList.Add((child as Control)!);
                }
                controlList.AddRange(AllChildren(child));
            }
            return controlList;
        }
        #endregion
    }
}
