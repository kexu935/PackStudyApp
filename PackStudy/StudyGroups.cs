using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;

using Android.Widget;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Android.Views;
using Android.Text;

namespace PackStudy
{
    [Activity(Label = "StudyGroups")]
    public class StudyGroups : Activity
    {
        List<User> students = new List<User>();
        EditText name;
        string course;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.Title = "Create Study Groups";

            WebClient client = new WebClient();
            Uri uri = new Uri("http://packstudy-com.stackstaging.com/getstudents.php");
            NameValueCollection parameter = new NameValueCollection();

            // to do course name by extra
            course = Intent.GetStringExtra("Course");
            parameter.Add("Course", course);
            byte[] returnValue = client.UploadValues(uri, parameter);
            string r = Encoding.ASCII.GetString(returnValue);
            List<User> users = JsonConvert.DeserializeObject<List<User>>(r);

            foreach (User s in users)
            {
                students.Add(s);
            }

            SetContentView(Resource.Layout.SelectStudents);
            LinearLayout ll = (LinearLayout)FindViewById(Resource.Id.ssll);

            name = new EditText(this);
            //btn.Text = c.semester + " " + c.id;
            name.Hint = "Enter Name of Study Group";
            name.Id = 0;
            //btn.SetBackgroundResource(Resource.Drawable.CircleButton);


            ll.AddView(name);

            foreach (User s in students)
            {
                CheckBox chkbox = new CheckBox(this);
                chkbox.Text = s.FirstName + "  " + s.LastName;
                chkbox.Checked = false;
                chkbox.Id = s.Id;
                ll.AddView(chkbox);
            }


            Button btnGenerateDoc = (Button)FindViewById(Resource.Id.btnGenerateDoc);
            btnGenerateDoc.Text = "Create Study Group";
            btnGenerateDoc.Click += btnStudyGroup_Click;
        }

        private void btnStudyGroup_Click(object sender, EventArgs e)
        {

            List<User> selectedStudents = new List<User>();
            LinearLayout root = (LinearLayout)FindViewById(Resource.Id.ssll);
            String str = "";

            if (!TextUtils.IsEmpty(name.Text))
            {


                WebClient client = new WebClient();
                Uri uri = new Uri("http://packstudy-com.stackstaging.com/createstudygroup.php");
                NameValueCollection parameter = new NameValueCollection();

                for (int i = 0; i < root.ChildCount; i++)
                {
                    View v = root.GetChildAt(i);
                    if (v is CheckBox)
                    {
                        if (((CheckBox)v).Checked == true)
                        {
                            foreach (User u in students)
                            {
                                if (u.Id == v.Id)
                                {
                                    parameter.Add("UserId", u.Id.ToString() + ",");

                                    str += u.FirstName + "  " + u.Email + "\n";
                                    break;
                                }
                            }
                        }
                    }
                    if (v is Button)
                    {
                        continue;
                    }
                }
                parameter.Add("Course", course);

                string no_spaces = name.Text.Replace(" ", "_");

                parameter.Add("Topic", no_spaces);

                byte[] returnValue = client.UploadValues(uri, parameter);
                string r = Encoding.ASCII.GetString(returnValue);

                if (r == "")
                {
                    str += "selected";
                    Context context = ApplicationContext;
                    Toast toast = Toast.MakeText(context, str, ToastLength.Long);
                    toast.Show();

                }
                else
                {
                    Context context = ApplicationContext;
                    Toast toast = Toast.MakeText(context, "Adding Study Group Was Not Successful", ToastLength.Long);
                    toast.Show();
                }

                Intent activityIntent = new Intent(this, typeof(selectedClassHomePage));

                activityIntent.PutExtra("StudyGroupName", name.Text);
                activityIntent.PutExtra("Course", course);

                StartActivity(activityIntent);
            }
            else
            {
                Context context = ApplicationContext;
                Toast toast = Toast.MakeText(context, "Please Enter a Name for the Study Group", ToastLength.Long);
                toast.Show();
            }
        }
    }
}