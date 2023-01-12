using System.ComponentModel.DataAnnotations;
using TaskList2.Services.Validation;

namespace TaskList2.Services.Models
{
    internal class Folder
    {
        [Key]
        public int Id { get; init; }
        [Required, MaxLength(100)]
        public string FolderName { get; set; } = null!;
        public bool IsDeleteable { get; init; }
        public bool IsRenameable { get; init; }
        public ICollection<Task> Tasks { get; set; } = new List<Task>();

        public ICollection<ValidationError> ValidationErrors { get; set; } = new List<ValidationError>();

        internal static string GetCapitalizedFolderName(string folderName)
        {
            char firstChar = folderName[0];

            if (firstChar.ToString() == firstChar.ToString().ToUpper())
                return folderName;

            return string.Concat(firstChar.ToString().ToUpper(), folderName.AsSpan(1));
        }

        internal static bool IsFolderNameUnique(string folderName)
        {
            FolderService fs = new();
            if (fs.GetFolders().Select(f => f.FolderName).ToList().Contains(folderName))
                return false;

            return true;
        }

        public bool IsValid()
        {
            this.FolderName = GetCapitalizedFolderName(FolderName);

            if (string.IsNullOrEmpty(this.FolderName)
                || string.IsNullOrWhiteSpace(this.FolderName))
                this.ValidationErrors.Add(new() { InvalidPropertyName = "FolderName", ErrorMessage = "FolderName is required." });
            if (!IsFolderNameUnique(this.FolderName))
                this.ValidationErrors.Add(new() { InvalidPropertyName = "FolderName", ErrorMessage = $"FolderName {this.FolderName} is already used." });
            if (this.FolderName.Length > 100)
                this.ValidationErrors.Add(new() { InvalidPropertyName = "FolderName", ErrorMessage = "Max length for FolderName is 100." });

            if (this.ValidationErrors.Any())
                return false;

            return true;
        }
    }
}
