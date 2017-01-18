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
        static string pathlocalplmrelease = ConfigurationManager.AppSettings["pathplmrelease"];
        static string pathniadplm = ConfigurationManager.AppSettings["pathniadplm"];
        static string pathlocalstoreniad = ConfigurationManager.AppSettings["pathlocalstoreniad"];
        static string pathamrelease = ConfigurationManager.AppSettings["pathamrelease"];
        static string pathniadam = ConfigurationManager.AppSettings["pathniadam"];
        static string pathamprocesssharecopy = ConfigurationManager.AppSettings["pathamprocesssharecopy"];
        static string pathamprocessshare = ConfigurationManager.AppSettings["pathamprocessshare"];
        static string pathlocalstorecopy = ConfigurationManager.AppSettings["pathlocalstorecopy"];
        static string pathlocalstoreprod = ConfigurationManager.AppSettings["pathlocalstoreprod"];
        static string pathlocalstorerelease = ConfigurationManager.AppSettings["pathlocalstorerelease"];
        static string pathcrawlerprod = ConfigurationManager.AppSettings["pathcrawlerprod"];
        static string pathcrawlercopy = ConfigurationManager.AppSettings["pathcrawlercopy"];
        static string pathcrawlerrelease = ConfigurationManager.AppSettings["pathcrawlerrelease"];
        static string pathcrawleramazon = ConfigurationManager.AppSettings["pathcrawleramazon"];

        static void Main(string[] args)
        {
            string input;
            do
            {
                Console.WriteLine("Select a command:");
                Console.WriteLine("Copy PLM release to niad: ============================ 1");
                Console.WriteLine("Copy AM release to niad: ============================= 2");
                Console.WriteLine("Copy localstore release to niad: ===================== 3");
                Console.WriteLine("Copy localstore release to processshare: ============= 4");
                Console.WriteLine("Copy AM release to processshare: ===================== 5");
                Console.WriteLine("Copy crawler release to processshare: ================ 6");
                Console.WriteLine("exit: ================================================ e");

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
                        CopyLocalStoreToNiad();
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
            DisconnectFromVPN();
            CopyFiles(pathlocalplmrelease, pathniadplm, defaultEndings);
            ReconnectToVPN();
        }

        private static void ReconnectToVPN()
        {
            Console.WriteLine("Enter VPN Username:");
            string name = Console.ReadLine();
            Console.WriteLine("Enter password to reconnect to VPN: ");
            string pw = ReadPassword();
            string command = string.Format(@"""DEV VPN"" ""{0}"" ""{1}""", name, pw);
            Process.Start("rasdial.exe", command);
        }

        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }

        private static void DisconnectFromVPN()
        {
            Process.Start("rasdial.exe", @"""DEV VPN"" /DISCONNECT");
        }

        public static void CopyLocalStoreToNiad()
        {
            DisconnectFromVPN();
            CopyFiles(pathlocalstorerelease, pathlocalstoreniad, defaultEndings);
            ReconnectToVPN();
        }

        public static void CopyAMToNiad()
        {
            DisconnectFromVPN();
            CopyFiles(pathamrelease, pathniadam, defaultEndings);
            ReconnectToVPN();
        }

        public static void CopyAMToProduction()
        {
            string destpathcopy = System.IO.Path.Combine(pathamprocesssharecopy, DateTime.Now.ToString("yyyy-MM-dd"));

            int i = 1;
            string append = string.Empty;
            while (System.IO.Directory.Exists(destpathcopy + append))
            {
                append = string.Format("({0})", i.ToString());
                i++;
            }

            destpathcopy = destpathcopy + append;

            System.IO.Directory.CreateDirectory(destpathcopy);

            CopyFiles(pathamprocessshare, destpathcopy, defaultEndings);

            CopyFiles(pathamrelease, pathamprocessshare, defaultEndings);
        }


        public static void CopyLocalStoreToProduction()
        {
            string destpathcopy = System.IO.Path.Combine(pathlocalstorecopy, DateTime.Now.ToString("yyyy-MM-dd"));

            int i = 1;
            string append = string.Empty;
            while (System.IO.Directory.Exists(destpathcopy + append))
            {
                append = string.Format("({0})", i.ToString());
                i++;
            }

            destpathcopy = destpathcopy + append;

            System.IO.Directory.CreateDirectory(destpathcopy);

            CopyFiles(pathlocalstoreprod, destpathcopy, defaultEndings);

            CopyFiles(pathlocalstorerelease, pathlocalstoreprod, defaultEndings);
        }

        public static void CopyCrawlerToProduction()
        {
            string destpathcopy = System.IO.Path.Combine(pathcrawlercopy, DateTime.Now.ToString("yyyy-MM-dd"));

            int i = 1;
            string append = string.Empty;
            while (System.IO.Directory.Exists(destpathcopy + append))
            {
                append = string.Format("({0})", i.ToString());
                i++;
            }

            destpathcopy = destpathcopy + append;

            System.IO.Directory.CreateDirectory(destpathcopy);

            CopyFiles(pathcrawlerprod, destpathcopy, defaultEndings);

            CopyFiles(pathcrawlerrelease, pathcrawlerprod, defaultEndings);

            CopyFiles(pathcrawlerrelease, pathcrawleramazon, defaultEndings);
        }

        //public static void CopyFiles(string sourcePath, string destinationPath)
        //{
        //    CopyFiles(sourcePath, destinationPath, null);
        //}

        public static void CopyFiles(string sourcePath, string destinationPath, string[] endings)
        {

            if (System.IO.Directory.Exists(sourcePath))
            {
                List<string> filteredFiles = new List<string>();

                if (endings != null)
                {
                    foreach (string ending in endings)
                    {
                        string[] files = System.IO.Directory.GetFiles(sourcePath, ending);
                        foreach (string f in files)
                        {
                            filteredFiles.Add(f);
                        }
                    }
                }
                else
                {
                    string[] files = System.IO.Directory.GetFiles(sourcePath);
                    foreach (string f in files)
                    {
                        filteredFiles.Add(f);
                    }
                }

                for (int i = 0; i < filteredFiles.Count; i++)
                {
                    if (filteredFiles[i].Contains("vshost.exe"))
                    {
                        filteredFiles.Remove(filteredFiles[i]);
                        break;
                    }
                }

                foreach (string s in filteredFiles)
                {
                    string filename = System.IO.Path.GetFileName(s);
                    string destfilepath = System.IO.Path.Combine(destinationPath, filename);
                    try
                    {
                        System.IO.File.Copy(s, destfilepath, true);
                        System.Threading.Thread.Sleep(100);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.InnerException);
                        Console.WriteLine(e.StackTrace);
                        //break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }
        }
    }
}
