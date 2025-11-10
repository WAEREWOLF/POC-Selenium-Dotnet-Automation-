using System.Data.SqlClient;
using POC_OpenCart.Configuration;

namespace POC_OpenCart.DataHelpers
{
    public static class DataCreator
    {
        /* This method is an example of populating database */
        public static void InsertOrderTemplate()
        {
            using (SqlConnection connection = new SqlConnection(ConfigManager.ConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"insert into dbo.OrderTemplates (Name, Description, Notes, Enabled, AllowOrderLinesSplit) 
                    values('Automated_Order_Template','Automated_Order_Template', 'automated test', 1, 1)";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
