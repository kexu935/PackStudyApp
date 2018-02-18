using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.Net;
using Android.Util;

namespace PackStudy
{
    [Activity(Label = "FullScreenImageView")]
    public class FullScreenImageView : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FullScreenImageView);
            string filepath = Intent.GetStringExtra("FilePath");
            ImageView image = (ImageView)FindViewById(Resource.Id.image);
            var imageBitmap = GetImageBitmapFromUrl(filepath);
            image.SetImageBitmap(imageBitmap);
            DisplayMetrics mets = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(mets);
            double viewWidthToBitmapWidthRatio = (double)mets.WidthPixels / (double)imageBitmap.Width;
            image.LayoutParameters.Height = (int)(imageBitmap.Height * viewWidthToBitmapWidthRatio); 
            // Create your application here
        }
        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}