using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheckDatPlace.Adapter;
using CheckDatPlace.BLL;
using CheckDatPlace.CDPActivity;
using CheckDatPlace.Helper;
using CheckDatPlace.Model;
using CheckDatPlace.Model.StaticData;
using Newtonsoft.Json;
using Xamarin.Media;

namespace CheckDatPlace
{
    [Activity(Label = "Create", MainLauncher = false)]
    public class CreatePlaceActivity : Activity
    {
        #region properties

        private bool _isEditing = false;

        private bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                if (value)
                {
                    this.Title = "Update place";
                }
                else
                {
                    this.Title = "Create place";
                }
                _isEditing = value;
            }
        }

        private EditText placeName;
        private EditText placeComment;
        private RatingBar placeGrade;
        private Button validateButton;
        private Spinner placeTypeSpinner;
        private Place _editedPlace;
        private AutoCompleteTextView autoCompleteAddress;
        private LinearLayout horizontalImageViewer;
        private Button gpsButton;
        private ImageHelper imageHelper;
        private ProgressBar gpsProgressBar;

        #endregion properties

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CreatePlaceLayout);

            #region visual component recovery

            placeName = FindViewById<EditText>(Resource.Id.PlaceName);
            placeComment = FindViewById<EditText>(Resource.Id.PlaceComment);
            validateButton = FindViewById<Button>(Resource.Id.AddPlaceButton);
            placeGrade = FindViewById<RatingBar>(Resource.Id.PlaceGrade);
            placeTypeSpinner = FindViewById<Spinner>(Resource.Id.placeTypeSpinner);
            autoCompleteAddress = FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteAddress);
            horizontalImageViewer = FindViewById<LinearLayout>(Resource.Id.horizontalImageViewer);
            gpsButton = FindViewById<Button>(Resource.Id.gpsButton);
            gpsProgressBar = FindViewById<ProgressBar>(Resource.Id.gpsProgressBar);

            #endregion visual component recovery

            await InitActivity();

            //view events
            validateButton.Click += ValidateButton_Click;
            autoCompleteAddress.ItemClick += autoCompleteAddress_ItemClick;
            autoCompleteAddress.TextChanged += autoCompleteAddress_TextChanged;
            gpsButton.Click += gpsButton_Click;
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Canceled)
                return;
            switch (requestCode)
            {
                case (int)Constants.ActivityRequestCode.ImageViewer:
                    horizontalImageViewer.RemoveAllViews();
                    await SetPicturePreviewAsync();
                    break;

                case (int)Constants.ActivityRequestCode.Camera:
                    await data.GetMediaFileExtraAsync(this).ContinueWith(t =>
                    {
                        var image = imageHelper.PathToCDPImage(t.Result.Path);
                        image.Click += image_Click;
                        horizontalImageViewer.AddView(image.Imageview);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    break;

                default:
                    break;
            }
        }

        private async Task InitActivity()
        {
            imageHelper = new ImageHelper(this);

            gpsProgressBar.Visibility = ViewStates.Invisible;

            var placeCategory = PlaceBLL.Instance.GetAllPlaceCategory().Where(p => p.ID != Constants.AllCategoryId).ToArray();
            PlaceCategoryAdapter spinnerArrayAdapter = new PlaceCategoryAdapter(this, placeCategory);
            placeTypeSpinner.Adapter = spinnerArrayAdapter;

            string editedPlaceJson = Intent.GetStringExtra("EditedPlace") ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(editedPlaceJson))
            {
                _editedPlace = JsonConvert.DeserializeObject<Place>(editedPlaceJson);
                IsEditing = true;
                this.FillFormForEditing();
                await SetPicturePreviewAsync();
            }
            else
            {
                IsEditing = false;
                _editedPlace = new Place();
                if (!PlaceBLL.Instance.SetPicturesFolder(_editedPlace))
                {
                    Toast.MakeText(this, "Error! Can't create pictures folder.", ToastLength.Short);
                }
            }
        }

        private void FillFormForEditing()
        {
            placeName.Text = _editedPlace.Name;
            placeGrade.Rating = _editedPlace.Grade;
            autoCompleteAddress.Text = _editedPlace.Address;
            placeComment.Text = _editedPlace.Comment;

            var cate = PlaceBLL.Instance.GetAllPlaceCategory().Where(c => c.ID != Constants.AllCategoryId).ToList();
            var catePosition = cate.FindIndex(c => c.ID == _editedPlace.PlaceCategoryId);
            placeTypeSpinner.SetSelection(catePosition);
        }

        #region menu

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.updatePlaceLayoutMenu, menu);

            menu.FindItem(Resource.Id.menu_ShowMap).SetVisible(IsEditing);
            menu.FindItem(Resource.Id.menu_camera).SetVisible(IsEditing);
            menu.FindItem(Resource.Id.menu_deletePlace).SetVisible(IsEditing);
            menu.FindItem(Resource.Id.menu_Share).SetVisible(IsEditing);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_camera:
                    LaunchCamera();
                    break;

                case Resource.Id.menu_ShowMap:
                    LaunchMapActivity();
                    break;

                case Resource.Id.menu_deletePlace:
                    DeletePlace();
                    break;

                case Resource.Id.menu_Share:

                    break;

                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion menu

        #region camera

        private void LaunchCamera()
        {
            var picker = new MediaPicker(this);
            if (!picker.IsCameraAvailable)
                Console.WriteLine("No camera!");
            else
            {
                string picturesPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                string path2 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                var folder = System.IO.Path.Combine(picturesPath, _editedPlace.PicturesFolderPath);
                var intent = picker.GetTakePhotoUI(new StoreCameraMediaOptions
                {
                    Name = "picture.jpg",
                    Directory = _editedPlace.PicturesFolderPath
                });
                StartActivityForResult(intent, (int)Constants.ActivityRequestCode.Camera);
            }
        }

        #endregion camera

        #region component events

        private void autoCompleteAddress_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            autoCompleteAddress.TextChanged -= autoCompleteAddress_TextChanged;
        }

        private async void autoCompleteAddress_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var st = sender.GetType();
            var results = await PlaceBLL.Instance.GetLocationFromAddress(this, e.Text.ToString(), 10);
            string[] test = new string[results.Count];
            for (int i = 0; i < results.Count; i++)
            {
                var item = results.ElementAt(i);
                test[i] = item.SubThoroughfare + " " + item.Thoroughfare + " " + item.PostalCode + " " + item.Locality;
            }
            ArrayAdapter autoCompleteAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, test);
            autoCompleteAddress.Adapter = autoCompleteAdapter;
            autoCompleteAddress.ShowDropDown();
        }

        private void ValidateButton_Click(object sender, EventArgs e)
        {
            if (!ValidatePlaceForm())
            {
                return;
            }
            var position = placeTypeSpinner.SelectedItemPosition;
            var cate = PlaceBLL.Instance.GetAllPlaceCategory().Where(p => p.ID != Constants.AllCategoryId).ElementAt(position);
            if (!IsEditing)
            {
                _editedPlace.Address = autoCompleteAddress.Text;
                _editedPlace.Comment = placeComment.Text;
                _editedPlace.Grade = placeGrade.Rating;
                _editedPlace.Name = placeName.Text;
                _editedPlace.PlaceCategoryId = cate.ID;

                var createResult = PlaceBLL.Instance.InsertOnePlace(_editedPlace);

                if (createResult)
                {
                    Toast.MakeText(this, "Place added with success", ToastLength.Short).Show();
                    autoCompleteAddress.Text = string.Empty;
                    placeComment.Text = string.Empty;
                    placeName.Text = string.Empty;
                    placeGrade.Rating = 0;
                    _editedPlace = new Place();
                    if (!PlaceBLL.Instance.SetPicturesFolder(_editedPlace))
                    {
                        Toast.MakeText(this, "Error! Can't create pictures folder.", ToastLength.Short);
                    }
                }
                else
                {
                    Toast.MakeText(this, "Error, place not created", ToastLength.Short).Show();
                }
            }
            else
            {
                _editedPlace.Address = autoCompleteAddress.Text;
                _editedPlace.Comment = placeComment.Text;
                _editedPlace.Grade = placeGrade.Rating;
                _editedPlace.Name = placeName.Text;
                _editedPlace.PlaceCategoryId = cate.ID;

                var updateResult = PlaceBLL.Instance.UpdateOnePlace(_editedPlace);

                if (updateResult)
                {
                    Toast.MakeText(this, "Place updated with success", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "Error, place not updated", ToastLength.Short).Show();
                }
            }

            this.OnBackPressed();
        }

        private bool ValidatePlaceForm()
        {
            bool result = true;
            if (string.IsNullOrWhiteSpace(placeName.Text))
            {
                result = false;
                Toast.MakeText(this, "A name is needed !", ToastLength.Long).Show();
            }

            if (string.IsNullOrWhiteSpace(autoCompleteAddress.Text))
            {
                result = false;
                Toast.MakeText(this, "An address is needed !", ToastLength.Long).Show();
            }

            return result;
        }

        private async void gpsButton_Click(object sender, EventArgs e)
        {
            ((Button)sender).Visibility = ViewStates.Invisible;
            gpsProgressBar.Visibility = ViewStates.Visible;

            var address = await PlaceBLL.Instance.GetAddressFromGPS(this);

            if (address != null)
            {
                RunOnUiThread(() =>
                {
                    autoCompleteAddress.Text = address.SubThoroughfare + " " + address.Thoroughfare + " " + address.PostalCode + " " + address.Locality;
                    _editedPlace.Latitude = address.Latitude;
                    _editedPlace.Longitude = address.Longitude;
                });
            }
            ((Button)sender).Visibility = ViewStates.Visible;
            gpsProgressBar.Visibility = ViewStates.Invisible;
        }

        private void DeletePlace()
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Confirm delete");
            builder.SetMessage("Are you sure you want to delete this place ?");
            builder.SetPositiveButton("Yes", (sender, args) => { DeletePlaceExecute(); });
            builder.SetNegativeButton("No", (sender, args) => { });
            builder.SetCancelable(false);
            builder.Show();
        }

        private void DeletePlaceExecute()
        {
            bool result = PlaceBLL.Instance.DeleteOnePlace(_editedPlace);
            var resultMessage = string.Empty;
            if (result)
            {
                resultMessage = "Delete success !";
                Toast.MakeText(this, resultMessage, ToastLength.Short);
                this.OnBackPressed();
            }
            else
            {
                resultMessage = "Delete fail !";
                Toast.MakeText(this, resultMessage, ToastLength.Short);
            }
        }

        #endregion component events

        #region pictures

        private void SetPicturePreview()
        {
            var picture = PlaceBLL.Instance.getPlacePictures(_editedPlace.PicturesFolderPath);

            if (picture != null)
            {
                for (var i = 0; i < picture.Length; i++)
                {
                    Java.IO.File imgFile = new Java.IO.File(picture[i]);

                    if (imgFile.Exists())
                    {
                        Bitmap myBitmap = BitmapFactory.DecodeFile(imgFile.AbsolutePath);
                        var bitmapScalled = Bitmap.CreateScaledBitmap(myBitmap, 300, 400, true);
                        ImageView image = new ImageView(this);
                        image.SetImageBitmap(bitmapScalled);
                        horizontalImageViewer.AddView(image);
                        myBitmap.Recycle();
                    }
                }
            }
        }

        private async Task SetPicturePreviewAsync()
        {
            List<ImageView> images = new List<ImageView>();
            await Task.Run(
                () =>
                {
                    var picture = PlaceBLL.Instance.getPlacePictures(_editedPlace.PicturesFolderPath);

                    if (picture != null)
                    {
                        for (var i = 0; i < picture.Length; i++)
                        {
                            Java.IO.File imgFile = new Java.IO.File(picture[i]);

                            if (imgFile.Exists())
                            {
                                var image = imageHelper.PathToCDPImage(imgFile.AbsolutePath);
                                images.Add(image.Imageview);
                                image.Click += image_Click;
                            }
                        }
                    }
                }
                );

            foreach (var image in images)
            {
                this.RunOnUiThread(() =>
                    {
                        horizontalImageViewer.AddView(image);
                    });
            }
        }

        private void image_Click(CDPImageView image)
        {
            var imageActivity = new Intent(this, typeof(ImageViewerActivity));
            imageActivity.PutExtra("imagePath", image.ImagePath);
            StartActivityForResult(imageActivity, (int)Constants.ActivityRequestCode.ImageViewer);
        }

        #endregion pictures

        private void LaunchMapActivity()
        {
            var mapIntent = new Intent(this, typeof(MapViewActivity));
            string[] adresses = new string[] { _editedPlace.Address };
            mapIntent.PutExtra("EditedAddress", adresses);
            StartActivity(mapIntent);
        }
    }
}