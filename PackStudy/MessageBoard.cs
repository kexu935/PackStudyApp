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
using Android.Graphics;
using System.Timers;
using System.Threading.Tasks;
using Urix = Android.Net.Uri;
using Android.Provider;

namespace PackStudy
{
    [Activity(Label = "MessageBoard")]
    public class MessageBoard : Activity
    {
        int lastmessageId = 0;
        bool starttimer = true;
        public static readonly int PickImageId = 1000;
        private ImageView _imageView;
        string currentCourse;

        //converts an image to a bitmap file path 
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

        //override of the activity result to post images to the database. This event is fired once
        //a new image is chosen from the gallery
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                WebClient Client = new System.Net.WebClient();
                string imagePath = UriHelper.GetPathFromUri(this, data.Data);
                Client.Headers.Add("Content-Type", "binary/octet-stream");
                //upload image to web server via php and bitmap
                byte[] result = Client.UploadFile("http://packstudy-com.stackstaging.com/uploadFileMessage.php", "POST",
                                                  imagePath);

                //display the uploaded image
                string s = System.Text.Encoding.UTF8.GetString(result, 0, result.Length);
                if(s == "sucess")
                {
                    string[] substrings = imagePath.Split('/');
                    string urlFilePath = "http://packstudy-com.stackstaging.com/fileUploads/" + substrings[substrings.Length - 1];
                    

                    ISharedPreferences sharedPrefrences = GetSharedPreferences("MyData", FileCreationMode.Private);
                    int UserId = sharedPrefrences.GetInt("id", 0);
                    WebClient client = new WebClient();
                    Uri uri = new Uri("http://packstudy-com.stackstaging.com/sendmessage.php");
                    NameValueCollection parameter = new NameValueCollection();

                    //add each of the parameters that need to be posted witht he photo
                    string FirstName = sharedPrefrences.GetString("FirstName", null);
                    string LastName = sharedPrefrences.GetString("LastName", null);
                    parameter.Add("UserId", UserId.ToString());
                    currentCourse = sharedPrefrences.GetString("CurrentCourse", null);
                    parameter.Add("Semester", "Spring2017");
                    parameter.Add("Course", currentCourse);
                    parameter.Add("Message", "FILE_"+urlFilePath);
                    parameter.Add("Name", FirstName + " " + LastName);
                    //send the messsage 
                    byte[] returnValue = client.UploadValues(uri, parameter);
                    string r = Encoding.ASCII.GetString(returnValue);

                }
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //set up the activity properties
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MessageBoard);
            // Create your application here
            WebClient client = new WebClient();
            Uri uri = new Uri("http://packstudy-com.stackstaging.com/messages.php");
            NameValueCollection parameter = new NameValueCollection();
            currentCourse = Intent.GetStringExtra("Course");
            ISharedPreferences sharedPrefrences = GetSharedPreferences("MyData", FileCreationMode.Private);
            var prefEditor = sharedPrefrences.Edit();
            if(currentCourse != null) prefEditor.PutString("CurrentCourse", currentCourse);
            //confim prefrences
            prefEditor.Commit();
            Title = currentCourse;
            GetMessages();
            Button btnUpload = (Button)FindViewById(Resource.Id.btnUpload);
            btnUpload.Click += BtnUpload_Click;
            Button btnSend = (Button)FindViewById(Resource.Id.btnSend);
            btnSend.Click += BtnSend_Click;
            RunUpdateLoop();

        }

        //view image in full screen
        private void BtnUpload_Click(object sender, EventArgs e)
        {
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
        }
        
        //check for a new message every 5 seconds
        private async void RunUpdateLoop()
        {
            while (starttimer)
            {
                await Task.Delay(5000);
                GetMessages();
            }
        }
        private void BtnSend_Click(object sender, EventArgs e)
        {
            starttimer = false;
            EditText txtMessage = (EditText)FindViewById(Resource.Id.txtMessage);
            //set up the send message service
            ISharedPreferences sharedPrefrences = GetSharedPreferences("MyData", FileCreationMode.Private);
            int UserId = sharedPrefrences.GetInt("id", 0);
            WebClient client = new WebClient();
            Uri uri = new Uri("http://packstudy-com.stackstaging.com/sendmessage.php");
            NameValueCollection parameter = new NameValueCollection();

            //add the fields that will be posted
            string FirstName = sharedPrefrences.GetString("FirstName", null);
            string LastName = sharedPrefrences.GetString("LastName", null);
            parameter.Add("UserId", UserId.ToString());
            currentCourse = sharedPrefrences.GetString("CurrentCourse", null);
            parameter.Add("Semester", "Spring2017");
            parameter.Add("Course", currentCourse);
            parameter.Add("Message", txtMessage.Text);
            parameter.Add("Name", FirstName + " " + LastName);
            //post message to the database
            byte[] returnValue = client.UploadValues(uri, parameter);
            string r = Encoding.ASCII.GetString(returnValue);

            txtMessage.Text = "";
            GetMessages();
            //resume timer until next check
            starttimer = true;
        }

        //function to get and display new messages
        private void GetMessages()
        {
            ISharedPreferences sharedPrefrences = GetSharedPreferences("MyData", FileCreationMode.Private);
            int UserId = sharedPrefrences.GetInt("id", 0);
            WebClient client = new WebClient();
            Uri uri = new Uri("http://packstudy-com.stackstaging.com/getmessages.php");
            NameValueCollection parameter = new NameValueCollection();
            currentCourse = sharedPrefrences.GetString("CurrentCourse", null);

            parameter.Add("Course", currentCourse);
            parameter.Add("LastId", lastmessageId.ToString());
            //obtain messages from getmessages.php 
            byte[] returnValue = client.UploadValues(uri, parameter);
            string r = Encoding.ASCII.GetString(returnValue);
            List<Message> messages = JsonConvert.DeserializeObject<List<Message>>(r);
            if(messages == null)
            {
                
                return;
            }
            
            LinearLayout llMessageBoard = (LinearLayout)FindViewById(Resource.Id.llMessageBoard);
            TextView textView1;
            //loop through messages and display each one
            foreach (Message m in messages)
            {

                textView1 = new TextView(this) { Text = m.Name + "\n\n" + m.aMessage + "\n\n" + m.reg_date + "\n" };
                if (UserId.ToString() == m.UserId)
                {
                    //#1A89F7 ligher blue
                    textView1.SetBackgroundColor(Color.ParseColor("#0E122D"));
                    textView1.SetTextColor(Color.White);
                    textView1.Gravity = GravityFlags.Right;

                }
                else
                {
                    textView1.SetBackgroundColor(Color.ParseColor("#E6E5EB"));
                    textView1.SetTextColor(Color.Black);

                }
                textView1.TextSize = 14;
                textView1.SetPadding(50, 50, 50, 50);
                textView1.Focusable = true;
                textView1.FocusableInTouchMode = true;
                if (m.aMessage.Contains("FILE_"))
                {
                    textView1.Text = m.Name + "\n\n" + m.reg_date + "\n";
                    ImageView image = new ImageView(this);
                    string absolutPath = m.aMessage.Replace("FILE_", "");
                    var imageBitmap = GetImageBitmapFromUrl(absolutPath);
                    image.SetImageBitmap(imageBitmap);
                    /*Urix url = Urix.Parse("http://packstudy-com.stackstaging.com/fileUploads/"+ substrings[substrings.Length - 1]);
                    image.SetImageURI(url);
                    image.SetImageURI (data.Data);*/
                    image.SetPadding(50, 50, 50, 50);
                    image.Focusable = false;
                    if (UserId.ToString() == m.UserId)
                    {
                        image.SetBackgroundColor(Color.ParseColor("#0E122D"));
                    }
                    else
                    {
                        image.SetBackgroundColor(Color.ParseColor("#E6E5EB"));
                    }
                    image.Tag = absolutPath; 
                    image.SetMinimumHeight(300);
                    image.SetMinimumWidth(300);
                    image.SetAdjustViewBounds(true);
                    image.Click += new EventHandler(this.image_Click);

                    llMessageBoard.AddView(image);
                    
                }
                llMessageBoard.AddView(textView1);
                // textView1.RequestFocus();
                TextView space = new TextView(this);
                space.SetMinimumHeight(50);
                space.Focusable = true;
                space.FocusableInTouchMode = true;
                llMessageBoard.AddView(space);
                space.RequestFocus();
                lastmessageId = m.id;
                
                
            }  
        }
        private void image_Click(object sender, EventArgs e)
        {
            ImageView imageView = sender as ImageView;
            string filepath = imageView.Tag.ToString();
            Intent activityIntent = new Intent(this, typeof(FullScreenImageView));

            // Grab extra and send over
            activityIntent.PutExtra("FilePath", filepath);

            // And finally, start the activity. Again, we are using the
            // context here.
            StartActivity(activityIntent);
        }
    }
}