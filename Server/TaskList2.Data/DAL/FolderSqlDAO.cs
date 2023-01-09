using System.Data;
using System.Data.SqlClient;
using TaskList2.Data.Models;
using Task = TaskList2.Data.Models.Task;

namespace TaskList2.Data.DAL
{
    internal class FolderSqlDAO : IFolderDAO
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

                return GetFolderWithTasks(newId);
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

                return GetFolderWithTasks(folderToUpdate.Id);
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }
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

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows && reader.Read())
                {
                    f = GetFolderFromReader(reader);
                }
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }

            return f;
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

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Folder f = GetFolderFromReader(reader);
                        fList.Add(f);
                    }
                }

            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }

            return fList;
        }

        public List<Folder> GetFoldersWithTasks()
        {
            List<Folder> fList = new();

            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("GetFoldersWithTasks", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = cmd.ExecuteReader();
                fList = GetFoldersWithTasksFromReader(reader);
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }

            return fList;
        }

        public Folder GetFolderWithTasks(int id)
        {
            Folder f = null!;

            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("GetFolderWithTasks", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();
                f = GetFolderWithTasksFromReader(reader);
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }

            return f;
        }

        private static Folder GetFolderFromReader(SqlDataReader reader)
        {
            Folder f = new()
            {
                Id = reader.GetFieldValue<int>("FolderId"),
                FolderName = reader.GetFieldValue<string>("FolderName"),
                IsDeleteable = reader.GetFieldValue<bool>("IsDeleteable"),
                IsRenameable = reader.GetFieldValue<bool>("IsDeleteable")
            };

            return f;
        }

        private static List<Folder> GetFoldersWithTasksFromReader(SqlDataReader reader)
        {
            List<Folder> fList = new();
            Dictionary<(int id, string folderName, bool isDeleteable, bool IsRenameable), List<Task>> fDict = new();
            (int id, string folderName, bool isDeleteable, bool isRenameable) key = (0, string.Empty, false, false);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    key = GetKeyForFolderAndTasksDictionary(reader);

                    if (!fDict.ContainsKey(key))
                    {
                        fDict.Add(key, new List<Task>());
                    }

                    if (reader.GetFieldValue<int>("TaskId") != 0)
                    {
                        Task t = InstantiateTaskFromReader(reader, key);

                        fDict[key].Add(t);
                    }

                    Folder f = InstantiateFolderFromDictionary(fDict, key);

                    fList.Add(f);
                }
            }

            return fList;
        }

        private static Folder GetFolderWithTasksFromReader(SqlDataReader reader)
        {
            Dictionary<(int id, string folderName, bool isDeleteable, bool IsRenameable), List<Task>> fDict = new();
            (int id, string folderName, bool isDeleteable, bool isRenameable) key = (0, string.Empty, false, false);

            if (reader.HasRows && reader.Read())
            {
                key = GetKeyForFolderAndTasksDictionary(reader);

                if (!fDict.ContainsKey(key))
                {
                    fDict.Add(key, new List<Task>());
                }

                if (reader.GetFieldValue<int>("TaskId") != 0)
                {
                    Task t = InstantiateTaskFromReader(reader, key);
                    fDict[key].Add(t);
                }
            }
            Folder f = InstantiateFolderFromDictionary(fDict, key);

            return f;
        }

        private static Folder InstantiateFolderFromDictionary(Dictionary<(int id, string folderName, bool isDeleteable, bool IsRenameable), List<Task>> fDict, (int id, string folderName, bool isDeleteable, bool isRenameable) key)
            => new()
            {
                Id = key.id,
                FolderName = key.folderName,
                IsDeleteable = key.isDeleteable,
                IsRenameable = key.isRenameable,
                Tasks = fDict[key]
            };

        private static Task InstantiateTaskFromReader(SqlDataReader reader, (int id, string folderName, bool isDeleteable, bool isRenameable) key)
            => new()
            {
                Id = reader.GetFieldValue<int>("TaskId"),
                TaskName = reader.GetFieldValue<string>("TaskName"),
                DueDate = reader.GetFieldValue<DateTime?>("DueDate"),
                RecurrenceId = reader.GetFieldValue<int>("RecurrenceId"),
                IsImportant = reader.GetFieldValue<bool>("IsImportant"),
                IsComplete = reader.GetFieldValue<bool>("IsComplete"),
                FolderId = reader.GetFieldValue<int>("FolderId"),
                Note = reader.GetFieldValue<string?>("Note"),
                Folder = new()
                {
                    Id = key.id,
                    FolderName = key.folderName,
                    IsDeleteable = key.isDeleteable,
                    IsRenameable = key.isRenameable,
                }
            };        

        private static (int id, string folderName, bool isDeleteable, bool isRenameable) GetKeyForFolderAndTasksDictionary(SqlDataReader reader)
            => (reader.GetFieldValue<int>("FolderId"),
                    reader.GetFieldValue<string>("FolderName"),
                    reader.GetFieldValue<bool>("IsDeleteable"),
                    reader.GetFieldValue<bool>("IsRenameable"));        
    }
}
