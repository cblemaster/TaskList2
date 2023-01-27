using System.ComponentModel.DataAnnotations;
using TaskList2.Services.Validation;

namespace TaskList2.Services.Models
{
    public class Task
    {
        [Key]
        public int Id { get; init; }
        [Required, MaxLength(100)]
        public string TaskName { get; set; } = null!;
        public DateTime? DueDate { get; set; }
        public int RecurrenceId { get; set; }
        public bool IsImportant { get; set; }
        public bool IsComplete { get; set; }
        public int FolderId { get; set; }
        [MaxLength(255)]
        public string? Note { get; set; }
        public Folder Folder { get; set; } = null!;
        public Recurrence Recurrence => (Recurrence)this.RecurrenceId;
        public bool IsPlanned => this.DueDate.HasValue;
        public bool IsRecurring => this.Recurrence != Recurrence.None;

        public ICollection<ValidationError> ValidationErrors { get; set; } = new List<ValidationError>();

        public bool IsValid()
        {
            if (this.TaskName.Length > 100)
                this.ValidationErrors.Add(new() { InvalidPropertyName = "TaskName", ErrorMessage = "Max length for TaskName is 100." });
            if (string.IsNullOrEmpty(this.TaskName)
                || string.IsNullOrWhiteSpace(this.TaskName))
                this.ValidationErrors.Add(new() { InvalidPropertyName = "TaskName", ErrorMessage = "TaskName is required." });
            if (this.Note != null && this.Note.Length > 255)
                this.ValidationErrors.Add(new() { InvalidPropertyName = "Note", ErrorMessage = "Max length for Note is 255." });

            if (this.ValidationErrors.Any())
                return false;

            return true;
        }
    }
}