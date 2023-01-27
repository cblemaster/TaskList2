using Microsoft.AspNetCore.Mvc;
using TaskList2.Data.DAL;
using Task = TaskList2.Data.Models.Task;

namespace TaskList2.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskDAO _taskDAO;
        public TaskController(ITaskDAO taskDAO)
        {
            _taskDAO = taskDAO;
        }

        // https://localhost:7021/Task/1
        [HttpGet("{id}")]
        public IActionResult GetTask(int id)
        {
            Task t = _taskDAO.GetTask(id);

            if (t != null)
                return Ok(t);
            else
                return NotFound();
        }

        // https://localhost:7021/Task
        [HttpGet]
        public IActionResult GetTasks()
        {
            List<Task> tList = _taskDAO.GetTasks();

            if (tList != null && tList.Any())
                return Ok(tList);
            else
                return NotFound();
        }

        // https://localhost:7021/Task/important
        [HttpGet("important")]
        public IActionResult GetImportantTasks()
        {
            List<Task> tList = _taskDAO.GetImportantTasks();

            if (tList != null && tList.Any())
                return Ok(tList);
            else
                return NotFound();
        }

        // https://localhost:7021/Task/completed
        [HttpGet("completed")]
        public IActionResult GetCompletedTasks()
        {
            List<Task> tList = _taskDAO.GetCompletedTasks();

            if (tList != null && tList.Any())
                return Ok(tList);
            else
                return NotFound();
        }

        // https://localhost:7021/Task/recurring
        [HttpGet("recurring")]
        public IActionResult GetRecurringTasks()
        {
            List<Task> tList = _taskDAO.GetRecurringTasks();

            if (tList != null && tList.Any())
                return Ok(tList);
            else
                return NotFound();
        }

        // https://localhost:7021/Task/planned
        [HttpGet("planned")]
        public IActionResult GetPlannedTasks()
        {
            List<Task> tList = _taskDAO.GetPlannedTasks();

            if (tList != null && tList.Any())
                return Ok(tList);
            else
                return NotFound();
        }

        // https://localhost:7021/Task
        [HttpPost]
        public IActionResult AddTask(Task taskToAdd)
        {
            Task created = _taskDAO.AddTask(taskToAdd);

            if (created != null && created.Id > 0)
                return Created("Task/" + created.Id, created);
            else
                return BadRequest();
        }

        // https://localhost:7021/Task/1
        [HttpPut("{id}")]
        public IActionResult UpdateTask(Task taskToUpdate)
        {
            Task existing = _taskDAO.GetTask(taskToUpdate.Id);

            if (existing == null)
                return NotFound("Task not found");

            Task updated = _taskDAO.UpdateTask(taskToUpdate);
            return updated != null ? Ok(updated) : StatusCode(500);
        }

        // https://localhost:7021/Task/1
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            Task deleted = _taskDAO.GetTask(id);

            if (deleted == null)
                return NotFound("Task not found");

            bool success = _taskDAO.DeleteTask(id);

            return success ? Ok(success) : StatusCode(500);
        }
    }
}
