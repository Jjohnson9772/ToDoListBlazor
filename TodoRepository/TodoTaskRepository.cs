using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using TodoService;
using Microsoft.Data.SqlClient;

namespace TodoRepository
{
    //this repository is isolated for sql driven methods
    public class TodoTaskRepository
    {
        private string ConnectionString = string.Empty;
        public TodoTaskRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public List<TodoTask> SelectFiltered(bool Completed)
        {
            List<TodoTask> items = new List<TodoTask>();

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand("[dbo].[SelectFilteredItems]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("Completed", Completed));

                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var item = new TodoTask
                            {
                                ItemID = (Int32)dataReader["ItemID"],
                                Name = dataReader["Name"].ToString(),
                                Completed = (bool)dataReader["Completed"],
                                Latitude = (double)dataReader["Latitude"],
                                Longitude = (double)dataReader["Longitude"]
                            };
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
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
                        while (dataReader.Read())
                        {
                            var item = new TodoTask
                            {
                                ItemID = (Int32)dataReader["ItemID"],
                                Name = dataReader["Name"].ToString(),
                                Completed = (bool)dataReader["Completed"],
                                Latitude = (double)dataReader["Latitude"],
                                Longitude = (double)dataReader["Longitude"]
                            };
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }

        public void Save(TodoTask task)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand("[dbo].[UpsertItem]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@ItemID", task.ItemID));
                    sqlCommand.Parameters.Add(new SqlParameter("@Name", task.Name));
                    sqlCommand.Parameters.Add(new SqlParameter("@Completed", task.Completed));

                    sqlCommand.ExecuteNonQuery();
                    
                }
            }
        }

        public void Delete(int itemID) {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand("[dbo].[DeleteItem]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@ItemID", itemID));

                    sqlCommand.ExecuteNonQuery();

                }
            }
        }

        public void SaveTaskMarker(int itemID, double lat, double lng)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand("[dbo].[UpsertTaskMarker]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@lng", lng));
                    sqlCommand.Parameters.Add(new SqlParameter("@Lat", lat));
                    sqlCommand.Parameters.Add(new SqlParameter("@ItemID", itemID));

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
