using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using TodoService;
using Microsoft.Data.SqlClient;

namespace TodoRepository
{
    public class TodoTaskRepository
    {
        private string ConnectionString = string.Empty;
        public TodoTaskRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public List<TodoTask> Select()
        {
            List<TodoTask> items = new List<TodoTask>();

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand("[dbo].[SelectAllItems]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            var item = new TodoTask
                            {
                                ItemID = (Int32)dataReader["UserID"],
                                Name = dataReader["DisplayName"].ToString(),
                                Completed = (bool)dataReader["Email"]
                            };
                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }
    }
}
