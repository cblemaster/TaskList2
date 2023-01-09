using System.Data;
using System.Data.SqlClient;
using Task = TaskList2.Data.Models.Task;

namespace TaskList2.Data.DAL
{
    internal class TaskSqlDAO : ITaskDAO
    {
        private readonly string _connectionString;

        public TaskSqlDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task AddTask(Task taskToAdd)
        {
            try
            {
                int newId = 0;

                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("AddTask", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@taskName", taskToAdd.TaskName);
                cmd.Parameters.AddWithValue("@dueDate", taskToAdd.DueDate == null
                                                               ? DBNull.Value
                                                               : taskToAdd.DueDate);
                cmd.Parameters.AddWithValue("@recurrenceId", taskToAdd.RecurrenceId);
                cmd.Parameters.AddWithValue("@isImportant", taskToAdd.IsImportant);
                cmd.Parameters.AddWithValue("@isComplete", taskToAdd.IsComplete);
                cmd.Parameters.AddWithValue("@folderId", taskToAdd.FolderId);
                cmd.Parameters.AddWithValue("@note", taskToAdd.Note == null
                                                            ? DBNull.Value
                                                            : taskToAdd.Note);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@taskId",
                    Value = newId,
                    IsNullable = false,
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output
                });

                int rowsAffected = cmd.ExecuteNonQuery();

                newId = (int)cmd.Parameters["@taskId"].Value;

                return GetTask(newId);
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }
        }
        public bool DeleteTask(int id)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("DeleteTask", conn)
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
        public Task UpdateTask(Task taskToUpdate)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("UpdateTask", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@id", taskToUpdate.Id);
                cmd.Parameters.AddWithValue("@taskName", taskToUpdate.TaskName);
                cmd.Parameters.AddWithValue("@dueDate", taskToUpdate.DueDate == null
                                                                    ? DBNull.Value
                                                                    : taskToUpdate.DueDate);
                cmd.Parameters.AddWithValue("@recurrenceId", taskToUpdate.RecurrenceId);
                cmd.Parameters.AddWithValue("@isImportant", taskToUpdate.IsImportant);
                cmd.Parameters.AddWithValue("@isComplete", taskToUpdate.IsComplete);
                cmd.Parameters.AddWithValue("@folderId", taskToUpdate.FolderId);
                cmd.Parameters.AddWithValue("@note", taskToUpdate.Note == null
                                                                 ? DBNull.Value
                                                                 : taskToUpdate.Note);

                int rowsAffected = cmd.ExecuteNonQuery();

                return GetTask(taskToUpdate.Id);
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }
        }
        public List<Task> GetCompletedTasks()
        {
            List<Task> tList = new();

            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("GetCompletedTasks", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Task t = GetTaskFromReader(reader);
                        tList.Add(t);
                    }
                }                
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }

            return tList;
        }
        public List<Task> GetImportantTasks()
        {
            List<Task> tList = new();

            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("GetImportantTasks", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Task t = GetTaskFromReader(reader);
                        tList.Add(t);
                    }
                }                
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }

            return tList;
        }
        public List<Task> GetPlannedTasks()
        {
            List<Task> tList = new();

            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("GetPlannedTasks", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Task t = GetTaskFromReader(reader);
                        tList.Add(t);
                    }
                }                
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }

            return tList;
        }
        public List<Task> GetRecurringTasks()
        {
            List<Task> tList = new();

            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("GetRecurringTasks", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Task t = GetTaskFromReader(reader);
                        tList.Add(t);
                    }
                }                
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }

            return tList;
        }
        public Task GetTask(int id)
        {
            Task t = null!;

            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("GetTask", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows && reader.Read())
                {
                    t = GetTaskFromReader(reader);
                }
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }

            return t;
        }
        public List<Task> GetTasks()
        {
            List<Task> tList = new();

            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                SqlCommand cmd = new("GetTasks", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Task t = GetTaskFromReader(reader);
                        tList.Add(t);
                    }
                }                
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }

            return tList;
        }

        private static Task GetTaskFromReader(SqlDataReader reader)
        {
            Task t = new()
            {
                Id = reader.GetFieldValue<int>("TaskId"),
                TaskName = reader.GetFieldValue<string>("TaskName"),
                DueDate = reader.GetFieldValue<DateTime?>("DueDate"),
                RecurrenceId = reader.GetFieldValue<int>("RecurrenceId"),
                IsImportant = reader.GetFieldValue<bool>("IsImportant"),
                IsComplete = reader.GetFieldValue<bool>("IsComplete"),
                FolderId = reader.GetFieldValue<int>("TaskFolderId"),
                Note = reader.GetFieldValue<string?>("Note"),
                Folder = new()
                {
                    Id = reader.GetFieldValue<int>("FolderId"),
                    FolderName = reader.GetFieldValue<string>("FolderName"),
                    IsDeleteable = reader.GetFieldValue<bool>("IsDeleteable"),
                    IsRenameable = reader.GetFieldValue<bool>("IsRenameable")
                }
            };

            return t;
        }
    }
}
