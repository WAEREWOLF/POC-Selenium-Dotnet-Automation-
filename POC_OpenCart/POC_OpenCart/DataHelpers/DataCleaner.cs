using System.Data.SqlClient;
using POC_OpenCart.Configuration;

namespace POC_OpenCart.DataHelpers
{
    public static class DataCleaner
    {
        /* This methods are just examples of cleaning database */
        public static void RemovePasswordForUser(string username)
        {
            using (SqlConnection connection = new SqlConnection(ConfigManager.ConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"update dbo.Users set Password = null where username = @username";
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteIssue(string id)
        {
            using (SqlConnection connection = new SqlConnection(ConfigManager.ConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"delete from dbo.TitleOrders where ParentIssueId = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = @"delete from dbo.TitleOrderIssues where id = @issueId";
                    cmd.Parameters.AddWithValue("@issueId", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
       
    }
}
