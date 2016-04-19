using System;
using System.Linq;
using Android.Content;
using CheckDatPlace.BLL;
using CheckDatPlace.Model.StaticData;

namespace CheckDatPlace.DAL.DatabaseMigrationScript
{
    public class DatabaseMigrator
    {
        private static void Migration()
        {
        }

        public static bool CheckForMigration(Context context)
        {
            BaseDal dal = new BaseDal();

            try
            {
                var lastMigrationVersion = dal.ReadAll<AppInfo>().First().Version;
                var currentVersion = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;
                return currentVersion > lastMigrationVersion;
            }
            catch (Exception)
            {
                AppInfo info = new AppInfo() { Version = 1 };
                dal.Insert<AppInfo>(info);
                DatabaseMigrator.ApplyMigration(context, true);
            }

            return false;
        }

        public static void InitDatabase()
        {
            PlaceBLL.Instance.InsertOneCategory("All");
            PlaceBLL.Instance.InsertOneCategory("Other");
            PlaceBLL.Instance.InsertOneCategory("Bar");
            PlaceBLL.Instance.InsertOneCategory("Restaurant");
            PlaceBLL.Instance.InsertOneCategory("Museum");
            PlaceBLL.Instance.InsertOneCategory("Monument");
            PlaceBLL.Instance.InsertOneCategory("People");
            PlaceBLL.Instance.InsertOneCategory("Public Garden");
            PlaceBLL.Instance.InsertOneCategory("Night club");
            PlaceBLL.Instance.InsertOneCategory("Sport club");
            PlaceBLL.Instance.InsertOneCategory("City");
        }

        public static bool ApplyMigration(Context context, bool initDatabase = false)
        {
            BaseDal dal = new BaseDal();
            Action migration = null;

            if (initDatabase)
            {
                migration = new Action(DatabaseMigrator.InitDatabase);
            }
            else
            {
                migration = new Action(DatabaseMigrator.Migration);
            }

            var result = dal.RunTransaction(migration);

            if (result)
            {
                var appInfo = dal.ReadAll<AppInfo>().First();
                var currentVersion = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;

                appInfo.Version = currentVersion;
                dal.Update<AppInfo>(appInfo);
            }

            return result;
        }
    }
}