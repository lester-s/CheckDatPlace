using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheckDatPlace.Helper;

namespace CheckDatPlace.CDPActivity
{
    [Activity(Label = "Gallery")]
    public class ImageViewerActivity : Activity
    {
        private ImageView image;
        private ImageHelper imageHelper;
        private LinearLayout imageViewerContainer;

        private string picturePath;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            imageHelper = new ImageHelper(this);

            SetContentView(Resource.Layout.ImageViewerLayout);
            imageViewerContainer = FindViewById<LinearLayout>(Resource.Id.imageViewerContainer);

            picturePath = Intent.GetStringExtra("imagePath") ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(picturePath))
            {
                await SetPicturePreviewAsync();
            }
            else
            {
                Toast.MakeText(this, "Can't recover image", ToastLength.Short).Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.imageViewerMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_DeleteImage:
                    DeleteImage();
                    break;
            }
            return base.OnMenuItemSelected(featureId, item);
        }

        private void DeleteImage()
        {
            imageHelper.DeleteImage(picturePath);
            this.OnBackPressed();
        }

        private async Task SetPicturePreviewAsync()
        {
            await Task.Run(
                () =>
                {
                    if (picturePath != null)
                    {
                        Java.IO.File imgFile = new Java.IO.File(picturePath);

                        if (imgFile.Exists())
                        {
                            image = imageHelper.PathToImageView(imgFile.AbsolutePath, false);
                        }
                    }
                }
                );

            this.RunOnUiThread(() =>
            {
                imageViewerContainer.AddView(image);
            });
        }

        public override void OnBackPressed()
        {
            Intent myIntent = new Intent(this, typeof(CreatePlaceActivity));
            SetResult(Result.Ok, myIntent);
            Finish();
            base.OnBackPressed();
        }
    }
}