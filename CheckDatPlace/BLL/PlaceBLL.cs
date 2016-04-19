using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.Widget;
using CheckDatPlace.Adapter;
using CheckDatPlace.DAL;
using CheckDatPlace.Model;
using CheckDatPlace.Model.StaticData;
using Plugin.Geolocator;

namespace CheckDatPlace.BLL
{
    public class PlaceBLL
    {
        private static PlaceBLL instance;

        private PlaceBLL()
        {
            IsAllPlacesDirty = true;
        }

        public static PlaceBLL Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PlaceBLL();
                }
                return instance;
            }
        }

        public bool IsAllPlacesDirty { get; set; }

        private List<Place> _allPlaces;

        #region CRUD

        public bool InsertOnePlace(Place newPlace)
        {
            var result = PlaceDal.Instance.Insert<Place>(newPlace) == 1;
            IsAllPlacesDirty = result ? true : IsAllPlacesDirty;
            return result;
        }

        public bool UpdateOnePlace(Place updatedPlace)
        {
            var result = PlaceDal.Instance.Update<Place>(updatedPlace) == 1;
            IsAllPlacesDirty = result ? true : IsAllPlacesDirty;
            return result;
        }

        public List<Place> ReadAllPlace()
        {
            if (IsAllPlacesDirty)
            {
                _allPlaces = PlaceDal.Instance.ReadAll<Place>();
            }
            return _allPlaces;
        }

        #endregion CRUD

        #region location

        async internal Task<List<Address>> GetLocationFromAddress(Context context, string address, int maxResult)
        {
            var geo = new Geocoder(context);
            //Android.Gms.Location.Places.
            List<Address> result = new List<Address>();
            try
            {
                var addresses = await geo.GetFromLocationNameAsync(address, maxResult);
                result = addresses.ToList<Address>();
            }
            catch (Exception)
            {
                Toast.MakeText(context, "Cannot recover addresses !", ToastLength.Short).Show();
            }
            return result;
        }

        async internal Task<List<Address>> GetLocationFromAddress(Context context, int maxResult, params string[] address)
        {
            List<Address> result = new List<Address>();
            foreach (var ad in address)
            {
                if (string.IsNullOrWhiteSpace(ad))
                {
                    continue;
                }
                var geo = new Geocoder(context);
                try
                {
                    var addresses = await geo.GetFromLocationNameAsync(ad, maxResult);
                    result.AddRange(addresses);
                }
                catch (Exception)
                {
                    Toast.MakeText(context, "Cannot recover addresses !", ToastLength.Short).Show();
                }
            }

            return result;
        }

        public async Task<Address> GetAddressFromGPS(Context context)
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            if (!locator.IsGeolocationEnabled)
            {
                Toast.MakeText(context, "GPS disable", ToastLength.Long).Show();
                return null;
            }
            else
            {
                var position = await locator.GetPositionAsync(100000);
                Geocoder geocoder = new Geocoder(context);
                IList<Address> addressList = await geocoder.GetFromLocationAsync(position.Latitude, position.Longitude, 10);

                Address address = addressList.FirstOrDefault();
                return address;
            }
        }

        #endregion location

        #region Category

        public List<PlaceCategory> GetAllPlaceCategory()
        {
            BaseDal dal = new BaseDal();
            return dal.ReadAll<PlaceCategory>();
        }

        public bool InsertOneCategory(string name)
        {
            PlaceCategory newCategory = new PlaceCategory() { Name = name };
            return PlaceDal.Instance.Insert<PlaceCategory>(newCategory) == 1;
        }

        #endregion Category

        #region Pictures

        internal bool SetPicturesFolder(Place _editedPlace)
        {
            string picturesPath = Constants.PicturePath;
            int directoryId = 0;
            try
            {
                directoryId = Directory.GetDirectories(picturesPath).Length;
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (Exception)
            {
                return false;
            }

            var directoryPath = string.Concat(picturesPath, "/", directoryId);
            var info = Directory.CreateDirectory(directoryPath);
            if (info.Exists)
            {
                _editedPlace.PicturesFolderPath = directoryId.ToString(); ;
                return true;
            }
            else
            {
                return false;
            }
        }

        internal string[] getPlacePictures(string p)
        {
            string picturesPath = Constants.PicturePath;
            string[] path = new string[0];
            try
            {
                var pictureFolder = Path.Combine(picturesPath, p);
                if (Directory.Exists(pictureFolder))
                {
                    var en = Directory.EnumerateFiles(pictureFolder);
                    var paths = Directory.GetFiles(pictureFolder);
                    path = paths;
                }
            }
            catch (Exception)
            {
                return null;
            }

            return path;
        }

        #endregion Pictures

        public Place[] FilterPlaces(float rate, PlaceCategory category, string filterValue)
        {
            var places = this.ReadAllPlace();
            if (string.IsNullOrWhiteSpace(filterValue))
            {
                if (category.ID == Constants.AllCategoryId)
                {
                    return places.Where(p => p.Grade >= rate).ToArray();
                }
                else
                {
                    return places.Where(p => p.Grade >= rate && p.PlaceCategoryId == category.ID).ToArray();
                }
            }
            else if (category.ID == Constants.AllCategoryId)
            {
                return places.Where(p => p.Grade >= rate &&
                    (p.Name.ToUpper().Contains(filterValue.ToUpper()) || p.Comment.ToUpper().Contains(filterValue.ToUpper()) || p.Address.ToUpper().Contains(filterValue.ToUpper()))).ToArray();
            }
            else
            {
                return places.Where(p => p.Grade >= rate && p.PlaceCategoryId == category.ID &&
                    (p.Name.ToUpper().Contains(filterValue.ToUpper()) || p.Comment.ToUpper().Contains(filterValue.ToUpper()) || p.Address.ToUpper().Contains(filterValue.ToUpper()))).ToArray();
            }
        }

        internal bool DeleteOnePlace(Place place)
        {
            return PlaceDal.Instance.Delete<Place>(place) == 1;
        }

        internal Adapter.PlaceCategoryAdapter GetPlaceCategoryAdapter(Activity context)
        {
            var placeCategory = PlaceBLL.Instance.GetAllPlaceCategory();
            return new PlaceCategoryAdapter(context, placeCategory.ToArray());
        }
    }
}