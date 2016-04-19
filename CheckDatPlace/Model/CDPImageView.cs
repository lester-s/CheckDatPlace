using System;

using Android.Widget;

namespace CheckDatPlace.Model
{
    public class CDPImageView
    {
        public event Action<CDPImageView> Click;

        public ImageView Imageview { get; set; }
        public string ImagePath { get; set; }

        public CDPImageView(ImageView image, string path)
        {
            this.Imageview = image;
            this.ImagePath = path;
            Imageview.Click += Imageview_Click;
        }

        private void Imageview_Click(object sender, EventArgs e)
        {
            this.Click(this);
        }
    }
}