namespace NIASReport
{
    public class DatabaseHelper
    {
        public static string ConnectionString;
        public static void Initialize(string dbFile)
        {
            ConnectionString = string.Format("Provider=Microsoft.Data.Sqlite;Data Source=\"{0}\";Journal Mode=Off;Version=3", dbFile);
        }
        public static void SaveData<T>(IEnumerable<T> list)
        {
        }
    }
}