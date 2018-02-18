﻿using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using Android.Content;
using System.Collections.Generic;
using Newtonsoft.Json;
using Android.Text;

namespace PackStudy
{
    [Activity(Label = "PackStudy", MainLauncher = true, Icon = "@drawable/newwolfhead")]
    public class MainActivity : Activity
    {
        string email;
        string password;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            //check the current sharedPrefrences to see if a user is signed in
            /* ISharedPreferences sharedPrefrences = GetSharedPreferences("MyData", FileCreationMode.Private);
             string CurrentUserFirstName = sharedPrefrences.GetString("FirstName", null);
             string CurrentUserLastName = sharedPrefrences.GetString("LastName", null);
             string message = "Welcome " + CurrentUserFirstName + " " + CurrentUserLastName;
             Context context = ApplicationContext;
             Toast toast = Toast.MakeText(context, message, ToastLength.Long);
             toast.Show();*/

            //clear the shared prefrences
            ISharedPreferences settings = GetSharedPreferences("MyData", FileCreationMode.Private);
            settings.Edit().Remove("MyData").Commit();
            Button btnLogin = (Button)FindViewById(Resource.Id.btnLogin);
            btnLogin.Click += BtnLogin_Click;
            Button btnRegister = (Button)FindViewById(Resource.Id.btnRegister);
            btnRegister.Click += btnRegister_Click;

        }

        //go to the regerst activity on click
        private void btnRegister_Click(object sender, EventArgs args)
        {
            Intent activityIntent = new Intent(this, typeof(UserRegistration));
            StartActivity(activityIntent);
        }
        //clicking the login button activity
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            //setup web service for login at login.php
            WebClient client = new WebClient();
            Uri uri = new Uri("http://packstudy-com.stackstaging.com/login.php");
            NameValueCollection parameter = new NameValueCollection();

            //get input values for email and password
            EditText txtEmail = (EditText)FindViewById(Resource.Id.txtEmail);
            EditText txtPassword = (EditText)FindViewById(Resource.Id.txtPassword);

            email = txtEmail.Text;
            password = txtPassword.Text;
            if (!TextUtils.IsEmpty(email) || !TextUtils.IsEmpty(password))
            {

                //set POST data for login.php
                parameter.Add("Email", email);
                parameter.Add("Password", password);
                byte[] returnValue = client.UploadValues(uri, parameter);
                string r = Encoding.ASCII.GetString(returnValue);
                //if empty string is returned then invalid email
                if (r == "")
                {
                    //display the toast for invaild email
                    string message = "Invalid Email";
                    Context context = ApplicationContext;
                    Toast toast = Toast.MakeText(context, message, ToastLength.Long);
                    toast.Show();
                }
                //if invalid password is returned, display the toast for invalid password
                else if (r == "Invalid Password")
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
                    string message = "Welcome " + CurrentUserFirstName + " " + CurrentUserLastName;
                    Context context = ApplicationContext;
                    Toast toast = Toast.MakeText(context, message, ToastLength.Long);
                    toast.Show();
                    Intent activityIntent = new Intent(this, typeof(CourseHome));
                    StartActivity(activityIntent);

                }

            }
            else
            {
                Toast toast = Toast.MakeText(ApplicationContext, "Please enter in the correct fields", ToastLength.Long);
                toast.Show();
            }

        }

    }
}