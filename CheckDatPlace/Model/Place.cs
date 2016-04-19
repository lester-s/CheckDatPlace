using SQLite;

namespace CheckDatPlace.Model
{
    public class Place : BaseItem
    {
        [NotNull]
        public string Name { get; set; }

        public float Grade { get; set; }
        public string Comment { get; set; }

        [NotNull]
        public string Address { get; set; }

        public int PlaceCategoryId { get; set; }

        [NotNull]
        public string PicturesFolderPath { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}