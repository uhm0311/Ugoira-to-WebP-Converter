using Pixiv.Utilities.Ugoira.ActionListeners;
using Pixiv.Utilities.Ugoira.Assembly;
using Pixiv.Utilities.Ugoira.Assembly.ActionListeners;
using Pixiv.Utilities.Ugoira.Assembly.Managers;
using Pixiv.Utilities.Ugoira.Download.ActionListeners;
using Pixiv.Utilities.Ugoira.Download.Managers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pixiv.Utilities.Ugoira.Convert.WebP.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] blacklist = new string[] { "Vector smash protection is enabled.\n" };
            Utility.StandardOutputMonitor.setListener(new MonitorActionListener(blacklist));
            Utility.StandardOutputMonitor.start();

            DownloadManager manager = new DownloadManager(getUgoiraLinks(), new DownloadActionListener(), 1);
            manager.startDownload().join();

            Utility.StandardOutputMonitor.dispose();

            UgoiraAssembler assembler = new UgoiraAssembler(new FileActionListener(), new WebPActionListener(), 1);
            assembler.startAssembly().join();

            Utility.printTerminationMessage();
        }

        static List<string> getUgoiraLinks()
        {
            string[] tempLinks = null;
            List<string> ugoiraLinks = new List<string>();

            try
            {
                tempLinks = File.ReadAllLines(FileManager.ugoiraLinkPath);
            }
            catch
            {
                if (!File.Exists(FileManager.ugoiraLinkPath))
                {
                    if (!Directory.Exists(Directory.GetParent(FileManager.ugoiraLinkPath).FullName))
                        Directory.GetParent(FileManager.ugoiraLinkPath).Create();
                    File.Create(FileManager.ugoiraLinkPath).Close();
                }

                tempLinks = new string[0];
            }

            foreach (string ugoiraLink in tempLinks)
            {
                Uri uriResult = null;

                if (Uri.TryCreate(ugoiraLink, UriKind.Absolute, out uriResult))
                {
                    string uri = uriResult.ToString();

                    if (!ugoiraLinks.Contains(uri) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                        ugoiraLinks.Add(uri);
                }
            }

            return ugoiraLinks;
        }
    }
}