using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace LogManager
{
    public partial class Warning : Form
    {
        public Warning()
        {
            InitializeComponent();
        }
        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            
            if (guna2TextBox1.Text != "" & guna2TextBox2.Text != "")
            {
                string badge = guna2TextBox1.Text.ToLower();
                string password = guna2TextBox2.Text.ToLower();
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        string login = client.DownloadString(Properties.Settings.Default.loginparser + badge + "&txtPassword=" + password);
                        if(login.Contains("ERR"))
                        {
                            gunaLabel2.Text = "BadgeID atau Password Salah !";
                            gunaLabel2.Visible = true;
                            guna2TextBox1.Text = "";
                            guna2TextBox2.Text = "";
                            this.ActiveControl = guna2TextBox1;
                        }
                        if(login.Contains("OK"))
                        {

                            gunaLabel2.Visible = false;
                            this.Close();
                        }
                    }
                }
                catch
                {
                    
                }
            }
            else
            {
                gunaLabel2.Text = "Isi Data Dengan Lengkap !";
                gunaLabel2.Visible = true;
                guna2TextBox1.Text = "";
                guna2TextBox2.Text = "";
                this.ActiveControl = guna2TextBox1;
            }
        }
        private void guna2TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("\t");
            }
        }
        private void guna2TextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                guna2Button1_Click_1(this, new EventArgs());
            }
        }

        private void Warning_Load(object sender, EventArgs e)
        {
            string[] data = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "Info.txt");
            string[] final = data[4].Split(':');
            string[] OK = final[1].Split(' ');

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(Properties.Settings.Default.listprojectNG);
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
            var requestx = (FtpWebRequest)WebRequest.Create(Properties.Settings.Default.listprojectNG + OK[1] + ".txt");
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
                        client.UploadFile(Properties.Settings.Default.listprojectNG + OK[1] + ".txt", WebRequestMethods.Ftp.UploadFile, AppDomain.CurrentDomain.BaseDirectory + "Info.txt");
                    }
                }
            }
        }

    }
}
