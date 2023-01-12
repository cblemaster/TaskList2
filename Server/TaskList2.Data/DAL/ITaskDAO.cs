using Task = TaskList2.Data.Models.Task;

namespace TaskList2.Data.DAL
{
    public interface ITaskDAO
    {
        Task GetTask(int id);
        List<Task> GetTasks();
        List<Task> GetImportantTasks();
        List<Task> GetCompletedTasks();
        List<Task> GetRecurringTasks();
        List<Task> GetPlannedTasks();
        Task AddTask(Task taskToAdd);
        Task UpdateTask(Task taskToUpdate);
        bool DeleteTask(int id);
    }
}
