using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5.Generator
{
    public class GetLastTransID
    {
        public static async Task<string> GetLastTransIdFromDatabase(string connectionString, string tblName)
        {
            string prefix = "";
            string fldName = "";
            if (tblName == "PurchaseOrders")
            {
                prefix = "PO";
                fldName = "PurchaseOrderID";
            }

            string lastId = prefix + "000"; // fallback

            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            string query = "SELECT TOP 1 " + fldName + " FROM " + tblName + " ORDER BY " + fldName + " DESC";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                lastId = reader.GetString(0); // assuming OrderId is a string
            }

            return lastId;
        }
    }
}
