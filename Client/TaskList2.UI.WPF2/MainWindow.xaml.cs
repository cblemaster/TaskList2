// Business rules:
// 1. Completed tasks cannot be edited - OK
// 2. These folders cannot be renamed: Planned, Completed, Recurring, Important, Tasks - OK
// 3. These folders cannot be deleted: Planned, Completed, Recurring, Important, Tasks - OK
// 4. Tasks cannot be added directly to these folders: Planned, Completed, Recurring, Important - OK
// 5. Summary of the Planned, Completed, Recurring, Important, Tasks folder contents:
//    5.a. Planned contains all tasks that have a DueDate
//    5.b. Completed contains all tasks where IsComplete = true
//    5.c. Recurring contains all tasks where Recurrence != None
//    5.d. Important contains all tasks where IsImportant = true
//    5.e. Tasks is the default folder and can have Tasks added to it

using System.Collections.Generic;
using System.Linq;
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
            this.addFolderView.btnAdd.Click += this.btnAddFolderConf_Click;
            this.addFolderView.btnCancel.Click += this.btnCancelAddFolder_Click;

            this.btnAddTask.Click += this.btnAddTask_Click;
            this.btnSaveChangesAddTask.Click += this.btnSaveChangesAddTask_Click;
            this.btnCancelChangesAddTask.Click += this.btnCancelChangesAddTask_Click;
        }     
        #endregion

        #region fields
        private readonly MainWindowViewModel _context = new();
        #endregion

        #region ui_events
        // TODO: Get this into xaml as binding
        private void lvFolderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //unshow rename and delete buttons in folderlistview
            //they may still be visibile from the last selected item
            if (e.RemovedItems.Count == 1 && e.RemovedItems[0] is Folder)
            {
                Folder previouslySelectedFolder = (e.RemovedItems[0] as Folder)!;
                if (previouslySelectedFolder != null)
                    UnconfigureFolderRenameAndDeleteButtons(previouslySelectedFolder);
            }

            Folder selectedFolder = (this.folderListView.lvFolderList.SelectedItem as Folder)!;
            this.taskListView.lvTaskList.ItemsSource = selectedFolder.Tasks;
            this.taskListView.lvTaskList.SelectedItem = null;
            this.btnDeleteTask.Visibility = Visibility.Collapsed;

            //show folder rename and delete buttons in folderlistview for selected item
            ConfigureFolderRenameAndDeleteButtons(selectedFolder);
            
            //show Add Task button only if appropriate for selected folder
            //cannot add tasks directly to Planned, Recurring, Important, or Completed folders
            switch (selectedFolder.FolderName)
            {
                case "Planned":
                case "Recurring":
                case "Important":
                case "Completed":
                    this.btnAddTask.Visibility = Visibility.Collapsed;
                    break;
                default:
                    this.btnAddTask.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void lvTaskList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // first need to re-enable the task detail view controls
            //  since the controls might have been previously 
            //  disabled for displaying a completed task
            this.EnableOrDisableTaskDetailViewControls(isEnabling: true);

            // TODO: Get this into xaml as binding
            Task? t = this.taskListView.lvTaskList.SelectedItem as Task;
            this.taskDetailsView.DataContext = t;
            if (t != null)
            {
                if (t.IsComplete)
                    this.EnableOrDisableTaskDetailViewControls(isEnabling: false);
                //show the task delete button
                this.btnDeleteTask.Visibility = Visibility.Visible;
            }                
        }

        private void btnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            this.grdMain.Visibility = Visibility.Collapsed;
            this.grdAddFolderView.Visibility = Visibility.Visible;
        }

        private void btnAddFolderConf_Click(object sender, RoutedEventArgs e)
        {
            this.grdMain.Visibility = Visibility.Visible;
            this.grdAddFolderView.Visibility = Visibility.Collapsed;
        }

        private void btnCancelAddFolder_Click(object sender, RoutedEventArgs e)
        {
            this.grdMain.Visibility = Visibility.Visible;
            this.grdAddFolderView.Visibility = Visibility.Collapsed;
        }

        private void btnRenameFolder_Click(object sender, RoutedEventArgs e)
        {
            Folder selectedFolder = (this.folderListView.lvFolderList.SelectedItem as Folder)!;
            ConfigureControlsWhileRenamingFolder(selectedFolder);
        }        
                
        private void btnRenameFolderConf_Click(object sender, RoutedEventArgs e)
        {
            //TODO: These next two (2) lines are repeated in btnCancelRenameFolder_Click()
            Folder selectedFolder = (this.folderListView.lvFolderList.SelectedItem as Folder)!;
            ConfigureControlsAfterRenamingFolder(selectedFolder);
        }

        private void btnCancelRenameFolder_Click(object sender, RoutedEventArgs e)
        {
            //TODO: These next two (2) lines are repeated in btnRenameFolderConf_Click()
            Folder selectedFolder = (this.folderListView.lvFolderList.SelectedItem as Folder)!;
            ConfigureControlsAfterRenamingFolder(selectedFolder);
        }

        private void btnDeleteFolder_Click(object sender, RoutedEventArgs e) => throw new System.NotImplementedException();

        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            //unshow controls that are unrelated to adding a task
            ConfigureControlsWhileAddingTask();
            
            //set datacontext for taskdetailsview to new Task that will be added
            this.taskDetailsView.DataContext = new Task();
        }
        
        private void btnSaveChangesAddTask_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Repeated code, same as found in btnCancelChangesAddTask_Click()
            ConfigureControlsAfterAddingTask();
            this.taskDetailsView.DataContext = this.taskListView.lvTaskList.SelectedItem as Task;
        }

        private void btnCancelChangesAddTask_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Repeated code, same as found in btnSaveChangesAddTask_Click()
            ConfigureControlsAfterAddingTask();
            this.taskDetailsView.DataContext = this.taskListView.lvTaskList.SelectedItem as Task;
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

        private void ConfigureFolderRenameAndDeleteButtons(Folder folder)
        {
            DependencyObject container = this.folderListView.lvFolderList.ItemContainerGenerator.ContainerFromItem(folder);
            Button btnRenameFolder = this.GetButtonFromContainerByName(container, "btnRenameFolder");
            Button btnDeleteFolder = this.GetButtonFromContainerByName(container, "btnDeleteFolder");

            SetVisibilityForListOfControls(new List<ContentControl>() { btnRenameFolder, btnDeleteFolder }, Visibility.Visible);

            //add click event handlers from folder rename and delete buttons
            btnRenameFolder.Click += this.btnRenameFolder_Click;
            btnDeleteFolder.Click += this.btnDeleteFolder_Click;
        }

        private void UnconfigureFolderRenameAndDeleteButtons(Folder folder)
        {
            DependencyObject container = this.folderListView.lvFolderList.ItemContainerGenerator.ContainerFromItem(folder);
            Button btnRenameFolder = this.GetButtonFromContainerByName(container, "btnRenameFolder");
            Button btnDeleteFolder = this.GetButtonFromContainerByName(container, "btnDeleteFolder");

            SetVisibilityForListOfControls(new List<ContentControl>() { btnRenameFolder, btnDeleteFolder }, Visibility.Collapsed);

            //remove click event handlers from folder rename and delete buttons
            btnRenameFolder.Click -= this.btnRenameFolder_Click;
            btnDeleteFolder.Click -= this.btnDeleteFolder_Click;
        }

        private void ConfigureControlsWhileRenamingFolder(Folder folder)
        {
            DependencyObject container = null!;
            Button btnRenameFolderConf;
            Button btnCancelRenameFolder;

            List<ContentControl> buttonsToShow = new();
            List<ContentControl> buttonsToUnShow = new();

            //show folder rename confirm and cancel buttons in folderlistview
            container = this.folderListView.lvFolderList.ItemContainerGenerator.ContainerFromItem(folder);
            btnRenameFolderConf = this.GetButtonFromContainerByName(container, "btnRenameFolderConf");
            btnCancelRenameFolder = this.GetButtonFromContainerByName(container, "btnCancel");

            buttonsToShow.AddRange(new List<ContentControl> { btnRenameFolderConf, btnCancelRenameFolder });

            //set click event handlers for folder rename conf and cancel buttons in folderlistview
            btnRenameFolderConf.Click += this.btnRenameFolderConf_Click;
            btnCancelRenameFolder.Click += this.btnCancelRenameFolder_Click;

            //hide other controls unrelated to renaming a folder
            Button btnRenameFolder = this.GetButtonFromContainerByName(container, "btnRenameFolder");
            Button btnDeleteFolder = this.GetButtonFromContainerByName(container, "btnDeleteFolder");

            buttonsToUnShow.AddRange(new List<ContentControl>() { btnRenameFolder, btnDeleteFolder, this.btnAddFolder, this.btnAddTask, this.btnSaveChangesAddTask, this.btnCancelChangesAddTask, this.btnSaveChangesEditTask, this.btnCancelChangesEditTask, this.btnDeleteTask }); //TODO: May not need all of these task action or the add folder/task buttons hidden, depends on their visibility rules which are TBD

            this.taskListView.IsEnabled = false;
            this.taskDetailsView.IsEnabled = false;

            SetVisibilityForListOfControls(buttonsToShow, Visibility.Visible);
            SetVisibilityForListOfControls(buttonsToUnShow, Visibility.Collapsed);
        }

        private void ConfigureControlsAfterRenamingFolder(Folder folder)
        {
            DependencyObject container = null!;
            Button btnRenameFolderConf;
            Button btnCancelRenameFolder;

            List<ContentControl> buttonsToShow = new();
            List<ContentControl> buttonsToUnShow = new();

            //unshow folder rename confirm and cancel buttons in folderlistview
            container = this.folderListView.lvFolderList.ItemContainerGenerator.ContainerFromItem(folder);
            btnRenameFolderConf = this.GetButtonFromContainerByName(container, "btnRenameFolderConf");
            btnCancelRenameFolder = this.GetButtonFromContainerByName(container, "btnCancel");

            buttonsToUnShow.AddRange(new List<ContentControl> { btnRenameFolderConf, btnCancelRenameFolder });

            //remove click event handlers for folder rename conf and cancel buttons in folderlistview
            btnRenameFolderConf.Click -= this.btnRenameFolderConf_Click;
            btnCancelRenameFolder.Click -= this.btnCancelRenameFolder_Click;

            //show other controls that were hidden as unrelated to renaming a folder
            Button btnRenameFolder = this.GetButtonFromContainerByName(container, "btnRenameFolder");
            Button btnDeleteFolder = this.GetButtonFromContainerByName(container, "btnDeleteFolder");

            buttonsToShow.AddRange(new List<ContentControl>() { btnRenameFolder, btnDeleteFolder, btnAddFolder });
            if (this.taskListView.lvTaskList.SelectedItem is Task && (this.taskListView.lvTaskList.SelectedItem as Task)!.Id > 0)
                buttonsToShow.Add(btnDeleteTask);
            //show Add Task button only if appropriate for selected folder
            //cannot add tasks directly to Planned, Recurring, Important, or Completed folders
            switch (folder.FolderName)
            {
                case "Planned":
                case "Recurring":
                case "Important":
                case "Completed":
                    buttonsToUnShow.Add(this.btnAddTask);
                    break;
                default:
                    buttonsToShow.Add(this.btnAddTask);
                    break;
            }

            this.taskListView.IsEnabled = true;
            this.taskDetailsView.IsEnabled = true;

            SetVisibilityForListOfControls(buttonsToShow, Visibility.Visible);
            SetVisibilityForListOfControls(buttonsToUnShow, Visibility.Collapsed);
        }

        private void ConfigureControlsWhileAddingTask()
        {
            SetVisibilityForListOfControls(new List<ContentControl>() { this.btnAddFolder, this.btnAddTask, this.btnDeleteTask }, Visibility.Collapsed);
            SetVisibilityForListOfControls(new List<ContentControl>() { this.btnSaveChangesAddTask, this.btnCancelChangesAddTask }, Visibility.Visible);

            this.folderListView.IsEnabled = false;
            this.taskListView.IsEnabled = false;
        }

        private void ConfigureControlsAfterAddingTask()
        {
            SetVisibilityForListOfControls(new List<ContentControl>() { this.btnAddFolder }, Visibility.Visible);
            //show Add Task button only if appropriate for selected folder
            //cannot add tasks directly to Planned, Recurring, Important, or Completed folders
            switch ((this.taskListView.lvTaskList.SelectedItem as Folder)!.FolderName) //TODO: CanHaveTasksAdded should be a property of folder that is defined in the db
            {
                case "Planned":
                case "Recurring":
                case "Important":
                case "Completed":
                    SetVisibilityForListOfControls(new List<ContentControl>() { this.btnAddTask }, Visibility.Collapsed);
                    break;
                default:
                    SetVisibilityForListOfControls(new List<ContentControl>() { this.btnAddTask }, Visibility.Visible);
                    break;
            }
            SetVisibilityForListOfControls(new List<ContentControl>() { this.btnSaveChangesAddTask, this.btnCancelChangesAddTask }, Visibility.Collapsed);

            this.folderListView.IsEnabled = true;
            this.taskListView.IsEnabled = true;

            if (this.taskListView.lvTaskList.SelectedItem is Task && (this.taskListView.lvTaskList.SelectedItem as Task)!.Id > 0)
            {
                SetVisibilityForListOfControls(new List<ContentControl>() { btnDeleteTask }, Visibility.Visible);
            }
        }

        private Button GetButtonFromContainerByName(DependencyObject container, string buttonName)
            => (AllChildren(container).FirstOrDefault(c => c is Button && c.Name == buttonName) as Button)!;

        //https://dzone.com/articles/how-access-named-control
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

        private static void SetVisibilityForListOfControls(List<ContentControl> controls, Visibility visibility)
        {
            foreach (ContentControl control in controls)
                control.Visibility = visibility;
        }
        #endregion
    }
}
