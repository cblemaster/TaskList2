using RestSharp;

namespace TaskList2.Services
{
    //https://stackoverflow.com/questions/10226089/restsharp-simple-complete-example

    internal class TaskService
    {
        private readonly static string API_BASE_URL = "https://localhost:7021/";
        private readonly RestClient client = new(API_BASE_URL);

        internal Task GetTask(int id)
        {
            RestRequest request = new("Task/{id}", Method.Get);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            RestResponse<Task> response = client.Execute<Task>(request);
            return response.Data!;
        }

        internal List<Task> GetTasks()
        {
            RestRequest request = new("Task", Method.Get);
            RestResponse<List<Task>> response = client.Execute<List<Task>>(request);
            return response.Data!;
        }

        internal List<Task> GetImportantTasks()
        {
            RestRequest request = new("Task/important", Method.Get);
            RestResponse<List<Task>> response = client.Execute<List<Task>>(request);
            return response.Data!;
        }

        internal List<Task> GetCompletedTasks()
        {
            RestRequest request = new("Task/completed", Method.Get);
            RestResponse<List<Task>> response = client.Execute<List<Task>>(request);
            return response.Data!;
        }

        internal List<Task> GetRecurringTasks()
        {
            RestRequest request = new("Task/recurring", Method.Get);
            RestResponse<List<Task>> response = client.Execute<List<Task>>(request);
            return response.Data!;
        }

        internal List<Task> GetPlannedTasks()
        {
            RestRequest request = new("Task/planned", Method.Get);
            RestResponse<List<Task>> response = client.Execute<List<Task>>(request);
            return response.Data!;
        }

        internal Task AddTask(Task taskToAdd)
        {
            RestRequest request = new("Task", Method.Post) { RequestFormat = DataFormat.Json };
            request.AddBody(taskToAdd);
            RestResponse<Task> response = client.Execute<Task>(request);
            return response.Data!;
        }

        internal Task UpdateTask(Task taskToUpdate)
        {
            RestRequest request = new("Task/{id}", Method.Put) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", taskToUpdate.Id);
            request.AddBody(taskToUpdate);
            RestResponse<Task> response = client.Execute<Task>(request);
            return response.Data!;
        }

        internal bool DeleteTask(int id)
        {
            RestRequest request = new("Task/{id}", Method.Delete);
            request.AddParameter("id", id);
            RestResponse<bool> response = client.Execute<bool>(request);
            return response.Data;
        }
    }
}
