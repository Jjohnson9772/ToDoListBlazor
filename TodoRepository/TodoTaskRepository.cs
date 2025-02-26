﻿using System;
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

        public async Task<List<TodoTask>> SelectFiltered(bool Completed)
        {
            List<TodoTask> items = new List<TodoTask>();

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                await sqlConnection.OpenAsync();
                using (var sqlCommand = new SqlCommand("[dbo].[SelectFilteredItems]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("Completed", Completed));

                    using (SqlDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        while (await dataReader.ReadAsync())
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

        public async Task<List<TodoTask>> Select()
        {
            List<TodoTask> items = new List<TodoTask>();

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                await sqlConnection.OpenAsync();
                using (var sqlCommand = new SqlCommand("[dbo].[SelectAllItems]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        while (await dataReader.ReadAsync())
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

        public async Task<bool> Save(TodoTask task)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                await sqlConnection.OpenAsync();
                using (var sqlCommand = new SqlCommand("[dbo].[UpsertItem]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@ItemID", task.ItemID));
                    sqlCommand.Parameters.Add(new SqlParameter("@Name", task.Name));
                    sqlCommand.Parameters.Add(new SqlParameter("@Completed", task.Completed));

                    await sqlCommand.ExecuteNonQueryAsync();
                    
                }
                return true;
            }
        }

        public async Task<bool> Delete(int itemID) {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                await sqlConnection.OpenAsync();
                using (var sqlCommand = new SqlCommand("[dbo].[DeleteItem]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@ItemID", itemID));

                    await sqlCommand.ExecuteNonQueryAsync();

                }
            }
            return true;
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
