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
using Android.Text;

namespace PackStudy
{
    [Activity(Label = "EditUserProfile")]
    public class EditUserProfile : Activity
    {
        EditText etEditfirstName;
        EditText etEditLastName;
        EditText etCurrentPassword;
        EditText etNewPassword;
        EditText etConfirmPassword;
        TextView tvSavePrompt;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EditUserProfile);

            // Connect layout
            etEditfirstName = (EditText)FindViewById(Resource.Id.etEditFirstName);
            etEditLastName = (EditText)FindViewById(Resource.Id.etEditLastName);
            etCurrentPassword = (EditText)FindViewById(Resource.Id.etCurrentPassword);
            etNewPassword = (EditText)FindViewById(Resource.Id.etNewPassword);
            etConfirmPassword = (EditText)FindViewById(Resource.Id.etConfirmPassword);
            Button btnUpdateProfilePic = (Button)FindViewById(Resource.Id.btnUpdateProfilePic);
            Button btnSaveChanges = (Button)FindViewById(Resource.Id.btnSaveChanges);

            // Create method for selecting image
            btnUpdateProfilePic.Click += btnUpdateProfilePic_Click;
            btnSaveChanges.Click += btnSaveChanges_Click;

            // Get shared settings to auto fill in data
            ISharedPreferences sharedPrefrences = GetSharedPreferences("MyData", FileCreationMode.Private);
            string CurrentUserID = sharedPrefrences.GetInt("id", 0).ToString();
            string CurrentUserFirstName = sharedPrefrences.GetString("FirstName", null);
            string CurrentUserLastName = sharedPrefrences.GetString("LastName", null);
            string CurrentUserPassword = sharedPrefrences.GetString("Password", null);

            // Auto fill in previous data and send back to data 
            etEditfirstName.Text = CurrentUserFirstName;
            etEditLastName.Text = CurrentUserLastName;

        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            bool firstNameTest, lastNameTest;
            bool nameTest = false;
            bool promptEmpty = false;
            string error = "";

            // Test to make sure first and last name are all chars
            firstNameTest = IsAllAlphabetic(etEditfirstName.Text);
            lastNameTest = IsAllAlphabetic(etEditLastName.Text);

            // Connect to texview
            tvSavePrompt = (TextView)FindViewById(Resource.Id.tvSavePrompt);

            //setup web service for login at login.php
            WebClient client = new WebClient();
            Uri uri = new Uri("http://packstudy-com.stackstaging.com/login.php");
            NameValueCollection parameter = new NameValueCollection();

            if (firstNameTest == true && lastNameTest == true)
            {
                parameter.Add("FirstName", etEditfirstName.Text);
                parameter.Add("LastName", etEditLastName.Text);
                //parameter.Add("Password", etPassword.Text);
                nameTest = true;

            }
            else
            {
                // Send toast for error
                error += "First and last name cannot contain any numbers or special characters";
            }
            // Test if modifying password, all feilds must be filled
            if (etCurrentPassword != null && etNewPassword != null && etConfirmPassword != null)
            {
                // Get inputed password compare to online sha1 encryption
                promptEmpty = true;

            }
            else
            {
                // Send toast for error
                error += "Please make sure all password feilds are filled";
               
            }

            // Test password portion if change is made

            
            string password = etCurrentPassword.Text;
            if (!TextUtils.IsEmpty(password))
            {

                //set POST data for login.php
                parameter.Add("Password", password);
                byte[] returnValue = client.UploadValues(uri, parameter);
                string r = Encoding.ASCII.GetString(returnValue);

                if (r == "Invalid Password")
                {
                    Context context = ApplicationContext;
                    Toast toast = Toast.MakeText(context, r, ToastLength.Long);
                    toast.Show();
                }
                else
                {
                    List<User> users = JsonConvert.DeserializeObject<List<User>>(r);
                    User CurrentUser = users[0];
                    //since the user name and password work, store it in the shared prefrences 
                    ISharedPreferences sharedPrefrences = GetSharedPreferences("MyData", FileCreationMode.Private);
                    var prefEditor = sharedPrefrences.Edit();
                    prefEditor.PutInt("id", CurrentUser.Id);
                    prefEditor.PutString("FirstName", CurrentUser.FirstName);
                    prefEditor.PutString("LastName", CurrentUser.LastName);
                    prefEditor.PutString("Email", CurrentUser.Email);
                    prefEditor.PutString("Password", CurrentUser.Password);
                    prefEditor.PutString("PhoneNumber", CurrentUser.PhoneNumber);
                    prefEditor.PutString("Username", CurrentUser.Username);

                    prefEditor.Commit();

                    //get the current shared prefrences and display welcome message
                    string CurrentUserFirstName = sharedPrefrences.GetString("FirstName", null);
                    string CurrentUserLastName = sharedPrefrences.GetString("LastName", null);

                }

            }

            // Success for all proper input
            if (nameTest == true && promptEmpty == true)
            {
                error += "Information has been successfully updated";
            }

            // Display to user and reset error
            tvSavePrompt.Text = error;
            error = "";

            // Back to home page
            Intent activityIntent = new Intent(this, typeof(CourseHome));
            StartActivity(activityIntent);

        }

        private void btnUpdateProfilePic_Click(object sender, EventArgs e)
        {
            Intent activityIntent = new Intent(this, typeof(ChangeUserPhoto));
            StartActivity(activityIntent);
        }
        bool IsAllAlphabetic(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsLetter(c))
                    return false;
            }

            return true;
        }
    }
}