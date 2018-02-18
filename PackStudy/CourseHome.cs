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
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace PackStudy
{
    [Activity(Label = "Course Home")]
    public class CourseHome : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            LinearLayout ll = new LinearLayout(this);
            ll.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams llp = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            SetContentView(ll, llp);
            LinearLayout.LayoutParams lpview = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            int counter = 0;
            WebClient client = new WebClient();
            Uri uri = new Uri("http://packstudy-com.stackstaging.com/getCourses.php");
            NameValueCollection parameter = new NameValueCollection();
            ISharedPreferences sharedPrefrences = GetSharedPreferences("MyData", FileCreationMode.Private);

            int UserId = sharedPrefrences.GetInt("id", 0);
            parameter.Add("UserId", UserId.ToString());
            byte[] returnValue = client.UploadValues(uri, parameter);
            string r = Encoding.ASCII.GetString(returnValue);
            List<Course> CourseListDB = JsonConvert.DeserializeObject<List<Course>>(r);
            string classes= "";
            if(CourseListDB == null)
            {

            }
            else
            {
                foreach (Course c in CourseListDB)
                {
                    Button btn = new Button(this);
                    //btn.Text = c.semester + " " + c.id;
                    if (classes.Contains(c.id))
                    {
                        //don't display
                    }
                    else
                    {
                        classes += c.id;
                        btn.Text = c.id;
                        btn.Id = counter;
                        //btn.SetBackgroundResource(Resource.Drawable.CircleButton);
                        counter += 1;
                        btn.Click += new EventHandler(this.button_Click);
                        ll.AddView(btn, lpview);
                    }

                }
            }
            
            //Button btn1 = new Button(this);
            //btn1.Text = "ha";
            //ll.AddView(btn1, lpview);
            Button btnGoBack = new Button(this);
            btnGoBack.Text = "Go Back";
            ll.AddView(btnGoBack, lpview);
            btnGoBack.Click += BtnGoBack_Click;

            Button btnProfileSettings = new Button(this);
            btnProfileSettings.Text = "Edit Profile";
            ll.AddView(btnProfileSettings, lpview);
            btnProfileSettings.Click += btnProfileSettings_Click;
        }
        
        private void btnProfileSettings_Click(object sender, EventArgs e)
        {
            Intent activityIntent = new Intent(this, typeof(EditUserProfile));
            StartActivity(activityIntent);
        }

        private void BtnGoBack_Click(object sender, EventArgs e)
        {
            Intent activityIntent = new Intent(this, typeof(SelectCourses));
            StartActivity(activityIntent);
        }
        void button_Click(object sender, System.EventArgs e)
        {
            Button button = sender as Button;
            Intent activityIntent = new Intent(this, typeof(selectedClassHomePage));

            // An extra is used to pass data from one activity to another.
            // The first argument contains a key and the second contains the 
            // data. Note that there are several overloads so we can pass
            // strings, numbers, or any primitive data type.
            activityIntent.PutExtra("Course", button.Text);

            // And finally, start the activity. Again, we are using the
            // context here.
            StartActivity(activityIntent);
        }
    }
}