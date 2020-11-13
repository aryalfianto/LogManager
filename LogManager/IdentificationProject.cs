using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogManager
{
    public class IdentificationProject
    {
        public string DirectoryLog1 { get; set; }
        public string DirectoryLog2 { get; set; }

        public string Project()
        {
            string ProjectRun;
            try
            {
                if (File.Exists(DirectoryLog1))
                {
                    ProjectRun = parsing(DirectoryLog1);
                }
                else
                {
                    ProjectRun = parsing(DirectoryLog2);
                }
            }
            catch
            {
                ProjectRun = "Gagal Membaca Testplan";
            }

            return ProjectRun;
        }

        private string parsing(string testplan)
        {
            try
            {
                string project = System.IO.File.ReadAllText(testplan);
                string projectnow = project.Substring(project.LastIndexOf("\\"));
                string[] projectnow1 = projectnow.Split('#');
                string final = projectnow1[0].Replace(@"\", "");
                return final;

            }
            catch
            {
                string project = System.IO.File.ReadAllText(testplan); ;
                string projectnow = project.Substring(project.LastIndexOf("/"));
                string[] projectnow1 = projectnow.Split('#');
                string final = projectnow1[0].Replace(@"/", "");
                return final;
            }
        }
    }
    
}
