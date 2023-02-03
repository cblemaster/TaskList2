using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskList2.Services;
using TaskList2.Services.Models;
using Task = TaskList2.Services.Models.Task;

namespace TaskList2.UI.WPF2
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            this.Folders = 
                new ObservableCollection<Folder>
                    (_folderService.GetFolders()
                    .OrderBy(f => f.IsRenameable)
                    .ThenBy(f => f.FolderName));

            foreach (Folder folder in Folders)
            {
                if (!folder.Tasks.Any())
                {
                    switch (folder.FolderName)
                    {
                        case "Planned":
                            folder.Tasks = 
                                new ObservableCollection<Task>
                                    (_taskService.GetPlannedTasks()
                                    .OrderBy(t => t.DueDate)    // TODO: Fix this sorting; I think the null due dates are screwing up the sort
                                    .ThenBy(t => t.TaskName));
                            break;
                        case "Important":
                            folder.Tasks =
                                new ObservableCollection<Task>
                                    (_taskService.GetImportantTasks()
                                    .OrderBy(t => t.DueDate)
                                    .ThenBy(t => t.TaskName));
                            break;
                        case "Recurring":
                            folder.Tasks =
                                new ObservableCollection<Task>
                                    (_taskService.GetRecurringTasks()
                                    .OrderBy(t => t.DueDate)
                                    .ThenBy(t => t.TaskName));
                            break;
                        case "Completed":
                            folder.Tasks =
                                new ObservableCollection<Task>
                                (_taskService.GetCompletedTasks()
                                .OrderBy(t => t.DueDate)
                                .ThenBy(t => t.TaskName));
                            break;
                        default:
                            break;
                    }
                }
            }            
        }
        
        private readonly FolderService _folderService = new();
        private readonly TaskService _taskService = new();

        private ObservableCollection<Folder> _folders = new();

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
                }                
            }
        }
    }
}
