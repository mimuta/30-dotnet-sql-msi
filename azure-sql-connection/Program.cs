using System;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Azure.Services.AppAuthentication;

namespace sqltest
{
    class Program
    {
        

        
        static void Main(string[] args)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(); 

                  // sql auth
                  // builder.DataSource = "<your_server.database.windows.net>";
                  // builder.UserID = "<your_username>";
                  // builder.Password = "<your_password>";
                  // builder.InitialCatalog = "<your_database>";
                  // builder.ConnectionString = "Server=tcp:sc-azure-test-sql.database.windows.net,1433;Initial Catalog=sc-impl-connection-test;Persist Security Info=False;User ID=mimuta;Password=SLR13mm2005(+;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                  // msi windows - not working
                  builder.ConnectionString = "Server=tcp:sc-azure-test-sql.database.windows.net,1433;Initial Catalog=sc-impl-connection-test;Persist Security Info=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                var azureServiceTokenProvider = new AzureServiceTokenProvider();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    // getting access token to enable MSI.
                    // line should be commented for SQL Auth
                    connection.AccessToken = (new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;
                    
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT TOP 20 pc.Name as CategoryName, p.name as ProductName ");
                    sb.Append("FROM [SalesLT].[ProductCategory] pc ");
                    sb.Append("JOIN [SalesLT].[Product] p ");
                    sb.Append("ON pc.productcategoryid = p.productcategoryid;");
                    String sql = sb.ToString();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("\nDone. Press enter.");
            Console.ReadLine();
        }
    }
}