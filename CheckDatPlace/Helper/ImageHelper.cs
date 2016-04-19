using System.IO;
using Android.Content;
using Android.Graphics;
using Android.Widget;
using CheckDatPlace.Model;

namespace CheckDatPlace.Helper
{
    public class ImageHelper
    {
        private Context _context;

        public ImageHelper(Context context)
        {
            this._context = context;
        }

        public CDPImageView PathToCDPImage(string path, bool downScale = true)
        {
            Bitmap myBitmap = BitmapFactory.DecodeFile(path);
            ImageView image = new ImageView(_context);

            if (downScale)
            {
                var bitmapScalled = Bitmap.CreateScaledBitmap(myBitmap, 400, 600, true);
                image.SetImageBitmap(bitmapScalled);
                image.SetPadding(15, 10, 15, 10);
                myBitmap.Recycle();
            }
            else
            {
                image.SetImageBitmap(myBitmap);
            }

            return new CDPImageView(image, path);
        }

        public ImageView PathToImageView(string path, bool downScale = true)
        {
            Bitmap myBitmap = BitmapFactory.DecodeFile(path);
            ImageView image = new ImageView(_context);

            if (downScale)
            {
                var bitmapScalled = Bitmap.CreateScaledBitmap(myBitmap, 400, 600, true);
                image.SetImageBitmap(bitmapScalled);
                image.SetPadding(15, 10, 15, 10);
                myBitmap.Recycle();
            }
            else
            {
                image.SetImageBitmap(myBitmap);
            }

            return image;
        }

        internal void DeleteImage(string picturePath)
        {
            if (File.Exists(picturePath))
            {
                File.Delete(picturePath);
            }
        }
    }
}