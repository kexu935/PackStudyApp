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

namespace PackStudy
{
    [Activity(Label = "ChangeUserPhoto")]
    public class ChangeUserPhoto : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ChangeUserPhoto);

            // Connect layout
            ImageView myImageView = (ImageView)FindViewById(Resource.Id.myImageView);
            Button myButton = (Button)FindViewById(Resource.Id.myButton);
            Button btnSettings = (Button)FindViewById(Resource.Id.btnSettings);

            // Create method for selecting image
            myButton.Click += myButton_Click;
            btnSettings.Click += btnSettings_Click;
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            Intent activityIntent = new Intent(this, typeof(EditUserProfile));
            StartActivity(activityIntent);
        }

        private void myButton_Click(object sender, EventArgs args)
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(
                Intent.CreateChooser(imageIntent, "Select photo"), 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                var imageView =
                    FindViewById<ImageView>(Resource.Id.myImageView);
                imageView.SetImageURI(data.Data);
            }
        }
    }

}