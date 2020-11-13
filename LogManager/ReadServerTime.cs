using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LogManager
{
    public class ReadServerTime
    {
        public string parser { get; set; }

        public string DateServer()
        {
            string ServerTime;
            try
            {
                using (WebClient client = new WebClient())
                {
                    string RawServerTime = client.DownloadString(parser);
                    ServerTime = parsing(RawServerTime);
                }
            }
            catch
            {
                ServerTime = DateTime.Now.ToString("yyyy/M/d");
            }
            return ServerTime;
        }

        private string parsing(string RawDate)
        {
            string [] pemisah = RawDate.Split(';');
            string[] Date = pemisah[1].Split(' ');
            string[] Dates = Date[0].Split('/');
            string finaldate =  Dates[2] + "/" + Dates[0] + "/" + Dates[1];
            return finaldate;
        }
    }
}
