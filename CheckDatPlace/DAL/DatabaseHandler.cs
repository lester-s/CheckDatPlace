using System.IO;
using SQLite;

namespace CheckDatPlace.DAL
{
    public class DatabaseHandler
    {
        private static string sqliteFilename = "CDPDatabase.db3";

        private string libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

        private static DatabaseHandler instance;

        private DatabaseHandler()
        {
        }

        public static DatabaseHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseHandler();
                }
                return instance;
            }
        }

        public SQLiteAsyncConnection GetAsyncConnection()
        {
            var dataBasePath = Path.Combine(libraryPath, sqliteFilename);
            return new SQLiteAsyncConnection(dataBasePath);
        }

        public SQLiteConnection GetConnection()
        {
            var dataBasePath = Path.Combine(libraryPath, sqliteFilename);
            return new SQLiteConnection(dataBasePath);
        }
    }
}