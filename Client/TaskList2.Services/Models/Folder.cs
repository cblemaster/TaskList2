using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;
using TaskList2.Services.Validation;

namespace TaskList2.Services.Models
{
    public class Folder
    {
        [Key]
        public int Id { get; init; }
        [Required, MaxLength(100)]
        public string FolderName { get; set; } = null!;
        public bool IsDeleteable { get; init; }
        public bool IsRenameable { get; init; }
        public ICollection<Task> Tasks { get; set; } = new List<Task>();

        public ICollection<ValidationError> ValidationErrors { get; set; } = new List<ValidationError>();

        private static string GetCapitalizedFolderName(string folderName)
        {
            if (string.IsNullOrEmpty(folderName)
                || string.IsNullOrWhiteSpace(folderName))
                return string.Empty;

            folderName = folderName.Trim();

            char firstChar = folderName[0];

            return firstChar.ToString() == firstChar.ToString().ToUpper()
                ? folderName
                : string.Concat(firstChar.ToString().ToUpper(), folderName.AsSpan(1));
        }

        private static bool IsFolderNameUnique(string folderName)
        {
            FolderService fs = new();
            return !fs.GetFolders().Select(f => f.FolderName).ToList().Contains(folderName);
        }

        public string GetValidationErrorsAsString()
        {
            List<ValidationError> errors = this.ValidationErrors.ToList();
            StringBuilder sb = new();
            for (int i = 0; i < errors.Count; i++)
            {
                if (errors.Count == 1 || i == errors.Count - 1)
                {
                    sb.Append(errors[i].ErrorMessage);
                }
                sb.AppendLine(errors[i].ErrorMessage);
            }
            return sb.ToString();
        }

        public bool IsValid()
        {
            this.ValidationErrors = new List<ValidationError>();

            this.FolderName = GetCapitalizedFolderName(FolderName);

            if (string.IsNullOrEmpty(this.FolderName)
                || string.IsNullOrWhiteSpace(this.FolderName))
                this.ValidationErrors.Add(new() { InvalidPropertyName = "FolderName", ErrorMessage = "Folder Name is required." });
            if (!IsFolderNameUnique(this.FolderName))
                this.ValidationErrors.Add(new() { InvalidPropertyName = "FolderName", ErrorMessage = $"Folder Name {this.FolderName} is already used." });
            if (this.FolderName.Length > 100)
                this.ValidationErrors.Add(new() { InvalidPropertyName = "FolderName", ErrorMessage = "Max length for Folder Name is 100." });

            return !this.ValidationErrors.Any();
        }
    }
}
