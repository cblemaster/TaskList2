using System.Data;
using System.Data.SqlClient;
using TaskList2.Data.Helpers;
using TaskList2.Data.Models;
using Task = TaskList2.Data.Models.Task;

namespace TaskList2.Data.DAL
{
    public class FolderSqlDAO : IFolderDAO
    {
        private readonly string _connectionString;

        public FolderSqlDAO(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public Folder AddFolder(Folder folderToAdd)
        {
            try
            {
                int newId = 0;

                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("AddFolder", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@folderName", folderToAdd.FolderName);
                cmd.Parameters.AddWithValue("@isDeleteable", true);
                cmd.Parameters.AddWithValue("@isRenameable", true);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@folderId",
                    Value = newId,
                    IsNullable = false,
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output
                });

                int rowsAffected = cmd.ExecuteNonQuery();

                newId = (int)cmd.Parameters["@folderId"].Value;

                return GetFolder(newId);
            }
            catch (SqlException) { throw; }  //TODO: Exception handling? Custom exceptions?
            catch (Exception) { throw; }
        }

        public bool DeleteFolder(int id)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("DeleteFolder", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@id", id);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected == 1;
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }
        }

        public Folder UpdateFolder(Folder folderToUpdate)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("UpdateFolder", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@id", folderToUpdate.Id);
                cmd.Parameters.AddWithValue("@folderName", folderToUpdate.FolderName);

                int rowsAffected = cmd.ExecuteNonQuery();

                return GetFolder(folderToUpdate.Id);
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }
        }

        public List<Folder> GetFolders()
        {
            List<Folder> fList = new();

            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("GetFolders", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using SqlDataReader reader = cmd.ExecuteReader();
                fList = GetFoldersFromReader(reader);
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }

            return fList;
        }

        public Folder GetFolder(int id)
        {
            Folder f = null!;

            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("GetFolder", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@id", id);

                using SqlDataReader reader = cmd.ExecuteReader();
                f = GetFolderFromReader(reader);
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }

            return f;
        }

        private static Folder GetFolderFromReader(SqlDataReader reader)
        {
            Folder f = new();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    SetVariablesFromReader(reader,
                                           out int folderId,
                                           out string folderName,
                                           out bool isDeleteable,
                                           out bool isRenameable,
                                           out int taskId,
                                           out string taskName,
                                           out DateTime? dueDate,
                                           out int recurrenceId,
                                           out bool isImportant,
                                           out bool isComplete,
                                           out string? note,
                                           out int taskFolderId);

                    if (f.Id == 0)
                    {
                        SetFolderPropertiesFromVariables(f,
                                                         folderId,
                                                         folderName,
                                                         isDeleteable,
                                                         isRenameable);
                    }
                    if (taskId != 0)
                    {
                        Task t = SetTaskPropertiesFromVariables(f,
                                                                taskId,
                                                                recurrenceId,
                                                                taskFolderId,
                                                                taskName,
                                                                note,
                                                                isImportant,
                                                                isComplete,
                                                                dueDate);

                        f.Tasks.Add(t);
                    }
                }
            }
            return f;
        }

        private static void SetVariablesFromReader(SqlDataReader reader,
                                                   out int folderId,
                                                   out string folderName,
                                                   out bool isDeleteable,
                                                   out bool isRenameable,
                                                   out int taskId,
                                                   out string taskName,
                                                   out DateTime? dueDate,
                                                   out int recurrenceId,
                                                   out bool isImportant,
                                                   out bool isComplete,
                                                   out string? note,
                                                   out int taskFolderId)
        {
            folderId = reader.GetFieldValue<int>("FolderId");
            folderName = reader.GetFieldValue<string>("FolderName");
            isDeleteable = reader.GetFieldValue<bool>("IsDeleteable");
            isRenameable = reader.GetFieldValue<bool>("IsRenameable");
            taskId = reader.GetFieldValue<int>("TaskId");
            taskName = reader.GetFieldValue<string>("TaskName");
            dueDate = reader.GetFieldValue<DateTime?>("DueDate");
            recurrenceId = reader.GetFieldValue<int>("RecurrenceId");
            isImportant = reader.GetFieldValue<bool>("IsImportant");
            isComplete = reader.GetFieldValue<bool>("IsComplete");
            note = reader.GetFieldValue<string?>("Note");
            taskFolderId = reader.GetFieldValue<int>("TaskFolderId");
        }

        private static List<Folder> GetFoldersFromReader(SqlDataReader reader)
        {
            Folder f = new();
            List<Folder> fList = new();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    SetVariablesFromReader(reader,
                                           out int folderId,
                                           out string folderName,
                                           out bool isDeleteable,
                                           out bool isRenameable,
                                           out int taskId,
                                           out string taskName,
                                           out DateTime? dueDate,
                                           out int recurrenceId,
                                           out bool isImportant,
                                           out bool isComplete,
                                           out string? note,
                                           out int taskFolderId);

                    if (!fList.Select(f => f.Id).ToList().Contains(folderId))
                    {
                        f = new();
                        SetFolderPropertiesFromVariables(f,
                                                         folderId,
                                                         folderName,
                                                         isDeleteable,
                                                         isRenameable);
                        fList.Add(f);
                    }
                    if (taskId != 0)
                    {
                        Task t = SetTaskPropertiesFromVariables(f,
                                                                taskId,
                                                                recurrenceId,
                                                                taskFolderId,
                                                                taskName,
                                                                note,
                                                                isImportant,
                                                                isComplete,
                                                                dueDate);

                        f.Tasks.Add(t);
                    }
                }
            }
            return fList;
        }

        private static Task SetTaskPropertiesFromVariables(Folder f,
                                                           int taskId,
                                                           int recurrenceId,
                                                           int taskFolderId,
                                                           string taskName,
                                                           string? note,
                                                           bool isImportant,
                                                           bool isComplete,
                                                           DateTime? dueDate) => new()
                                                           {
                                                               Id = taskId,
                                                               TaskName = taskName,
                                                               DueDate = dueDate,
                                                               RecurrenceId = recurrenceId,
                                                               IsImportant = isImportant,
                                                               IsComplete = isComplete,
                                                               Note = note,
                                                               FolderId = taskFolderId,
                                                               Folder = new()
                                                               {
                                                                   Id = f.Id,
                                                                   FolderName = f.FolderName,
                                                                   IsDeleteable = f.IsDeleteable,
                                                                   IsRenameable = f.IsRenameable,
                                                               }
                                                           };

        private static void SetFolderPropertiesFromVariables(Folder f,
                                                             int folderId,
                                                             string folderName,
                                                             bool isDeleteable,
                                                             bool isRenameable)
        {
            f.Id = folderId;
            f.FolderName = folderName;
            f.IsDeleteable = isDeleteable;
            f.IsRenameable = isRenameable;
        }
    }
}
