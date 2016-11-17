using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MoveFiles
{
    class Program
    {
        static string[] defaultEndings = new string[] { "*.dll", "*.exe" };
        static Process processNiad = new Process();
        
        static void Main(string[] args)
        {
            string input;
            do
            {
                Console.WriteLine("Select a command:");
                Console.WriteLine("Copy PLM debug to niad: 1");
                Console.WriteLine("Copy AM debug to niad: 2");
                Console.WriteLine("Copy localstore debug to niad: 3");
                Console.WriteLine("Copy localstore release to processshare: 4");
                Console.WriteLine("Copy AM release to processshare: 5");
                Console.WriteLine("Copy crawler release to processshare: 6");
                Console.WriteLine("exit: e");

                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        CopyPLMToNiad();
                        break;
                    case "2":
                        CopyAMToNiad();
                        break;
                    case "3":
                        break;
                    case "4":
                        CopyLocalStoreToProduction();
                        break;
                    case "5":
                        CopyAMToProduction();
                        break;
                    case "6":
                        CopyCrawlerToProduction();
                        break;

                    default:
                        break;
                }

            } while (input != "e");    
        }
        
        public static void CopyPLMToNiad()
        {
            string pathlocalplmdebug = ConfigurationManager.AppSettings["pathplmdebug"];
            string pathniadplm = ConfigurationManager.AppSettings["pathniadplm"];

            CopyFiles(pathlocalplmdebug, pathniadplm, defaultEndings);
        }

        public static void CopyAMToNiad()
        {
            string pathamdebug = ConfigurationManager.AppSettings["pathamdebug"];
            string pathniadam = ConfigurationManager.AppSettings["pathniadam"];

            CopyFiles(pathamdebug, pathniadam, defaultEndings);
        }

        public static void CopyAMToProduction()
        {
            string pathamprocesssharecopy = ConfigurationManager.AppSettings["pathamprocesssharecopy"];
            string pathamprocessshare = ConfigurationManager.AppSettings["pathamprocessshare"];
            string pathamrelease = ConfigurationManager.AppSettings["pathamrelease"];
            
            string destpathcopy = System.IO.Path.Combine(pathamprocesssharecopy, DateTime.Now.ToString("yyyy-MM-dd"));
                
            System.IO.Directory.CreateDirectory(destpathcopy);

            CopyFiles(pathamprocessshare, destpathcopy, defaultEndings);

            CopyFiles(pathamrelease, pathamprocessshare, defaultEndings);
        }
        

        public static void CopyLocalStoreToProduction()
        {
            string pathlocalstorecopy = ConfigurationManager.AppSettings["pathlocalstoreprod"];
            string pathlocalstoreprod = ConfigurationManager.AppSettings["pathlocalstoreprod"];
            string pathlocalstorerelease = ConfigurationManager.AppSettings["pathlocalstorerelease"];

            string destpathcopy = System.IO.Path.Combine(pathlocalstorecopy, DateTime.Now.ToString("yyyy-MM-dd"));

            System.IO.Directory.CreateDirectory(destpathcopy);

            CopyFiles(pathlocalstoreprod, destpathcopy, defaultEndings);

            CopyFiles(pathlocalstorerelease, pathlocalstoreprod, defaultEndings);
        }
        
        public static void CopyCrawlerToProduction()
        {
            string pathcrawlerprod = ConfigurationManager.AppSettings["pathcrawlerprod"];
            string pathcrawlercopy = ConfigurationManager.AppSettings["pathcrawlercopy"];
            string pathcrawlerrelease = ConfigurationManager.AppSettings["pathcrawlerrelease"];
            string pathcrawleramazon = ConfigurationManager.AppSettings["pathcrawleramazon"];

            string destpathcopy = System.IO.Path.Combine(pathcrawlercopy, DateTime.Now.ToString("yyyy-MM-dd"));

            System.IO.Directory.CreateDirectory(destpathcopy);

            CopyFiles(pathcrawlerprod, destpathcopy, defaultEndings);

            CopyFiles(pathcrawlerrelease, pathcrawlerprod, defaultEndings);

            CopyFiles(pathcrawlerrelease, pathcrawleramazon, defaultEndings);
        }
        
        public static void CopyFiles(string sourcePath, string destinationPath, string[] endings)
        {
            if (System.IO.Directory.Exists(sourcePath))
            {
                List<string> filteredFiles = new List<string>();

                foreach (string ending in endings)
                {
                    string[] files = System.IO.Directory.GetFiles(sourcePath, ending);
                    foreach (string f in files)
                    {
                        filteredFiles.Add(f);
                    }
                }
                
                foreach (string s in filteredFiles)
                {
                    string filename = System.IO.Path.GetFileName(s);
                    string destfilepath = System.IO.Path.Combine(destinationPath, filename);
                    System.IO.File.Copy(s, destfilepath, true);
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }
        }
    }
}
