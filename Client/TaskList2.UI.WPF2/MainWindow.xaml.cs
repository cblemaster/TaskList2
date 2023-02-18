#region business rules
// Business rules:
// 1. Completed tasks cannot be edited - OK
// 2. These folders cannot be renamed: Planned, Completed, Recurring, Important, Tasks - OK
// 3. These folders cannot be deleted: Planned, Completed, Recurring, Important, Tasks - OK
// 4. Tasks cannot be added directly to these folders: Planned, Completed, Recurring, Important - OK, but needs to be in the db
#endregion

#region system folder summaries
// Summary of the Planned, Completed, Recurring, Important, Tasks folder contents:   TODO: These could be tooltips for the folder name textbox?
// 1. Planned contains all tasks that have a DueDate
// 2. Completed contains all tasks where IsComplete = true
// 3. Recurring contains all tasks where Recurrence != None
// 4. Important contains all tasks where IsImportant = true
// 5. Tasks is the default folder and can have Tasks added to it
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TaskList2.Services;
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

            this.folderListView.lvFolderList.ItemsSource = this._context.Folders;   // TODO: Get this into xaml as binding

            this.folderListView.lvFolderList.SelectionChanged += this.lvFolderList_SelectionChanged;
            this.taskListView.lvTaskList.SelectionChanged += this.lvTaskList_SelectionChanged;

            this.btnAddFolder.Click += this.btnAddFolder_Click;
            this.addFolderView.btnAdd.Click += this.btnAddFolderConf_Click;
            this.addFolderView.btnCancel.Click += this.btnCancelAddFolder_Click;

            //TODO: Why are these needed? Shouldn't the event handlers be added
            // when the buttons become visibile, and the event handlers be removed
            // when the buttons become collapsed?
            this.btnAddTask.Click += this.btnAddTask_Click;
            this.btnSaveChangesAddTask.Click += this.btnSaveChangesAddTask_Click;
            this.btnCancelChangesAddTask.Click += this.btnCancelChangesAddTask_Click;
        }
        #endregion

        #region fields
        private readonly MainWindowViewModel _context = new();
        #endregion

        #region ui_events
        private void lvFolderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.taskDetailsView.IsEnabled = false;
            
            //unshow rename and delete buttons in folderlistview
            //they may still be visible from the last selected item
            if (e.RemovedItems.Count == 1 && e.RemovedItems[0] is Folder previouslySelectedFolder)
            {
                if (this.folderListView.lvFolderList.Items.Contains(previouslySelectedFolder))
                {
                    UnconfigureFolderRenameAndDeleteButtons(previouslySelectedFolder);
                }                
            }

            Folder selectedFolder = GetListViewSelectedItemAsT<Folder>(this.folderListView.lvFolderList);

            if (selectedFolder == null)
            {
                this.taskListView.lvTaskList.ItemsSource = null;
            }
            else
            {
                this.taskListView.lvTaskList.ItemsSource = selectedFolder.Tasks;    // TODO: Get this into xaml as binding
                this.taskListView.lvTaskList.SelectedItem = null;

                //show folder rename and delete buttons in folderlistview for selected item
                ConfigureFolderRenameAndDeleteButtons(selectedFolder);

                //show Add Task button only if appropriate for selected folder
                //cannot add tasks directly to Planned, Recurring, Important, or Completed folders
                this.btnAddTask.Visibility = selectedFolder.FolderName switch
                {
                    "Planned" or "Recurring" or "Important" or "Completed" => Visibility.Collapsed,
                    _ => Visibility.Visible,
                };
            }            

            this.btnDeleteTask.Visibility = Visibility.Collapsed;            
        }

        private void lvTaskList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // first need to re-enable the task detail view controls
            //  since the controls might have been previously 
            //  disabled for displaying a completed task
            this.EnableOrDisableTaskDetailViewControls(isEnabling: true);
            RemoveEventHandlersFromTaskDetailsViewControls();

            this.btnSaveChangesEditTask.Click -= this.btnSaveChangesEditTask_Click;
            this.btnCancelChangesEditTask.Click -= this.btnCancelChangesEditTask_Click;

            // TODO: Get this into xaml as binding
            Task? t = GetListViewSelectedItemAsT<Task>(this.taskListView.lvTaskList);
            this.taskDetailsView.DataContext = t;
            if (t != null)
            {
                if (t.Id > 0)
                {
                    this.taskDetailsView.IsEnabled = true;
                    //show the task delete button
                    this.btnDeleteTask.Visibility = Visibility.Visible;
                    AddEventHandlersToTaskDetailsViewControls();

                    this.btnSaveChangesEditTask.Click += this.btnSaveChangesEditTask_Click;
                    this.btnCancelChangesEditTask.Click += this.btnCancelChangesEditTask_Click;
                }
                else   //TODO: Is this gonna cause problems when I try to add tasks (e.g., new task with Id = 0) ??
                    this.taskDetailsView.IsEnabled = false;
                
                if (this.taskDetailsView.IsEnabled && t.IsComplete)
                    this.EnableOrDisableTaskDetailViewControls(isEnabling: false);
            }
        }

        private void btnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            this.grdMain.Visibility = Visibility.Collapsed;
            this.grdAddFolderView.Visibility = Visibility.Visible;

            this.addFolderView.DataContext = new Folder();
        }

        private void btnAddFolderConf_Click(object sender, RoutedEventArgs e)
        {
            this.addFolderView.lblFolderNameErrors.Visibility = Visibility.Collapsed;
            
            if (this.addFolderView.DataContext is Folder folderToAdd)
            {
                if (folderToAdd.IsValid())
                {
                    FolderService fs = new();
                    folderToAdd = fs.AddFolder(folderToAdd);

                    if (folderToAdd.Id > 0)
                    {
                        //this.folderListView.lvFolderList.Items.Add(folderToAdd);
                        this._context.Folders.Add(folderToAdd);
                        this.folderListView.lvFolderList.Items.Refresh();
                        this.folderListView.lvFolderList.SelectedItem = folderToAdd;
                    }
                    else
                    {
                        //TODO: Handle error
                    }
                }
                else
                {
                    this.addFolderView.lblFolderNameErrors.Visibility = Visibility.Visible;
                    this.addFolderView.lblFolderNameErrors.ToolTip = folderToAdd.GetValidationErrorsAsString();
                }
            }
            
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
            Folder selectedFolder = GetListViewSelectedItemAsT<Folder>(this.folderListView.lvFolderList);
            ConfigureControlsWhileRenamingFolder(selectedFolder);

            DependencyObject container = this.folderListView.lvFolderList.ItemContainerGenerator.ContainerFromItem(selectedFolder);
            TextBox tb = GetTextBoxFromContainerByName(container, "tbFolderName");
            tb.IsEnabled = true;
        }

        private void btnRenameFolderConf_Click(object sender, RoutedEventArgs e)
        {
            this.addFolderView.lblFolderNameErrors.Visibility = Visibility.Collapsed;

            Folder folderToRename = GetListViewSelectedItemAsT<Folder>(this.folderListView.lvFolderList);
            if (folderToRename.IsValid())
            {
                FolderService fs = new();
                fs.UpdateFolder(folderToRename);

                ConfigureControlsAfterRenamingFolder(folderToRename);

                DependencyObject container = this.folderListView.lvFolderList.ItemContainerGenerator.ContainerFromItem(folderToRename);
                TextBox tb = GetTextBoxFromContainerByName(container, "tbFolderName");
                tb.IsEnabled = false;
            }
            else
            {
                this.addFolderView.lblFolderNameErrors.Visibility = Visibility.Visible;
                this.addFolderView.lblFolderNameErrors.ToolTip = folderToRename.GetValidationErrorsAsString();
            }
        }

        private void btnCancelRenameFolder_Click(object sender, RoutedEventArgs e)
        {
            this.addFolderView.lblFolderNameErrors.Visibility = Visibility.Collapsed;

            Folder selectedFolder = GetListViewSelectedItemAsT<Folder>(this.folderListView.lvFolderList);
            ConfigureControlsAfterRenamingFolder(selectedFolder);

            DependencyObject container = this.folderListView.lvFolderList.ItemContainerGenerator.ContainerFromItem(selectedFolder);
            TextBox tb = GetTextBoxFromContainerByName(container, "tbFolderName");
            tb.IsEnabled = false;
        }

        private void btnDeleteFolder_Click(object sender, RoutedEventArgs e)
        {
            string message = "Delete this folder and everything in it?";
            string title = "Confirm Folder Delete";

            MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                FolderService fs = new();
                Folder f = GetListViewSelectedItemAsT<Folder>(this.folderListView.lvFolderList);
                if (f != null)
                {
                    if (fs.DeleteFolder(f.Id))
                    {
                        this._context.Folders.Remove(f);
                        this.folderListView.lvFolderList.SelectedItem = null;
                    }
                    else
                    {
                        //TODO: Error handling
                    }
                } 
            }
        }

        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            //unshow controls that are unrelated to adding a task
            ConfigureControlsWhileAddingTask();

            this.RemoveEventHandlersFromTaskDetailsViewControls();

            //set datacontext for taskdetailsview to new Task that will be added
            this.taskDetailsView.DataContext = new Task();

            this.AddEventHandlersToTaskDetailsViewControls();            
        }

        private void btnSaveChangesAddTask_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Repeated code, same as found in btnCancelChangesAddTask_Click()
            ConfigureControlsAfterAddingTask();
            this.RemoveEventHandlersFromTaskDetailsViewControls();
            this.taskDetailsView.DataContext = GetListViewSelectedItemAsT<Task>(this.taskListView.lvTaskList);
            this.AddEventHandlersToTaskDetailsViewControls();
        }

        private void btnCancelChangesAddTask_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Repeated code, same as found in btnSaveChangesAddTask_Click()
            ConfigureControlsAfterAddingTask();
            this.RemoveEventHandlersFromTaskDetailsViewControls();
            this.taskDetailsView.DataContext = GetListViewSelectedItemAsT<Task>(this.taskListView.lvTaskList);
            this.AddEventHandlersToTaskDetailsViewControls();
        }

        private void tbTaskName_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ConfigureControlsWhileEditingTask();
            this.RemoveEventHandlersFromTaskDetailsViewControls();
        } 

        private void dpDueDate_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            this.ConfigureControlsWhileEditingTask();
            this.RemoveEventHandlersFromTaskDetailsViewControls();
        }

        private void cboRecurrence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ConfigureControlsWhileEditingTask();
            this.RemoveEventHandlersFromTaskDetailsViewControls();
        }

        private void cbIsImportant_CheckedChanged(object sender, RoutedEventArgs e)
        {
            this.ConfigureControlsWhileEditingTask();
            this.RemoveEventHandlersFromTaskDetailsViewControls();
        }

        private void cbIsComplete_CheckedChanged(object sender, RoutedEventArgs e)
        {
            this.ConfigureControlsWhileEditingTask();
            this.RemoveEventHandlersFromTaskDetailsViewControls();
        }

        private void tbNote_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ConfigureControlsWhileEditingTask();
            this.RemoveEventHandlersFromTaskDetailsViewControls();
        }

        private void btnSaveChangesEditTask_Click(object sender, RoutedEventArgs e)
        {
            this.ConfigureControlsAfterEditingTask();
            this.AddEventHandlersToTaskDetailsViewControls();
        }

        private void btnCancelChangesEditTask_Click(object sender, RoutedEventArgs e)
        {
            this.ConfigureControlsAfterEditingTask();
            this.AddEventHandlersToTaskDetailsViewControls();
        }
        #endregion

        #region methods
        private void RemoveEventHandlersFromTaskDetailsViewControls()
        {
            this.taskDetailsView.tbTaskName.TextChanged -= this.tbTaskName_TextChanged;
            this.taskDetailsView.dpDueDate.SelectedDateChanged -= this.dpDueDate_SelectedDateChanged;
            this.taskDetailsView.cboRecurrence.SelectionChanged -= this.cboRecurrence_SelectionChanged;
            this.taskDetailsView.cbIsImportant.Checked -= this.cbIsImportant_CheckedChanged;
            this.taskDetailsView.cbIsImportant.Unchecked -= this.cbIsImportant_CheckedChanged;
            this.taskDetailsView.cbIsComplete.Checked -= this.cbIsComplete_CheckedChanged;
            this.taskDetailsView.cbIsComplete.Unchecked -= this.cbIsComplete_CheckedChanged;
            this.taskDetailsView.tbNote.TextChanged -= this.tbNote_TextChanged;
        }

        private void AddEventHandlersToTaskDetailsViewControls()
        {
            if (this.taskDetailsView.DataContext is Task t && t.Id > 0)
            {
                this.taskDetailsView.tbTaskName.TextChanged += this.tbTaskName_TextChanged;
                this.taskDetailsView.dpDueDate.SelectedDateChanged += this.dpDueDate_SelectedDateChanged;
                this.taskDetailsView.cboRecurrence.SelectionChanged += this.cboRecurrence_SelectionChanged;
                this.taskDetailsView.cbIsImportant.Checked += this.cbIsImportant_CheckedChanged;
                this.taskDetailsView.cbIsImportant.Unchecked += this.cbIsImportant_CheckedChanged;
                this.taskDetailsView.cbIsComplete.Checked += this.cbIsComplete_CheckedChanged;
                this.taskDetailsView.cbIsComplete.Unchecked += this.cbIsComplete_CheckedChanged;
                this.taskDetailsView.tbNote.TextChanged += this.tbNote_TextChanged;
            }            
        }

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
            if (container != null)  //TODO: This gets around a problem when, after adding a folder and passing it down to this method, the container generated by the line above is null; as such after adding a folder it is selected in the folder list but it doesn't have rename and delete buttons
            {
                Button btnRenameFolder = this.GetButtonFromContainerByName(container, "btnRenameFolder");
                Button btnDeleteFolder = this.GetButtonFromContainerByName(container, "btnDeleteFolder");

                if (folder.IsRenameable)
                {
                    SetVisibilityForListOfControls(new List<ContentControl>() { btnRenameFolder }, Visibility.Visible);
                    //add click event handler for folder rename button
                    btnRenameFolder.Click += this.btnRenameFolder_Click;
                }
                if (folder.IsDeleteable)
                {
                    SetVisibilityForListOfControls(new List<ContentControl>() { btnDeleteFolder }, Visibility.Visible);
                    //add click event handler for folder delete button
                    btnDeleteFolder.Click += this.btnDeleteFolder_Click;
                }
            }            
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

            //disable selection on the folder list view
            foreach (Folder f in folderListView.lvFolderList.Items)
            {
                container = this.folderListView.lvFolderList.ItemContainerGenerator.ContainerFromItem(f);
                if (f.Id != folder.Id)
                {
                    if (container != null)
                    {
                        ((ListViewItem)container).Focusable = false;
                    }                    
                }    
            }           

            //container = null!;
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
            if (GetListViewSelectedItemAsT<Task>(this.taskListView.lvTaskList) is Task && GetListViewSelectedItemAsT<Task>(this.taskListView.lvTaskList).Id > 0)
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

            //enable selection on the folder list view
            foreach (Folder f in folderListView.lvFolderList.Items)
            {
                container = this.folderListView.lvFolderList.ItemContainerGenerator.ContainerFromItem(f);
                ((ListViewItem)container).Focusable = true;
            }
        }

        private void ConfigureControlsWhileAddingTask()
        {
            SetVisibilityForListOfControls(new List<ContentControl>() { this.btnAddFolder, this.btnAddTask, this.btnDeleteTask }, Visibility.Collapsed);
            SetVisibilityForListOfControls(new List<ContentControl>() { this.btnSaveChangesAddTask, this.btnCancelChangesAddTask }, Visibility.Visible);

            this.folderListView.IsEnabled = false;
            this.taskListView.IsEnabled = false;
            this.taskDetailsView.IsEnabled = true;

            if (GetListViewSelectedItemAsT<Task>(this.taskListView.lvTaskList) is Task t && !t.IsComplete)
                this.EnableOrDisableTaskDetailViewControls(isEnabling: true);
            
        }

        private void ConfigureControlsAfterAddingTask()
        {
            SetVisibilityForListOfControls(new List<ContentControl>() { this.btnAddFolder }, Visibility.Visible);
            SetVisibilityForListOfControls(new List<ContentControl>() { this.btnSaveChangesAddTask, this.btnCancelChangesAddTask }, Visibility.Collapsed);

            this.folderListView.IsEnabled = true;
            this.taskListView.IsEnabled = true;
            this.taskDetailsView.IsEnabled = false;

            if (this.taskListView.lvTaskList.HasItems)
                this.btnAddTask.Visibility = Visibility.Visible;

            if (GetListViewSelectedItemAsT<Task>(this.taskListView.lvTaskList) is Task && GetListViewSelectedItemAsT<Task>(this.taskListView.lvTaskList).Id > 0)
            {
                SetVisibilityForListOfControls(new List<ContentControl>() { btnDeleteTask }, Visibility.Visible);
                //show Add Task button only if appropriate for selected folder
                //cannot add tasks directly to Planned, Recurring, Important, or Completed folders
                switch (GetListViewSelectedItemAsT<Folder>(this.folderListView.lvFolderList).FolderName) //TODO: CanHaveTasksAdded should be a property of folder that is defined in the db; for now maybe move it into a get-only property on the folder model (client model only)
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
                this.taskDetailsView.IsEnabled = true;
            }
        }

        private void ConfigureControlsWhileEditingTask()
        {
            List<ContentControl> buttonsToShow = new();
            List<ContentControl> buttonsToUnShow = new();

            buttonsToUnShow.AddRange(new List<ContentControl>() { btnAddFolder, btnAddTask, btnDeleteTask });
            buttonsToShow.AddRange(new List<ContentControl>() { btnSaveChangesEditTask, btnCancelChangesEditTask });

            SetVisibilityForListOfControls(buttonsToShow, Visibility.Visible);
            SetVisibilityForListOfControls(buttonsToUnShow, Visibility.Collapsed);

            this.folderListView.IsEnabled = false;
            this.taskListView.IsEnabled = false;
        }

        private void ConfigureControlsAfterEditingTask()
        {
            List<ContentControl> buttonsToShow = new();
            List<ContentControl> buttonsToUnShow = new();

            buttonsToShow.AddRange(new List<ContentControl>() { btnAddFolder });
            buttonsToUnShow.AddRange(new List<ContentControl>() { btnSaveChangesEditTask, btnCancelChangesEditTask });

            if (GetListViewSelectedItemAsT<Task>(this.taskListView.lvTaskList) is Task t && t.Id > 0)
                buttonsToShow.AddRange(new List<ContentControl>() { this.btnAddTask, this.btnDeleteTask });

            SetVisibilityForListOfControls(buttonsToShow, Visibility.Visible);
            SetVisibilityForListOfControls(buttonsToUnShow, Visibility.Collapsed);

            this.folderListView.IsEnabled = true;
            this.taskListView.IsEnabled = true;
        }

        private Button GetButtonFromContainerByName(DependencyObject container, string buttonName)
            => (AllChildren(container).FirstOrDefault(c => c is Button && c.Name == buttonName) as Button)!;

        private TextBox GetTextBoxFromContainerByName(DependencyObject container, string textBoxName)
            => (AllChildren(container).FirstOrDefault(c => c is TextBox && c.Name == textBoxName) as TextBox)!;
        
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

        private static T GetListViewSelectedItemAsT<T>(ListView listview) => (T)listview.SelectedItem;
        #endregion
    }
}
