using SQLite;

namespace CheckDatPlace.Model.StaticData
{
    public class AppInfo : BaseItem
    {
        [NotNull]
        public float Version { get; set; }
    }
}