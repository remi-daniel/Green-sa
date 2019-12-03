using SQLite;

namespace GreenSa.Persistence
{
    public interface ISQLiteDb
    {
        SQLiteConnection GetConnection();
        SQLiteAsyncConnection GetConnectionAsync();

    }
}

