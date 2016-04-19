using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckDatPlace.Model;
using SQLite;

namespace CheckDatPlace.DAL
{
    public class BaseDal
    {
        private SQLiteAsyncConnection DBConnectionAsync
        {
            get
            {
                return DatabaseHandler.Instance.GetAsyncConnection();
            }
        }

        private SQLiteConnection DBConnection
        {
            get
            {
                return DatabaseHandler.Instance.GetConnection();
            }
        }

        #region CRUD Async

        public async Task<int> InsertAsync<T>(T newItem) where T : BaseItem, new()
        {
            await DBConnectionAsync.CreateTableAsync<T>();
            return await DBConnectionAsync.InsertAsync(newItem);
        }

        public async Task<int> DeleteAsync<T>(T deletedItem) where T : BaseItem
        {
            return await DBConnectionAsync.DeleteAsync(deletedItem);
        }

        public async Task<int> UpdateAsync<T>(T updatedItem) where T : BaseItem
        {
            return await DBConnectionAsync.UpdateAsync(updatedItem);
        }

        public async Task<List<T>> ReadAllAsync<T>() where T : BaseItem, new()
        {
            return await DBConnectionAsync.Table<T>().ToListAsync();
        }

        public async Task<List<T>> ReadOneByParameterAsync<T>(string parmeterName, object parameterValue) where T : BaseItem, new()
        {
            var typeName = typeof(T).Name;
            //var result = await DBConnectionAsync.QueryAsync<T>("Select * from ? where ? = ?", typeName, parmeterName, parameterValue);
            var result = await DBConnectionAsync.QueryAsync<T>("Select * from Person where FirstName = Simon");
            return result;
        }

        #endregion CRUD Async

        #region CRUD sync

        public int Insert<T>(T newItem) where T : BaseItem, new()
        {
            int result;
            using (DBConnection)
            {
                DBConnection.CreateTable<T>();
                result = DBConnection.Insert(newItem);
            }
            return result;
        }

        public int Delete<T>(T deletedItem) where T : BaseItem
        {
            int result;
            using (DBConnection)
            {
                result = DBConnection.Delete(deletedItem);
            }
            return result;
        }

        public int Update<T>(T updatedItem) where T : BaseItem
        {
            int result;
            using (DBConnection)
            {
                result = DBConnection.Update(updatedItem);
            }
            return result;
        }

        public List<T> ReadAll<T>() where T : BaseItem, new()
        {
            List<T> result = new List<T>();
            using (DBConnection)
            {
                try
                {
                    var places = DBConnection.Table<T>();
                    result = places.ToList();
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

        public List<T> ReadOneByParameter<T>(string parameterName, object parameterValue) where T : BaseItem, new()
        {
            List<T> result = null;
            using (DBConnection)
            {
                var typeName = typeof(T).Name;
                result = DBConnection.Query<T>(string.Format("Select * from {0} where {1} = ?", typeName, parameterName), parameterValue);
            }
            return result;
        }

        #endregion CRUD sync

        public bool RunTransaction(Action transaction)
        {
            using (DBConnection)
            {
                try
                {
                    DBConnection.RunInTransaction(transaction);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        public bool ClearTable<T>() where T : BaseItem, new()
        {
            using (DBConnection)
            {
                var rowCount = DBConnection.Table<T>().Count();
                var deleteCount = DBConnection.DeleteAll<T>();
                return rowCount == deleteCount;
            }
        }
    }
}