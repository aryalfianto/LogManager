using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LogManager
{
    public partial class Tester_Location : Form
    {
        public Tester_Location()
        {
            InitializeComponent();
        }

        string currentStation;
        static void DeleteFtpDirectory(string url)
        {
            try
            {
                FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(url);
                listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                listRequest.Credentials = new NetworkCredential(Properties.Settings.Default.ftp_user, Properties.Settings.Default.ftp_password);
                List<string> lines = new List<string>();

                using (FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse())
                using (Stream listStream = listResponse.GetResponseStream())
                using (StreamReader listReader = new StreamReader(listStream))
                {
                    while (!listReader.EndOfStream)
                    {
                        lines.Add(listReader.ReadLine());
                    }
                }

                foreach (string line in lines)
                {
                    string[] tokens =
                        line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                    string name = tokens[8];
                    string permissions = tokens[0];

                    string fileUrl = url + name;

                    if (permissions[0] == 'd')
                    {

                        DeleteFtpDirectory(fileUrl + "/");
                    }
                    else
                    {
                        FtpWebRequest deleteRequest = (FtpWebRequest)WebRequest.Create(fileUrl);
                        deleteRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                        deleteRequest.Credentials = new NetworkCredential(Properties.Settings.Default.ftp_user, Properties.Settings.Default.ftp_password);

                        deleteRequest.GetResponse();
                    }
                }

                FtpWebRequest removeRequest = (FtpWebRequest)WebRequest.Create(url);
                removeRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
                removeRequest.Credentials = new NetworkCredential(Properties.Settings.Default.ftp_user, Properties.Settings.Default.ftp_password);
                removeRequest.GetResponse();
            }
            catch
            {

            }
        }
        string testernow;
        private void button1_Click(object sender, EventArgs e)
        {
            DeleteFtpDirectory(Properties.Settings.Default.tester_signal);
            string[] current1 = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "Info.txt");
            foreach (string list in current1)
            {
                if (list.Contains("Station"))
                {
                    string[] parse = list.Split(' ');
                    testernow = parse[2];
                    break;
                }
            }

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(Properties.Settings.Default.SendSignal + testernow + "/");
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

            int milliseconds = 5000;
            Thread.Sleep(milliseconds);

            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(Properties.Settings.Default.tester_signal);
            ftpRequest.Credentials = new NetworkCredential(Properties.Settings.Default.ftp_user, Properties.Settings.Default.ftp_password);
            
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
            StreamReader streamReader = new StreamReader(response.GetResponseStream());

            List<string> directories = new List<string>();

            string line = streamReader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                string[] preparse = line.Split('_');
                directories.Add(preparse[2]);
                line = streamReader.ReadLine();
            }
            streamReader.Close();

            List<string> locals = new List<string>();
            List<Control> gbg = new List<Control>();
            List<Control> lbl = new List<Control>();
            foreach (Control gb in this.Controls)
            {
                if (gb is Guna.UI2.WinForms.Guna2GroupBox)
                {
                    gbg.Add(gb);
                    string[] preparse = gb.Text.Split('#');
                    locals.Add(preparse[1]);
                }
                if (gb is System.Windows.Forms.Label)
                {
                    lbl.Add(gb);
                }
            }

            string[] current = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "Info.txt");
            foreach (string list in current)
            {
                if (list.Contains("Station"))
                {
                    string[] parse = list.Split('_');
                    currentStation = parse[2];
                }
            }

            foreach (Control z in gbg)
            {
                for (int a = 0; a < locals.Count; a++)
                {
                    try
                    {
                        if (z.Text.Contains(directories[a]))
                        {
                            foreach (Control x in lbl)
                            {
                                if (x.Name.Contains(currentStation))
                                {
                                    x.ForeColor = Color.White;
                                    x.BackColor = Color.ForestGreen;
                                }
                                if (x.Name.Contains(directories[a]))
                                {
                                    x.Text = "OK";
                                    
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
            button1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Tester_Location_Load(object sender, EventArgs e)
        {
            button1.Visible = true;
        }
    }
}
