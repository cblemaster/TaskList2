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
    }
}
