namespace CheckDatPlace.DAL
{
    public class PlaceDal : BaseDal
    {
        private static PlaceDal instance;

        private PlaceDal()
        {
        }

        public static PlaceDal Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PlaceDal();
                }
                return instance;
            }
        }
    }
}