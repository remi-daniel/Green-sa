using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using GreenSa.Windows.Persistence;
using GreenSa.Persistence;
using Windows.Storage;

[assembly: Dependency(typeof(SQLiteDb))]
namespace GreenSa.Windows.Persistence
{
    public class SQLiteDb : ISQLiteDb
    {
        SQLite.SQLiteConnection ISQLiteDb.GetConnection()
        {
            var documentsPath = ApplicationData.Current.LocalFolder.Path;
            var path = Path.Combine(documentsPath, "MySQLite.db3");
            return new SQLiteConnection( path);
        }
    }
}

