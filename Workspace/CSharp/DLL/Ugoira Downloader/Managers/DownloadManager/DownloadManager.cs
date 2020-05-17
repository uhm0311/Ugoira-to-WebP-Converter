using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;

namespace Pixiv.Utilities.Ugoira.Download.Managers
{
    public partial class DownloadManager
    {
        private List<string> ugoiraLinks = new List<string>();

        private Dictionary<string, Tuple<string, string, string>> ugoiraSrcMap = new Dictionary<string, Tuple<string, string, string>>();
        private ConcurrentQueue<string> ugoiraZipFileNamesQueue = new ConcurrentQueue<string>();
        private List<string> ugoiraZipFileNames = new List<string>();

        private Thread formThead = null;
        private Thread[] threads = null;
        private object mutex = new object();
        private int threadCount = 1;

        public static readonly int maxThreadCount = 2;

        public DownloadManager(IList<string> ugoiraLinks, ActionListener actionListener = null, int threadCount = 4)
            : this(ugoiraLinks, actionListener, actionListener, threadCount) { }

        public DownloadManager(IList<string> ugoiraLinks, ConnectionListener connectionListener = null, DownloadListener downloadListener = null, int threadCount = 4)
        {
            this.connectionListener = connectionListener;
            this.downloadListener = downloadListener;
            this.ugoiraLinks.AddRange(ugoiraLinks);

            threadCount = Math.Min(ugoiraLinks.Count, Math.Min(maxThreadCount, threadCount));
            
            threads = new Thread[this.threadCount = threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(new ThreadStart(run)) { IsBackground = true };
                threads[i].SetApartmentState(ApartmentState.STA);
            }
        }

        public DownloadManager startDownload()
        {
            formThead = new Thread(() => 
            {
                if (isPixivAlive())
                {
                    initForm();

                    for (int i = 0; i < threadCount; i++)
                        threads[i].Start();
                }
            });

            formThead.IsBackground = true;
            formThead.SetApartmentState(ApartmentState.STA);
            formThead.Start();

            return this;
        }

        public void join()
        {
            if (formThead != null && formThead.IsAlive)
                formThead.Join();

            for (int i = 0; i < threadCount; i++)
            {
                if (threads[i].IsAlive)
                {
                    try { threads[i].Join(); }
                    catch { }
                }
            }
        }

        private void run()
        {
            while (!ugoiraZipFileNamesQueue.IsEmpty)
            {
                try
                {
                    bool isValidZipFileName = false;
                    string ugoiraZipFileName = null;

                    lock (mutex)
                    {
                        isValidZipFileName = ugoiraZipFileNamesQueue.TryDequeue(out ugoiraZipFileName);
                    }

                    if (isValidZipFileName)
                        downloadUgoira(ugoiraZipFileName, getIndexOf(ugoiraZipFileName), getCount());
                }
                catch (Exception e)
                {
                    lock (mutex)
                    {
                        Console.Error.WriteLine(e.ToString());
                        Console.Error.WriteLine();
                    }
                }
            }
        }

        private int getIndexOf(string ugoiraZipFileName)
        {
            lock (mutex)
            {
                return ugoiraZipFileNames.IndexOf(ugoiraZipFileName);
            }
        }

        private int getCount()
        {
            lock (mutex)
            {
                return ugoiraZipFileNames.Count;
            }
        }
    }
}
