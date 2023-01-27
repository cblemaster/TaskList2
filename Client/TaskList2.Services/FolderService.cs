using RestSharp;
using TaskList2.Services.Models;

namespace TaskList2.Services
{
    //https://stackoverflow.com/questions/10226089/restsharp-simple-complete-example

    public class FolderService
    {
        private readonly static string API_BASE_URL = "https://localhost:7021/";
        private readonly RestClient client = new(API_BASE_URL);

        public Folder GetFolder(int id)
        {
            RestRequest request = new("Folder/{id}", Method.Get);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            RestResponse<Folder> response = client.Execute<Folder>(request);
            return response.Data!;
        }

        public List<Folder> GetFolders()
        {
            RestRequest request = new("Folder", Method.Get);
            RestResponse<List<Folder>> response = client.Execute<List<Folder>>(request);
            return response.Data!;
        }

        public Folder AddFolder(Folder folderToAdd)
        {
            RestRequest request = new("Folder", Method.Post) { RequestFormat = DataFormat.Json };
            request.AddBody(folderToAdd);
            RestResponse<Folder> response = client.Execute<Folder>(request);
            return response.Data!;
        }

        public Folder UpdateFolder(Folder folderToUpdate)
        {
            RestRequest request = new("Folder/{id}", Method.Put) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", folderToUpdate.Id, ParameterType.UrlSegment);
            request.AddBody(folderToUpdate);
            RestResponse<Folder> response = client.Execute<Folder>(request);
            return response.Data!;
        }

        public bool DeleteFolder(int id)
        {
            RestRequest request = new("Folder/{id}", Method.Delete);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            RestResponse<bool> response = client.Execute<bool>(request);
            return response.Data;
        }
    }
}
