using System.Data;
using System.Data.SqlClient;

namespace API_AutomationFramework.Helpers
{
    public class DButil
    {
        private static SqlConnection? dbConn;
        private static SqlDataAdapter? SQLDataAdapter;
        public static DataTable? dtResult = null;

        private static SqlConnection? SqlConn(string ConnectionString)
        {
            dbConn = new SqlConnection();
            try
            {
                dbConn.ConnectionString = ConnectionString;
                dbConn?.Open();

            }
            catch
            {
                if (dbConn != null)
                    dbConn?.Dispose();
                return null;
                
            }
            return dbConn;
        }

        public static void DBConnectionClose()
        {
            try
            {
                dbConn?.Close();
                dbConn?.Dispose();
            }
            catch
            {
                throw new Exception("An Error occured while closing DB");
            }
        }

        public static void ExecuteSQLQuery(string queryStr, string db)
        {
            SqlConn(db);
            SqlCommand command = new SqlCommand(queryStr, dbConn);
            command?.ExecuteNonQuery();
            DBConnectionClose();
        }

        public static DataTable? getDataFromQuery(string queryString, string db)
        {
            //DataTable dtResult=null;
            try
            {
                SqlConn(db);
                // Create a SqlDataAdapter to get the results as DataTable
                Console.WriteLine(queryString);
                SQLDataAdapter = new SqlDataAdapter(queryString, dbConn);
                // Create a new DataTable
                dtResult = new DataTable();
                // Fill the DataTable with the result of the SQL statement
                SQLDataAdapter?.Fill(dtResult);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DBConnectionClose();
            }

            return dtResult;
        }

        public static string[] getAllDtRowValues(DataTable dtResult)
        {
            string[] arrList = new string[dtResult.Rows.Count];
            List<string> lst = new List<string>();
            int i = 0;
            foreach (DataRow row in dtResult.Rows)
            {
                foreach (var col in row.ItemArray)
                {
                    lst.Add(col?.ToString()!);
                }
                arrList[i] = string.Join("[::]", lst.ToArray());
                i++;
                lst.Clear();
            }

            return arrList;
        }

    }
}
