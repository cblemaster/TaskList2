namespace TaskList2.Data.Models
{
    public class Folder
    {
        public int Id { get; set; }
        public string FolderName { get; set; } = null!;
        public bool IsDeleteable { get; set; }
        public bool IsRenameable { get; set; }
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
