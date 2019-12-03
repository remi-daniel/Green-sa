using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using GreenSa.Persistence;
using GreenSa.Droid.Persistence;

[assembly: Dependency(typeof(SQLiteDb))]

namespace GreenSa.Droid.Persistence
{
	public class SQLiteDb : ISQLiteDb
	{
        public SQLiteAsyncConnection GetConnectionAsync()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentsPath, "MySQLite.db3");

            return new SQLiteAsyncConnection(path);
        }

        SQLiteConnection ISQLiteDb.GetConnection()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentsPath, "MySQLite.db3");

            return new SQLiteConnection( path);
        }
    }
}

