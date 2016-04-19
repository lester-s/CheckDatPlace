using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using CheckDatPlace.Model.StaticData;

namespace CheckDatPlace.Adapter
{
    public class PlaceCategoryAdapter : BaseAdapter<PlaceCategory>
    {
        private PlaceCategory[] items;
        private Activity activity;
        private Context context;

        public PlaceCategoryAdapter(Activity activity, PlaceCategory[] items)
            : base()
        {
            this.activity = activity;
            this.items = items;
        }

        public PlaceCategoryAdapter(Activity activity, Context context, PlaceCategory[] items)
        {
            this.context = context;
            this.activity = activity;
            this.items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override PlaceCategory this[int position]
        {
            get { return items[position]; }
        }

        public override int Count
        {
            get { return items.Length; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (context == null)
            {
                if (view == null)
                {// otherwise create a new one
                    view = activity.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
                }

                view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position].Name;
            }
            else
            {
                if (view == null)
                {// otherwise create a new one
                    LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Service.LayoutInflaterService);
                    view = inflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
                }
                view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position].Name;
            }
            return view;
        }
    }
}