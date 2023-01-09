namespace TaskList2.Data.Models
{
    public class Task
    {
        public int Id { get; init; }
        public string TaskName { get; set; } = null!;
        public DateTime? DueDate { get; set; }
        public int RecurrenceId { get; set; }
        public bool IsImportant { get; set; }
        public bool IsComplete { get; set; }
        public int FolderId { get; set; }
        public string? Note { get; set; }
        public Folder Folder { get; set; } = null!;
        public Recurrence Recurrence => (Recurrence)this.RecurrenceId;
        public bool IsPlanned => this.DueDate.HasValue;
        public bool IsRecurring => this.Recurrence != Recurrence.None;
    }
}