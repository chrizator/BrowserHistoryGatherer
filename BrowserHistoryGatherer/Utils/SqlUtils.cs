using System.Data;
using System.Data.SQLite;

namespace BrowserHistoryGatherer.Utils
{
    public static class SqlUtils
    {
        public static DataTable QueryDataTable(string dbPath, string query)
        {
            SQLiteDataAdapter sqlDataAdapter;
            DataTable dataTable = new DataTable();

            string connectionString = string.Format("Data Source={0};New=False;Version=3;Compress=True", dbPath);
            using (SQLiteConnection sqlConnection = new SQLiteConnection(connectionString))
            {
                sqlConnection.Open();
                sqlConnection.CreateCommand();

                sqlDataAdapter = new SQLiteDataAdapter(query, sqlConnection);
                sqlDataAdapter.Fill(dataTable);

                return dataTable;
            } 
        }
    }
}