using System.ComponentModel.DataAnnotations;

namespace TaskList2.Services.Models
{
    internal class Folder
    {
        [Key]
        public int Id { get; init; }
        // TODO: New/updated folder names must be unique
        // TODO: New/updated folder names must be capitalized
        [Required, MaxLength(100)]
        public string FolderName { get; set; } = null!;
        public bool IsDeleteable { get; init; }
        public bool IsRenameable { get; init; }
        public ICollection<Task> Tasks { get; set; } = new List<Task>();

        internal string GetCapitalizedFolderName(string folderName)
        {
            char firstChar = folderName[0];
            
            if (firstChar.ToString() == firstChar.ToString().ToUpper())
                return folderName;
            
            return firstChar.ToString().ToUpper() + folderName.Substring(1);
        }

        internal bool IsFolderNameUnique(string folderName)
        {
            FolderService fs = new();
            if (fs.GetFolders().Select(f => f.FolderName).ToList().Contains(folderName))
                return false;
            
            return true;
        }
    }
}
