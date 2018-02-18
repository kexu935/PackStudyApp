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
    [Activity(Label = "Google Doc Description")]
    public class googleDocDescription : Activity
    {
        // Grab edit text content
        EditText etDescription;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set layout
            SetContentView(Resource.Layout.googleDocDescriptionLayout);
            // Connect button
            Button btnSelectStudents = (Button)FindViewById(Resource.Id.btnSelectStudents);
            // Create method
            btnSelectStudents.Click += btnSelectStudents_Click;


        }

        private void btnSelectStudents_Click(object sender, EventArgs args)
        {
            // Grab google doc Description
            etDescription = (EditText)FindViewById(Resource.Id.etDescription);

            // Send to correct message board
            Intent activityIntent = new Intent(this, typeof(SelectStudents));

            // Grab extra and send over
            string currentCourse = Intent.GetStringExtra("Course");
            activityIntent.PutExtra("Description", etDescription.Text);
            activityIntent.PutExtra("Course", currentCourse);

            // And finally, start the activity. Again, we are using the
            // context here.
            StartActivity(activityIntent);

        }
    }
}