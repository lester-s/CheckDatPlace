namespace CheckDatPlace.DAL
{
    public class PersonDal : BaseDal
    {
        private static PersonDal instance;

        private PersonDal()
        {
        }

        public static PersonDal Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PersonDal();
                }
                return instance;
            }
        }
    }
}