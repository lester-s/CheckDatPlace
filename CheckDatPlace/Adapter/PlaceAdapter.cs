using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using CheckDatPlace.Model;

namespace CheckDatPlace.Adapter
{
    public class PlaceAdapter : BaseAdapter<Place>
    {
        private Place[] items;
        private Activity activity;
        private Context context;

        public PlaceAdapter(Activity context, Place[] items)
            : base()
        {
            this.activity = context;
            this.items = items;
        }

        public PlaceAdapter(Activity activity, Context context, Place[] items)
        {
            this.context = context;
            this.activity = activity;
            this.items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Place this[int position]
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
                    view = activity.LayoutInflater.Inflate(Android.Resource.Layout.TwoLineListItem, null);
                }
            }
            else
            {
                if (view == null)
                {// otherwise create a new one
                    LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Service.LayoutInflaterService);
                    view = inflater.Inflate(Android.Resource.Layout.TwoLineListItem, null);
                }
            }

            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position].Name;
            view.FindViewById<TextView>(Android.Resource.Id.Text1).TextSize = 30;
            view.FindViewById<TextView>(Android.Resource.Id.Text1).SetPadding(40, 10, 0, 5);
            view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = items[position].Address;
            view.FindViewById<TextView>(Android.Resource.Id.Text2).SetPadding(40, 5, 0, 5);

            return view;
        }
    }
}