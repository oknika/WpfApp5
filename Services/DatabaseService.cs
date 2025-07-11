using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5.Services
{
    public static class DatabaseService
    {
        public static async Task<List<T>> LoadDataFromSQLAsync<T>(
            string connectionString,
            string sqlQuery,
            Func<SqlDataReader, T> mapFunc)
        {
            var result = new List<T>();

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                using SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                await conn.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(mapFunc(reader));
                }
            }
            catch (Exception ex)
            {
                // Optional: log the error
                Console.WriteLine($"[Database Error] {ex.Message}");
                throw; // rethrow so caller can handle it
            }

            return result;
        }
    }
}
