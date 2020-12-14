using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace LogManager
{
    public partial class Form1 : Form
    {
        readonly IdentificationProject _readTestplan = new IdentificationProject();
        readonly ReadServerTime _serverTime = new ReadServerTime();
        readonly ShortLocalFile _shortLocalFile = new ShortLocalFile();
        public Form1()
        {
            InitializeComponent();
            _readTestplan.DirectoryLog1 = Properties.Settings.Default.local_testplan1;
            _readTestplan.DirectoryLog2 = Properties.Settings.Default.local_testplan2;
            _serverTime.parser = Properties.Settings.Default.date_parser;
            _shortLocalFile.localproject = Properties.Settings.Default.local_project;
            _shortLocalFile.localdir = Properties.Settings.Default.local_directory;
        }
        int timer = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer++;
            gunaLabel5.Text = _readTestplan.Project();
            gunaLabel2.Text = Yield();
            guna2TextBox1.Text = Properties.Settings.Default.ftp_directory;
            guna2TextBox2.Text = Properties.Settings.Default.local_directory;
            if (WindowState == FormWindowState.Normal && timer > 10)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
            IWASManager(Properties.Settings.Default.process_iwas, Properties.Settings.Default.iwas_version);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            timer = 0;
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void gunaLabel5_TextChanged(object sender, EventArgs e)
        {
            List<String> Mylocalfolder = Directory.GetFiles(Properties.Settings.Default.local_directory, "*", SearchOption.AllDirectories).ToList();
            foreach(string list in Mylocalfolder)
            {
                FileInfo mFile = new FileInfo(list);
                if(mFile.FullName.Contains(Properties.Settings.Default.extension)==false)
                {
                    File.Delete(list);
                }
            }
            tolerace = 0;
        }

        /// <summary>
        /// Mengirim File Ke FTP Server dari Folder Datalog_Project
        /// </summary>
        /// <param name="localproject">Directory Project</param>
        /// <param name="ftppath">Directory FPT Server</param>
        private void Send(string localproject, string ftppath)
        {
            var serverdate = _serverTime.DateServer();
            var date = serverdate;
            string extension = Properties.Settings.Default.extension;
            List<String> Mylocalfolder = Directory.GetFiles(localproject, "*", SearchOption.AllDirectories).ToList();
            foreach (string file in Mylocalfolder)
            {
                FileInfo mFile = new FileInfo(file);
                string folderproject = Path.GetFileName(Path.GetDirectoryName(file));
                CreateFolder(ftppath, folderproject, date, file, mFile.Name);
            }
        }

        /// <summary>
        /// Membuat Folder Berdasarkan Waktu
        /// </summary>
        /// <param name="ftpServer">directory FTP server</param>
        /// <param name="Folderproject">Nama Project</param>
        /// <param name="date">Keterangan Waktu</param>
        /// <param name="file">Directory File</param>
        /// <param name="filename">Nama File</param>
        private void CreateFolder(string ftpServer, string Folderproject, string date, string file, string filename)
        {
            try
            {
                string User = Properties.Settings.Default.ftp_user;
                string Password = Properties.Settings.Default.ftp_password;
                string[] datez = date.Split('/');
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ftpServer + Folderproject + "/" + datez[0] + "/" + datez[1] + "/" + datez[2]);
                request.Credentials = new NetworkCredential(User, Password);
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
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(User, Password);
                    client.UploadFile(ftpServer + Folderproject + "/" + datez[0] + "/" + datez[1] + "/" + datez[2] + "/" + filename, WebRequestMethods.Ftp.UploadFile, file);
                }

                var request4 = (FtpWebRequest)WebRequest.Create(ftpServer + Folderproject + "/" + datez[0] + "/" + datez[1] + "/" + datez[2] + "/" + filename);
                request4.Credentials = new NetworkCredential(User, Password);
                request4.Method = WebRequestMethods.Ftp.GetFileSize;

                try
                {
                    FtpWebResponse response = (FtpWebResponse)request4.GetResponse();
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        //gagal hapus file
                    }
                    notifyIcon1.BalloonTipText = (filename + " Sent To " + Folderproject + " !");
                    notifyIcon1.ShowBalloonTip(1000);

                    FtpWebRequest requestx = (FtpWebRequest)FtpWebRequest.Create(Properties.Settings.Default.log_sent + datez[0] + "/" + datez[1] + "/");
                    requestx.Credentials = new NetworkCredential(User, Password);
                    requestx.Method = WebRequestMethods.Ftp.MakeDirectory;
                    try
                    {
                        using (var resp = (FtpWebResponse)requestx.GetResponse())
                        {
                        }
                    }
                    catch
                    {
                        //
                    }
                    
                    try
                    {
                        using (var client = new WebClient())
                        {
                            client.Credentials = new NetworkCredential(User, Password);
                            client.DownloadFile(Properties.Settings.Default.log_sent + datez[0] + "/" + datez[1] + "/" + gunaLabel1.Text + ".txt", AppDomain.CurrentDomain.BaseDirectory + gunaLabel1.Text + ".txt");
                        }

                        FtpWebRequest requestv = (FtpWebRequest)FtpWebRequest.Create(Properties.Settings.Default.log_sent + datez[0] + "/" + datez[1] + "/" + gunaLabel1.Text + ".txt");
                        requestv.Credentials = new NetworkCredential(User, Password);
                        requestv.Method = WebRequestMethods.Ftp.DeleteFile;
                        FtpWebResponse responsev = (FtpWebResponse)requestv.GetResponse();
                        responsev.Close();

                        using (WebClient client = new WebClient())
                        {
                            string RawServerTime = client.DownloadString(Properties.Settings.Default.date_parser);
                            string tempo = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + gunaLabel1.Text + ".txt");
                            string newtempo = RawServerTime + " " + filename + " Sent To " + Folderproject + "\n" + tempo;
                            File.Delete(AppDomain.CurrentDomain.BaseDirectory + gunaLabel1.Text + ".txt");
                            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + gunaLabel1.Text + ".txt", newtempo);
                        }
                        using (var client = new WebClient())
                        {
                            client.Credentials = new NetworkCredential(User, Password);
                            client.UploadFile(Properties.Settings.Default.log_sent + datez[0] + "/" + datez[1] + "/" + gunaLabel1.Text + ".txt", WebRequestMethods.Ftp.UploadFile, AppDomain.CurrentDomain.BaseDirectory + gunaLabel1.Text + ".txt");
                        }
                        
                    }
                    catch
                    {
                        using (WebClient client = new WebClient())
                        {
                            string RawServerTime = client.DownloadString(Properties.Settings.Default.date_parser);
                            string tempo = RawServerTime + " " + filename + " Sent To " + Folderproject;
                            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + gunaLabel1.Text + ".txt", tempo);
                        }
                        using (var client = new WebClient())
                        {
                            client.Credentials = new NetworkCredential(User, Password);
                            client.UploadFile(Properties.Settings.Default.log_sent + datez[0] + "/" + datez[1] + "/" + gunaLabel1.Text + ".txt", WebRequestMethods.Ftp.UploadFile, AppDomain.CurrentDomain.BaseDirectory + gunaLabel1.Text + ".txt");
                        }
                        //jika tidak ada langsungsuk kirim
                    }

                }
                catch (WebException ex)
                {
                    var response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {

                    }
                }
            }
            catch
            {
               //
            }
            
        }
        /// <summary>
        /// Check Ftp server Active or Not
        /// </summary>
        /// <param name="directory">Directory Alamat FTP Server Yang Akan Dicheck</param>
        /// <returns></returns>
        private bool FtpCheck(string directory)
        {
            try
            {
                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(new Uri(directory));
                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                requestDir.Credentials = new NetworkCredential(Properties.Settings.Default.ftp_user, Properties.Settings.Default.ftp_password);
                requestDir.UsePassive = true;
                requestDir.UseBinary = true;
                requestDir.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return true;
                }
                else
                {
                    response.Close();
                    return false;
                }
            }
        }

        /// <summary>
        /// Mengecek IWAS version harus diatas 5.5.2 //NewAutoICTver5.5.2
        /// </summary>
        /// <param name="processname">Iwas Process Name</param>
        void IWASManager(string processname,string ver)
        {
            string [] angkaz = ver.Split('.');
            int digit1 = Convert.ToInt16(angkaz[0]);
            int digit2 = Convert.ToInt16(angkaz[1]);
            int digit3 = Convert.ToInt16(angkaz[2]);
            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in processlist)
            {
                
                if (theprocess.ProcessName.Contains(processname))
                {
                    try
                    {
                        string prosname = theprocess.ProcessName;
                        string[] version = prosname.Split('r');
                        string[] angka = version[1].Split('.');
                        int major = Convert.ToInt16(angka[0]);
                        int minor = Convert.ToInt16(angka[1]);
                        int revision = Convert.ToInt16(angka[2]);
                        if (major == digit1)
                        {
                            if (minor == digit2)
                            {
                                if (revision >= digit3)
                                {
                                    break;
                                }
                                else
                                {
                                    TutupProses(theprocess.ProcessName);
                                    IWASnotif iwas = new IWASnotif();
                                    iwas.Show();
                                }
                            }
                            if (minor > digit2)
                            {
                                break;
                            }
                            if (minor < digit2)
                            {
                                TutupProses(theprocess.ProcessName);
                                IWASnotif iwas = new IWASnotif();
                                iwas.Show();
                            }
                        }
                        if (major > digit1)
                        {
                            break;
                        }
                        if (major < digit1)
                        {
                            TutupProses(theprocess.ProcessName);
                            IWASnotif iwas = new IWASnotif();
                            iwas.Show();
                        }
                    }
                    catch
                    {
                        TutupProses(theprocess.ProcessName);
                        IWASnotif iwas = new IWASnotif();
                        iwas.Show();
                    }
                    
                }
            }
        }
        /// <summary>
        /// Menutup Semua Proses
        /// </summary>
        /// <param name="name">nama proses yang ingin ditutup</param>
        void TutupProses(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    clsProcess.Kill();
                    clsProcess.WaitForExit();
                    clsProcess.Dispose();
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gunaLabel1.Text = Tester();
            gunaLabel6.Text = GetLocalIPAddress();
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public string Tester()
        {
            string TesterNow;
            try
            {

                if (File.Exists(Properties.Settings.Default.testername1))
                {
                    TesterNow = parsing(Properties.Settings.Default.testername1);
                }
                else
                {
                    TesterNow = parsing(Properties.Settings.Default.testername2);
                }
            }
            catch
            {
                TesterNow = "Gagal Membaca Khaensu";
            }

            return TesterNow;
        }
        string station;
        private string parsing(string tester)
        {
             string [] TesteR  = System.IO.File.ReadAllLines(tester);
             foreach(string Read in TesteR)
             {
                 if (Read.Contains("StationName"))
                 {
                     station = Read;
                 }
             }
             string [] preparse = station.Split('=');
             string finalparse = preparse[1];
             return finalparse;
        }
        public string Yield()
        {
            string YieldNow;
            try
            {

                if (File.Exists(Properties.Settings.Default.yield1))
                {
                    YieldNow = parsingyield(Properties.Settings.Default.yield1);
                }
                else
                {
                    YieldNow = parsingyield(Properties.Settings.Default.yield2);
                }
            }
            catch
            {
                YieldNow = "";
            }

            return YieldNow;
        }
        private string parsingyield(string yield)
        {
            string YieldR = System.IO.File.ReadAllText(yield);
            string [] preparse = YieldR.Split('@');
            return preparse[6];
        }
        int prestart = 0;
        int tolerace = 0;
        private void gunaLabel2_TextChanged(object sender, EventArgs e)
        {
            prestart++;
            if (prestart > 1)
            {
                if (gunaLabel2.Text != "")
                {
                    List<String> Mylocalfolder = Directory.GetFiles(Properties.Settings.Default.local_directory, "*", SearchOption.AllDirectories).ToList();
                    if (Mylocalfolder.Count != 0)
                    {
                        tolerace = 0;
                        var testplan = _readTestplan.Project();
                        gunaLabel5.Text = testplan;
                        _shortLocalFile.testplan = gunaLabel5.Text;
                        _shortLocalFile.ShortFile();

                        if (FtpCheck(Properties.Settings.Default.ftp_directory))
                        {
                            string localproject = Properties.Settings.Default.local_project;
                            string ftppath = Properties.Settings.Default.ftp_directory;

                            Send(localproject, ftppath);
                        }
                    }
                    else
                    {
                        tolerace++;
                        if (tolerace == 3)
                        {
                            Warning Wr = new Warning();
                            Wr.Show();
                        }

                    }
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(Properties.Settings.Default.SendSignal + gunaLabel1.Text + "/");
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
            string Data = "Station : " + gunaLabel1.Text + "\n\r" + "Total Test : " + gunaLabel2.Text + "\n\r" + "Project : " + gunaLabel5.Text + "\n\r" + "Date : " + DateTime.Now + "\n\r" + "Version : " + label1.Text + "\n\r" + "IP Computer : " + gunaLabel6.Text ;
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "Info.txt", Data);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Tester_Location TL = new Tester_Location();
            TL.Show();
        }

        private void guna2ControlBox2_Click(object sender, EventArgs e)
        {
            timer = 0;
        }
    }
}
