using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TaskList2.Services;
using TaskList2.Services.Models;
using Task = TaskList2.Services.Models.Task;
using ValidationError = TaskList2.Services.Validation.ValidationError;

namespace TaskList2.UI.WPF.Views
{
    /// <summary>
    /// Interaction logic for MasterDetailsView.xaml
    /// </summary>
    public partial class MasterDetailsView : UserControl
    {
        public MasterDetailsView() => InitializeComponent();
        
        private readonly FolderService _folderService = new();

        private MasterDetailsViewModel GetViewModel() => (MasterDetailsViewModel)this.DataContext;

        private void lvFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Folder selectedFolder = (Folder)this.lvFolders.SelectedItem;

            if (selectedFolder != null)
            {
                MasterDetailsViewModel context = this.GetViewModel();
                context.SelectedFolder = selectedFolder;
                context.Tasks = new ObservableCollection<Task>(selectedFolder.Tasks);
                context.SelectedTask = new();
            }
        }

        private void lvTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MasterDetailsViewModel context = this.GetViewModel();
            context.SelectedTask = (Task)this.lvTasks.SelectedItem;            
        }

        private void btnAddFolder_Click(object sender, RoutedEventArgs e) => this.EnableAddEditFolderControls(isAdd: true);
        
        private void btnAddFolderConf_Click(object sender, RoutedEventArgs e)
        {
            MasterDetailsViewModel context = this.GetViewModel();
            Folder folderToAdd = context.FolderToAddOrUpdate;

            if (folderToAdd.IsValid())
            {
                Folder addedFolder = this._folderService.AddFolder(folderToAdd);
                context.Folders.Add(addedFolder);
                context.FolderToAddOrUpdate = new();

                this.lvFolders.SelectedItem = addedFolder;

                this.DisableAddEditFolderControls(isAdd: true);
            }
            else
                this.ShowValidationErrors(folderToAdd);
        }

        private void btnRenameFolder_Click(object sender, RoutedEventArgs e)
        {
            this.EnableAddEditFolderControls(isAdd: false);

            MasterDetailsViewModel context = this.GetViewModel();
            context.FolderToAddOrUpdate = new()
            {
                Id = context.SelectedFolder.Id,
                FolderName = context.SelectedFolder.FolderName,
                IsDeleteable = context.SelectedFolder.IsDeleteable,
                IsRenameable = context.SelectedFolder.IsRenameable,
                Tasks = context.SelectedFolder.Tasks,
            };
        }

        private void btnRenameFolderConf_Click(object sender, RoutedEventArgs e)
        {
            MasterDetailsViewModel context = this.GetViewModel();
            Folder folderToRename = context.FolderToAddOrUpdate;

            if (folderToRename.IsValid())
            {
                Folder renamedFolder = _folderService.UpdateFolder(folderToRename);
                context.Folders.Add(renamedFolder);
                context.Folders.Remove((Folder)this.lvFolders.SelectedItem);
                context.FolderToAddOrUpdate = new();

                this.lvFolders.SelectedItem = renamedFolder;

                this.DisableAddEditFolderControls(isAdd: true);
            }
            else
                this.ShowValidationErrors(folderToRename);
        }

        private void btnCancelAddEditFolder_Click(object sender, RoutedEventArgs e)
        {
            bool isAdd = this.btnAddTaskConf.Visibility == Visibility.Visible;
            this.DisableAddEditFolderControls(isAdd: isAdd);

            MasterDetailsViewModel context = this.GetViewModel();
            
            if (isAdd)
                context.FolderToAddOrUpdate = new();
            else
                context.FolderToAddOrUpdate = new();
        }

        private void btnDeleteFolder_Click(object sender, RoutedEventArgs e)
        {
            string message = "Delete this folder and everything in it?";
            string title = "Delete folder?";
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage image = MessageBoxImage.Warning;
            MessageBoxResult defaultSelection = MessageBoxResult.No;

            MessageBoxResult dialogResult = MessageBox.Show(message,
                title, buttons, image, defaultSelection);

            if (dialogResult == MessageBoxResult.Yes)
            {
                Folder folderToDelete = (Folder)this.lvFolders.SelectedItem;

                if (folderToDelete != null && folderToDelete.IsDeleteable)
                {
                    FolderService fs = new();
                    if (fs.DeleteFolder(folderToDelete.Id))
                    {
                        MasterDetailsViewModel context = this.GetViewModel();
                        context.Folders.Remove(folderToDelete);
                    }
                }
            }
        }

        private void EnableAddEditFolderControls(bool isAdd)
        {
            this.grdMain.Visibility = Visibility.Collapsed;
            this.grdAddEditFolder.Visibility = Visibility.Visible;

            this.btnCancelAddEditFolder.Visibility = Visibility.Visible;

            if (isAdd)
                this.btnAddFolderConf.Visibility = Visibility.Visible;
            else
                this.btnRenameFolderConf.Visibility = Visibility.Visible;
        }

        private void DisableAddEditFolderControls(bool isAdd)
        {
            this.btnCancelAddEditFolder.Visibility = Visibility.Collapsed;

            this.grdAddEditFolder.Visibility = Visibility.Collapsed;
            this.grdMain.Visibility = Visibility.Visible;

            this.lblFolderNameErrors.ToolTip = null;
            this.lblFolderNameErrors.Visibility = Visibility.Collapsed;

            if (isAdd)
                this.btnAddFolderConf.Visibility = Visibility.Collapsed;
            else
                this.btnRenameFolderConf.Visibility = Visibility.Collapsed;
        }       

        private void ShowValidationErrors(Folder folderToValidate)
        {
            this.lblFolderNameErrors.Visibility = Visibility.Visible;
            this.lblFolderNameErrors.ToolTip = null;

            StringBuilder sb = new();
            foreach (ValidationError error in folderToValidate.ValidationErrors)
            {
                if (error.InvalidPropertyName == "FolderName")
                {
                    if (sb.Length > 0)
                        sb.AppendLine(error.ErrorMessage);
                    else
                        sb.Append(error.ErrorMessage);
                }
            }

            this.lblFolderNameErrors.ToolTip = sb.ToString();
        }
    }
}
