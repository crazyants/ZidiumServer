using System;
using System.Configuration;
using Zidium.Core.Api;

namespace Zidium.Core.ConfigDb
{
    public class DatabaseService : IDatabaseService
    {
        public DatabaseInfo[] GetDatabases()
        {
            return new[] { MainDatabase };
        }

        public DatabaseInfo GetOneOrNullById(Guid databaseId)
        {
            return MainDatabase;
        }

        public DatabaseInfo GetOneById(Guid databaseId)
        {
            return MainDatabase;
        }

        public void SetIsBroken(Guid id, bool isBroken)
        {
        }

        public static void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static string ConnectionString
        {
            get { return _connectionString ?? ConfigurationManager.ConnectionStrings["DbContext"].ConnectionString; }
        }

        private static string _connectionString;

        public static DatabaseInfo MainDatabase
        {
            get
            {
                if (_databaseInfo == null)
                    _databaseInfo = new DatabaseInfo()
                    {
                        Id = new Guid("22222222-2222-2222-2222-222222222222"),
                        SystemName = "System",
                        DisplayName = "Main database",
                        ConnectionString = ConnectionString,
                        Description = null,
                        IsBroken = false
                    };
                return _databaseInfo;
            }
        }

        private static DatabaseInfo _databaseInfo;

    }
}
