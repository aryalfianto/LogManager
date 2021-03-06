﻿using System;
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
        public int tolerance = 0;
        public void ShortFile()
        {
            Directory.CreateDirectory(localproject);
            Directory.CreateDirectory(localdir);
            List<String> Mylocalfolder = Directory.GetFiles(localdir, "*", SearchOption.AllDirectories).ToList();
            foreach (string file in Mylocalfolder)
            {
                int notxt = 0;
                FileInfo mFile = new FileInfo(file);
                string extension = Properties.Settings.Default.extension;
                if (mFile.Name.Contains(extension))
                {
                    tolerance = 0;
                    Directory.CreateDirectory(localproject + @"\" + testplan);
                    try
                    {
                        File.Move(file, localproject + @"\" + testplan + @"\" + mFile.Name);
                    }
                    catch
                    {

                    }
                }
                else
                {
                    notxt++;
                    Directory.CreateDirectory(localproject + @"\" + testplan);
                    try
                    {
                        File.Move(file, localproject + @"\" + testplan + @"\" + mFile.Name);
                    }
                    catch
                    {

                    }
                    if (notxt == Mylocalfolder.Count)
                    {
                        tolerance++;
                        string[] data = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "Info.txt");
                        string[] final = data[4].Split(':');
                        string[] OK = final[1].Split(' ');

                        if (tolerance > 4)
                        {
                            Notification nt = new Notification();
                            nt.Show();
                            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(Properties.Settings.Default.listprojectNT);
                            request.Credentials = new NetworkCredential(Properties.Settings.Default.ftp_user, Properties.Settings.Default.ftp_password);
                            request.Method = WebRequestMethods.Ftp.MakeDirectory;
                            try
                            {
                                using (var resp = (FtpWebResponse)request.GetResponse())
                                {
                                }
                            }
                            catch
                            {
                                //
                            }
                            var requestx = (FtpWebRequest)WebRequest.Create(Properties.Settings.Default.listprojectNT + OK[1] + ".txt");
                            requestx.Credentials = new NetworkCredential(Properties.Settings.Default.ftp_user, Properties.Settings.Default.ftp_password);
                            requestx.Method = WebRequestMethods.Ftp.GetFileSize;
                            try
                            {
                                FtpWebResponse response = (FtpWebResponse)requestx.GetResponse();
                            }
                            catch (WebException ex)
                            {
                                FtpWebResponse response = (FtpWebResponse)ex.Response;
                                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                                {
                                    using (var client = new WebClient())
                                    {
                                        client.Credentials = new NetworkCredential(Properties.Settings.Default.ftp_user, Properties.Settings.Default.ftp_password);
                                        client.UploadFile(Properties.Settings.Default.listprojectNT + OK[1] + ".txt", WebRequestMethods.Ftp.UploadFile, AppDomain.CurrentDomain.BaseDirectory + "Info.txt");
                                    }
                                }
                            }
                            tolerance = 0;
                        }
                    }
                }   
            }
        }
    }
}
