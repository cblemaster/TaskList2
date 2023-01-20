using System.Collections.ObjectModel;
using System.ComponentModel;
using TaskList2.Services;
using TaskList2.Services.Models;
using Task = TaskList2.Services.Models.Task;

namespace TaskList2.UI.WPF.Views.Folders
{
    public class FolderListViewModel : INotifyPropertyChanged
    {
        public FolderListViewModel()
        {
            this.Folders = new ObservableCollection<Folder>(_fs.GetFolders());
            foreach (Folder f in this.Folders)
            {
                switch (f.FolderName)
                {
                    case "Planned":
                        f.Tasks = new ObservableCollection<Task>(_ts.GetPlannedTasks());
                        break;
                    case "Completed":
                        f.Tasks = new ObservableCollection<Task>(_ts.GetCompletedTasks());
                        break;
                    case "Recurring":
                        f.Tasks = new ObservableCollection<Task>(_ts.GetRecurringTasks());
                        break;
                    case "Important":
                        f.Tasks = new ObservableCollection<Task>(_ts.GetImportantTasks());
                        break;
                }
            }
        }

        private readonly FolderService _fs = new();
        private readonly TaskService _ts = new();
        private ObservableCollection<Folder> _folders = new();

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        public ObservableCollection<Folder> Folders
        {
            get { return _folders; }
            set
            {
                if (value != _folders)
                {
                    _folders = value;
                    PropertyChanged!(this, new PropertyChangedEventArgs(nameof(Folders)));
                };
            }
        }
    }


}
