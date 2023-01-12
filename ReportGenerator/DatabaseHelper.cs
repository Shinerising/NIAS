using Dapper;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace NIASReport
{
    public class DatabaseHelper : IDisposable
    {
        public static DatabaseHelper? Instance { get; private set; }
        public static DatabaseHelper Initialize(string dbFile)
        {
            Instance = new DatabaseHelper(dbFile);
            return Instance;
        }

        private static readonly List<Type> ReserveTypes = new() { typeof(RawData.Switch), typeof(RawData.Adapter), typeof(RawData.Connection) };

        public event ErrorEventHandler? ErrorHandler;
        public string ConnectionString;
        private readonly SqliteConnection connection;
        private DatabaseHelper(string dbFile)
        {
            ConnectionString = string.Format("Provider=Microsoft.Data.Sqlite;Data Source=\"{0}\";Journal Mode=Off;Version=3", dbFile);
            connection = new(ConnectionString);
        }
        public async Task OpenDatabase()
        {
            try
            {
                await connection.OpenAsync();
                await InitialDatabase();
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, new ErrorEventArgs(e));
            }
        }
        public async Task CloseDatabase()
        {
            try
            {
                await connection.CloseAsync();
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, new ErrorEventArgs(e));
            }
        }

        private async Task InitialDatabase()
        {
            foreach (Type type in ReserveTypes)
            {
                await CreateTable(type);
            }
        }

        public async Task SaveData<T>(IEnumerable<T> list)
        {
            try
            {
                string tableName = typeof(T).Name;
                using var transaction = connection.BeginTransaction();
                string sql = string.Format("INSERT INTO {0} VALUES ({1}) ;", tableName, string.Join(", ", typeof(T).GetProperties().Select(item => "@" + item.Name)));
                foreach (T item in list)
                {
                    await connection.ExecuteAsync(sql, item, transaction: transaction);
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, new ErrorEventArgs(e));
            }
        }

        public async Task<IEnumerable<T>> GetData<T>(string filter = "")
        {
            try
            {
                string tableName = typeof(T).Name;
                string sql = string.Format("SELECT * FROM {0} {1};", tableName, filter);
                return await connection.QueryAsync<T>(sql);
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, new ErrorEventArgs(e));
            }
            return Enumerable.Empty<T>();
        }

        public async Task<IEnumerable<T>> GetDataByTime<T>(DateTimeOffset startTime, DateTimeOffset endTime)
        {
            string filter = string.Format("WHERE Time >= {0} AND Time < {1} ORDER BY Time", startTime.ToUnixTimeSeconds(), endTime.ToUnixTimeSeconds());
            return await GetData<T>(filter);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public static string GetSqliteSchema(Type type)
        {
            IEnumerable<string> columnList = type.GetProperties().Select(item => string.Format("[{0}] {1}", item.Name, GetSqliteType(item.PropertyType)));
            string schema = string.Format("({0})", string.Join(",\n", columnList));
            return schema;
        }
        public static string GetSqliteType(Type type)
        {
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Byte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 => "INTEGER",
                TypeCode.Decimal or TypeCode.Double => "REAL",
                _ => "TEXT",
            };
        }
        private async Task CreateTable(Type type)
        {
            try
            {
                string tableName = type.Name;
                string schema = GetSqliteSchema(type);
                string sql = string.Format(@"CREATE TABLE IF NOT EXISTS {0} {1};", tableName, schema);
                await connection.ExecuteAsync(sql);
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, new ErrorEventArgs(e));
            }
        }
    }
}