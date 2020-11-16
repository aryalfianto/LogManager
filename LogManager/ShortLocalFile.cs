using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace LogManager
{
    public class ShortLocalFile
    {
        public string localdir { get; set; }
        public string testplan { get; set; }
        public string localproject { get; set; }
        string extention = Properties.Settings.Default.extension;
        public int tolerance = 0;
        public void ShortFile()
        {
            Directory.CreateDirectory(localproject);
            Directory.CreateDirectory(localdir);
            List<String> Mylocalfolder = Directory.GetFiles(localdir, "*"+extention, SearchOption.AllDirectories).ToList();
            foreach (string file in Mylocalfolder)
            {
                FileInfo mFile = new FileInfo(file);
                Directory.CreateDirectory(localproject + @"\" + testplan);
                try
                {
                    int milliseconds = 1000;
                    Thread.Sleep(milliseconds);
                    File.Move(file, localproject + @"\" + testplan + @"\" + mFile.Name);
                }
                catch
                {

                }      
            }
        }
    }
}
