using System.Data.SqlClient;

namespace TaskList2.Data.Helpers
{
    public static class DataReaderHelpers
    {
        public static T GetFieldValue<T>(this SqlDataReader dr, string name)
        {
            T returnValue = default!;

            if (!dr[name].Equals(DBNull.Value))
            {
                returnValue = (T)dr[name];
            }
            return returnValue!;
        }
    }
}
