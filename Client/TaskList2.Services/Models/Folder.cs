namespace TaskList2.Services.Models
{
    internal class Folder
    {
        public int Id { get; init; }
        public string FolderName { get; set; } = null!;
        public bool IsDeleteable { get; init; }
        public bool IsRenameable { get; init; }
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
