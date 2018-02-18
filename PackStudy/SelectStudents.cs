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
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace PackStudy
{
    [Activity(Label = "Select Students")]
    public class SelectStudents : Activity
    {
        List<User> students = new List<User>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            WebClient client = new WebClient();
            Uri uri = new Uri("http://packstudy-com.stackstaging.com/getstudents.php");
            NameValueCollection parameter = new NameValueCollection();
            
            // to do course name by extra
            string course = Intent.GetStringExtra("Course");
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
            
            foreach (User s in students)
            {
                CheckBox chkbox = new CheckBox(this);
                chkbox.Text = s.FirstName + "  " + s.LastName;
                chkbox.Checked = false;
                chkbox.Id = s.Id;
                ll.AddView(chkbox);
            }

            Button btnGenerateDoc = (Button)FindViewById(Resource.Id.btnGenerateDoc);
            btnGenerateDoc.Click += btnGenerateDoc_Click;
        }

        private void btnGenerateDoc_Click(object sender, EventArgs e)
        {
            List<User> selectedStudents = new List<User>();
            LinearLayout root = (LinearLayout)FindViewById(Resource.Id.ssll);
            String s = "";
            string DocDescription = Intent.GetStringExtra("Description");
            string course = Intent.GetStringExtra("Course");
            ISharedPreferences sharedPrefrences = GetSharedPreferences("MyData", FileCreationMode.Private);
            string currentUserEmail = sharedPrefrences.GetString("Email", null);
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("PackStudy", "packstudyunr@gmail.com"));
            message.Subject = "Google Doc For " + course + " "+DocDescription;
            message.Body = new TextPart("plain")
            {
                Text = @"Here is the link for the Google Doc "+DocDescription+@", 

https://docs.google.com/document/d/1_BKlr9rwaqBM3euM5IstoiDOrRrqLdwO8MNS96GOGbU/edit

-- Pack Study"
            };
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
                                selectedStudents.Add(u);
                                s += u.FirstName + "  " + u.Email + "\n";
                                message.Bcc.Add(new MailboxAddress("Student", u.Email));
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
            s += "selected";
            Context context = ApplicationContext;
            Toast toast = Toast.MakeText(context, s, ToastLength.Long);
            toast.Show();

            // to do, send google doc link emails
            using (var client = new SmtpClient())
            {

                client.Connect("smtp.gmail.com", 587);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("packstudyunr", "packstudy1");

                client.Send(message);
                client.Disconnect(true);

            }
            Intent activityIntent = new Intent(this, typeof(selectedClassHomePage));
            StartActivity(activityIntent);
        }
    }
}