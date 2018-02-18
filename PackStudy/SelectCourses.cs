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

namespace PackStudy
{
    [Activity(Label = "SelectCourses")]
    public class SelectCourses : Activity
    {
        public static List<Course> CourseList = new List<Course>();
        string curCourse;
        string curSemester;
        Spinner spinCourses;
        Spinner spinSemesters;
        TextView txtSelectedCourses;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SelectCourses);

            spinSemesters = (Spinner)FindViewById(Resource.Id.spinSemester);
            spinSemesters.ItemSelected += spinSemesters_ItemSelected;

            ArrayAdapter adapter1 = ArrayAdapter.CreateFromResource(this, Resource.Array.semesters_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter1.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinSemesters.Adapter = adapter1;

            spinCourses = (Spinner)FindViewById(Resource.Id.spinClasses);
            spinCourses.ItemSelected += spinCourses_ItemSelected;

            ArrayAdapter adapter2 = ArrayAdapter.CreateFromResource(this, Resource.Array.courses_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter2.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinCourses.Adapter = adapter2;

            Button btnAddClass = (Button)FindViewById(Resource.Id.btnAddCourse);
            btnAddClass.Click += btnAddCourse_Click;

            Button btnRemoveClass = (Button)FindViewById(Resource.Id.btnRemoveCourse);
            btnRemoveClass.Click += btnRemoveCourse_Click;

            Button btnGoNext = (Button)FindViewById(Resource.Id.btnGoNext);
            btnGoNext.Click += BtnGoNext_Click;

            txtSelectedCourses = (TextView)FindViewById(Resource.Id.txtSelectedCourses);
            ShowCourseList(txtSelectedCourses);
        }

        private void ShowCourseList(TextView txtSelectedCourses)
        {
            if (CourseList.Count() == 0)
            {
                txtSelectedCourses.Text = "No course in your list";
            }
            else
            {
                txtSelectedCourses.Text = "You've selected:\n";
                foreach (Course c in CourseList)
                {
                    txtSelectedCourses.Text += c.semester + " " + c.id + "\n";
                }
                txtSelectedCourses.Text += "\n";
            }
        }

        private void BtnGoNext_Click(object sender, EventArgs e)
        {
            WebClient client = new WebClient();
            Uri uri = new Uri("http://packstudy-com.stackstaging.com/addcourses.php");
            NameValueCollection parameter = new NameValueCollection();
            ISharedPreferences sharedPrefrences = GetSharedPreferences("MyData", FileCreationMode.Private);
            int UserId = sharedPrefrences.GetInt("id", 0);

            foreach (Course c in CourseList)
            {
                parameter.Add("UserId", UserId.ToString());
                parameter.Add("CourseId", c.id);
                parameter.Add("SemesterId", c.semester);
                byte[] returnValue = client.UploadValues(uri, parameter);
                string r = Encoding.ASCII.GetString(returnValue);
                parameter.Remove("UserId");
                parameter.Remove("CourseId");
                parameter.Remove("SemesterId");
            }

            Intent activityIntent = new Intent(this, typeof(CourseHome));
            StartActivity(activityIntent);
        }

        private void btnRemoveCourse_Click(object sender, EventArgs e)
        {
            if (curCourse == null || curSemester == null)
            {
                return;
            }
            Course temp = Course.MakeCourse(curCourse, curSemester);
            if (CourseList.Exists(c => c.id == temp.id && c.semester == temp.semester))
            {
                Course toRemove = new Course();
                foreach (Course c in CourseList)
                {
                    if (c.id == temp.id)
                    {
                        toRemove = c;
                        break;
                    }
                }
                CourseList.Remove(toRemove);

                String s = curSemester + " " + curCourse + " removed";
                Context context = ApplicationContext;
                Toast toast = Toast.MakeText(context, s, ToastLength.Long);
                toast.Show();
            }
            else
            {
                String s = curSemester + " " + curCourse + " has been removed";
                Context context = ApplicationContext;
                Toast toast = Toast.MakeText(context, s, ToastLength.Long);
                toast.Show();
            }

            ShowCourseList(txtSelectedCourses);
        }

        private void btnAddCourse_Click(object sender, EventArgs e)
        {
            Course temp = Course.MakeCourse(curCourse, curSemester);
            if (!CourseList.Exists(c => c.id == temp.id && c.semester == temp.semester))
            {
                CourseList.Add(temp);

                String s = curSemester + " " + curCourse + " added";
                Context context = ApplicationContext;
                Toast toast = Toast.MakeText(context, s, ToastLength.Long);
                toast.Show();
            }
            else
            {
                String s = curSemester + " " + curCourse + " has been added";
                Context context = ApplicationContext;
                Toast toast = Toast.MakeText(context, s, ToastLength.Long);
                toast.Show();
            }

            ShowCourseList(txtSelectedCourses);
        }

        private void spinCourses_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            curCourse = spinner.GetItemAtPosition(e.Position).ToString();
        }

        private void spinSemesters_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            curSemester = spinner.GetItemAtPosition(e.Position).ToString();
        }
    }
}