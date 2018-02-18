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
using Newtonsoft.Json;

namespace PackStudy
{
    class Message
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("UserId")]
        public string UserId { get; set; }

        [JsonProperty("Message")]
        public string aMessage { get; set; }

        [JsonProperty("reg_date")]
        public string reg_date { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

    }

    class Topic
    {
        [JsonProperty("SessionTitle")]
        public string SessionTitle { get; set; }
    }
}