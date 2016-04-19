using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Widget;
using CheckDatPlace.BLL;

namespace CheckDatPlace
{
    [Activity(Label = "Map")]
    public class MapViewActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MapViewLayout);

            string[] addressString = Intent.GetStringArrayExtra("EditedAddress") ?? new string[0];

            if (addressString.Length <= 0)
            {
                Toast.MakeText(this, "There is no places to show !", ToastLength.Short).Show();
            }
            else
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                BuildMap(addressString);
            }
        }

        private async void BuildMap(string[] addressString)
        {
            MapFragment mapFrag = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            GoogleMap map = mapFrag.Map;

            if (map != null)
            {
                map.MoveCamera(CameraUpdateFactory.NewLatLng(new LatLng(0, 0)));
            }

            List<MarkerOptions> markers = new List<MarkerOptions>();

            if (addressString.Length != 0)
            {
                var locations = await PlaceBLL.Instance.GetLocationFromAddress(this, 1, addressString);

                if (locations != null && locations.Count > 0)
                {
                    var i = 0;
                    foreach (var loc in locations)
                    {
                        markers.Add(BuildMarkerFromLocation(loc, addressString[i]));
                        i++;
                    }
                }
            }

            this.RunOnUiThread(() =>
            {
                if (map != null && markers != null && markers.Count > 0)
                {
                    foreach (var mark in markers)
                    {
                        map.AddMarker(mark);
                    }

                    if (markers.Count == 1)
                    {
                        CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                        builder.Target(markers.ElementAt(0).Position);
                        builder.Zoom(14);
                        CameraPosition cameraPosition = builder.Build();
                        CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                        map.MoveCamera(cameraUpdate);
                    }
                    else
                    {
                        map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(GetBoundsForMap(markers), 250, 250, 0));
                    }
                }
            });
        }

        private LatLngBounds GetBoundsForMap(List<MarkerOptions> markers)
        {
            var latOrdered = markers.OrderByDescending(m => m.Position.Latitude);
            var latMin = latOrdered.Last().Position.Latitude;
            var latMax = latOrdered.First().Position.Latitude;

            var lngOrdered = markers.OrderByDescending(m => m.Position.Longitude);
            var lngMin = lngOrdered.Last().Position.Longitude;
            var lngMax = lngOrdered.First().Position.Longitude;
            LatLng sw = new LatLng(latMin, lngMin);
            LatLng ne = new LatLng(latMax, lngMax);
            return new LatLngBounds(sw, ne);
        }

        private MarkerOptions BuildMarkerFromLocation(Address location, string address)
        {
            var latLng = new LatLng(location.Latitude, location.Longitude);
            MarkerOptions markerOpt = new MarkerOptions();
            markerOpt.SetPosition(latLng);
            markerOpt.SetTitle(address);
            return markerOpt;
        }
    }
}