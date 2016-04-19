using SQLite;

namespace CheckDatPlace.Model.StaticData
{
    public class PlaceCategory : BaseItem
    {
        [NotNull, Unique]
        public string Name { get; set; }
    }
}