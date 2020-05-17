using Pixiv.Utilities.Ugoira.Assembly.Managers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Pixiv.Utilities.Ugoira.Assembly
{
    public class UgoiraAssembler : IDisposable
    {
        private ConcurrentQueue<string> inputFilesQueue = null;
        private List<string> inputFiles = null;

        private Thread[] threads = null;
        private object mutex = new object();
        private int threadCount = 1;

        private FileManager fileManager = null;
        private WebPManager.ActionListener webPActionListener = null;

        public static readonly int maxThreadCount = (Environment.ProcessorCount > 0 ? Environment.ProcessorCount : 1) * 2;

        public UgoiraAssembler(FileManager.ActionListener fileActionListener = null, WebPManager.ActionListener webPActionListener = null, int threadCount = 4)
        {
            this.webPActionListener = webPActionListener;

            fileManager = new FileManager(fileActionListener);
            inputFilesQueue = new ConcurrentQueue<string>(inputFiles = new List<string>(fileManager.getInputFiles()));

            threadCount = Math.Min(inputFiles.Count, Math.Min(maxThreadCount, threadCount));

            threads = new Thread[this.threadCount = threadCount];
            for (int i = 0; i < threadCount; i++)
                threads[i] = new Thread(new ThreadStart(run)) { IsBackground = true };
        }

        public void Dispose()
        {
            fileManager.Dispose();
        }

        ~UgoiraAssembler()
        {
            Dispose();
        }

        public UgoiraAssembler startAssembly()
        {
            for (int i = 0; i < threadCount; i++)
                threads[i].Start();

            return this;
        }

        public void join()
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (threads[i].IsAlive)
                {
                    try { threads[i].Join(); }
                    catch { }
                }
            }

            fileManager.scanErrorLog();
        }

        private void run()
        {
            while (!inputFilesQueue.IsEmpty)
            {
                try
                {
                    bool isValidInputFile = false;
                    string inputFile = null;

                    lock (mutex)
                    {
                        isValidInputFile = inputFilesQueue.TryDequeue(out inputFile);
                    }

                    if (isValidInputFile)
                        new WebPManager(inputFile, getIndexOf(inputFile) + 1, getCount(), webPActionListener).produceWebP();
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

        private int getIndexOf(string inputFile)
        {
            lock (mutex)
            {
                return inputFiles.IndexOf(inputFile);
            }
        }

        private int getCount()
        {
            lock (mutex)
            {
                return inputFiles.Count;
            }
        }
    }
}
