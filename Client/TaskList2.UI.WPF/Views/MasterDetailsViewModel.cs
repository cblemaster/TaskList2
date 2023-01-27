using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TaskList2.Services;
using TaskList2.Services.Models;
using Task = TaskList2.Services.Models.Task;

namespace TaskList2.UI.WPF.Views
{
    class MasterDetailsViewModel : INotifyPropertyChanged
    {
        public MasterDetailsViewModel()
        {
            this.Folders = new ObservableCollection<Folder>(_folderService.GetFolders());

            foreach (Folder f in this.Folders)
            {
                switch (f.FolderName)
                {
                    case "Planned":
                        f.Tasks = new ObservableCollection<Task>(_taskService.GetPlannedTasks());
                        break;
                    case "Important":
                        f.Tasks = new ObservableCollection<Task>(_taskService.GetImportantTasks());
                        break;
                    case "Completed":
                        f.Tasks = new ObservableCollection<Task>(_taskService.GetCompletedTasks());
                        break;
                    case "Recurring":
                        f.Tasks = new ObservableCollection<Task>(_taskService.GetRecurringTasks());
                        break;
                    case "Tasks":
                        f.Tasks = new ObservableCollection<Task>(_taskService.GetTasks().Where(t => t.Folder.FolderName == "Tasks"));
                        break;
                    default:
                        break;
                }
            }
        }

        private readonly FolderService _folderService = new();
        private readonly TaskService _taskService = new();

        private Folder _selectedFolder = new();
        private Task _selectedTask = new();

        private ObservableCollection<Folder> _folders = new();
        private ObservableCollection<Task> _tasks = new();

        private Folder _folderToAddOrUpdate = new();
        private Task _taskToAddOrUpdate = new();

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        public ObservableCollection<Folder> Folders
        {
            get => _folders;
            set
            {
                if (value != _folders)
                {
                    _folders = value;
                    this.PropertyChanged!(this, new PropertyChangedEventArgs(nameof(Folders)));
                };
            }
        }

        public ObservableCollection<Task> Tasks
        {
            get => _tasks;
            set
            {
                if (value != _tasks)
                {
                    _tasks = value;
                    this.PropertyChanged!(this, new PropertyChangedEventArgs(nameof(Tasks)));
                };
            }
        }

        public Folder SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                if (value != _selectedFolder)
                {
                    _selectedFolder = value;
                    this.PropertyChanged!(this, new PropertyChangedEventArgs(nameof(SelectedFolder)));
                }
            }
        }

        public Task SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (value != _selectedTask)
                {
                    _selectedTask = value;
                    this.PropertyChanged!(this, new PropertyChangedEventArgs(nameof(SelectedTask)));
                }
            }
        }

        public Folder FolderToAddOrUpdate
        {
            get => _folderToAddOrUpdate;
            set
            {
                if (value != _folderToAddOrUpdate)
                {
                    _folderToAddOrUpdate = value;
                    this.PropertyChanged!(this, new PropertyChangedEventArgs(nameof(FolderToAddOrUpdate)));
                }
            }
        }

        public Task TaskToAddOrUpdate
        {
            get => _taskToAddOrUpdate;
            set
            {
                if (value != _taskToAddOrUpdate)
                {
                    _taskToAddOrUpdate = value;
                    this.PropertyChanged!(this, new PropertyChangedEventArgs(nameof(TaskToAddOrUpdate)));
                }
            }
        }
    }
}
