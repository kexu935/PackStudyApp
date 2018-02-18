using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Text;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PackStudy
{
    [Activity(Label = "Class Home Page")]
    public class selectedClassHomePage : Activity
    {
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set layout
            SetContentView(Resource.Layout.selectedClassHomePageLayout);

            // Connect buttons
            Button btnClassMessages = (Button)FindViewById(Resource.Id.btnClassMessages);
            Button btnCreateGoogleDoc = (Button)FindViewById(Resource.Id.btnCreateGoogleDoc);
            Button btnCreateStudyGroup = (Button)FindViewById(Resource.Id.btnCreateStudyGroup);

            string currentCourse = Intent.GetStringExtra("Course");
            Title = currentCourse +" - Home Page";
            // methods

            btnClassMessages.Click += btnClassMessages_Click;
            btnCreateGoogleDoc.Click += btnCreateGoogleDoc_Click;
            btnCreateStudyGroup.Click += BtnCreateStudyGroup_Click;


            WebClient client = new WebClient();
            Uri uri = new Uri("http://packstudy-com.stackstaging.com/getstudysessions.php");
            NameValueCollection parameter = new NameValueCollection();

            ISharedPreferences sharedPrefrences = GetSharedPreferences("MyData", FileCreationMode.Private);
            int id = sharedPrefrences.GetInt("id", 0);

            parameter.Add("UserId", id.ToString());
            parameter.Add("ClassId", currentCourse);

            byte[] returnValue = client.UploadValues(uri, parameter);
            string r = Encoding.ASCII.GetString(returnValue);

            List<Topic> topic = JsonConvert.DeserializeObject<List<Topic>>(r);

            //name = Intent.GetStringExtra("StudyGroupName");
            LinearLayout ll = (LinearLayout)FindViewById(Resource.Id.ssll);

            int counterId = 0;
            if( topic != null)
            {

                foreach (Topic s in topic)
                {
                    Button btn = new Button(this) { Text = s.SessionTitle } ;

                    //btn.Text = s.SessionTitle;
                    btn.Id = counterId;
                    counterId++;
                    //btn.SetBackgroundResource(Resource.Drawable.CircleButton);

                    btn.Click += new EventHandler(this.button_Click);

                    ll.AddView(btn);

                }
            }

        }

        private void button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            // Send to correct message board
            Intent activityIntent = new Intent(this, typeof(MessageBoard));

            // Grab extra and send over
            string currentCourse = Intent.GetStringExtra("Course");
            activityIntent.PutExtra("Course", currentCourse + "_" + btn.Text);
            
            // And finally, start the activity. Again, we are using the
            // context here.
            StartActivity(activityIntent);
        }

        private void BtnCreateStudyGroup_Click(object sender, EventArgs e)
        {
            // Send to correct message board
            Intent activityIntent = new Intent(this, typeof(StudyGroups));

            // Grab extra and send over
            string currentCourse = Intent.GetStringExtra("Course");
            activityIntent.PutExtra("Course", currentCourse);

            // And finally, start the activity. Again, we are using the
            // context here.
            StartActivity(activityIntent);
        }

        private void btnCreateGoogleDoc_Click(object sender, EventArgs args)
        {
            // Send to correct message board
            Intent activityIntent = new Intent(this, typeof(googleDocDescription));

            // Grab extra and send over
            string currentCourse = Intent.GetStringExtra("Course");
            activityIntent.PutExtra("Course", currentCourse);

            // And finally, start the activity. Again, we are using the
            // context here.
            StartActivity(activityIntent);
        }

        private void btnClassMessages_Click(object sender, EventArgs args)
        {
            // Send to correct message board
            Intent activityIntent = new Intent(this, typeof(MessageBoard));

            // Grab extra and send over
            string currentCourse = Intent.GetStringExtra("Course");
            activityIntent.PutExtra("Course", currentCourse);

            // And finally, start the activity. Again, we are using the
            // context here.
            StartActivity(activityIntent);
        }
    }
}