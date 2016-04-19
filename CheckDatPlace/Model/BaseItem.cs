using SQLite;

namespace CheckDatPlace.Model
{
    public class BaseItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
    }
}