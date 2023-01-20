using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TaskList2.Services;
using Task = TaskList2.Services.Models.Task;

namespace TaskList2.UI.WPF.Views.Tasks
{
    internal class TaskListViewModel : INotifyPropertyChanged
    {
        public TaskListViewModel()
        {
            //this.Tasks = new ObservableCollection<Task>();
            this.Tasks = new ObservableCollection<Task>(_ts.GetTasks().Where(t => t.FolderId == 5));
        }

        public TaskListViewModel(int folderId)
        {
            folderId = 5;   // TODO: Fix this...
            this.Tasks = new ObservableCollection<Task>(_ts.GetTasks().Where(t => t.FolderId == folderId));
        }

        private readonly TaskService _ts = new();
        private ObservableCollection<Task> _tasks = new();

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        public ObservableCollection<Task> Tasks
        {
            get { return _tasks; }
            set
            {
                if (value != _tasks)
                {
                    _tasks = value;
                    PropertyChanged!(this, new PropertyChangedEventArgs(nameof(Folders)));
                };
            }
        }
    }
}
