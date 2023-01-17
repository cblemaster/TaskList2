using System.ComponentModel;
using TaskList2.Services.Models;

namespace TaskList2.UI.WPF.Views.Folders
{
    internal class AddUpdateFolderViewModel : INotifyPropertyChanged
    {
        public AddUpdateFolderViewModel()
        {
        }

        private Folder _folder = new();

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        public Folder Folder
        {
            get { return _folder; }
            set
            {
                if (value != _folder)
                {
                    _folder = value;
                    PropertyChanged!(this, new PropertyChangedEventArgs(nameof(Folder)));
                };
            }
        }
    }
}
