using CommonUtilities.StandardStreams.Redirectors;
using System.Threading;

namespace Pixiv.Utilities.Ugoira.Convert.WebP.Console
{
    public static partial class Utility
    {
        public static partial class StandardOutputMonitor
        {
            private static Thread monitoringThread = null;
            private static bool isRunning = false;

            public static void setListener(ActionListener listener = null)
            {
                StandardOutputMonitor.listener = listener;
            }

            public static void dispose()
            {
                stop();
                StandardStreamRedirector.close(StandardStreamRedirector.StreamFileDescriptor.Output);
            }

            public static void start()
            {
                if (!isRunning)
                {
                    isRunning = true;

                    StandardStreamRedirector.open(StandardStreamRedirector.StreamFileDescriptor.Output, null);
                    onRedirectionStarted();

                    monitoringThread = new Thread(new ThreadStart(monitoring));
                    monitoringThread.Start();
                }
            }

            public static void stop()
            {
                if (isRunning)
                {
                    isRunning = false;

                    StandardStreamRedirector.close(StandardStreamRedirector.StreamFileDescriptor.Output);
                    onRedirectionStoped();
                }
            }

            public static void writeStandardOutput(string message)
            {
                StandardStreamRedirector.writeStandardStream(StandardStreamRedirector.StreamFileDescriptor.Output, message);
            }

            public static void monitoring()
            {
                string buffer = string.Empty;

                while (isRunning)
                {
                    buffer = StandardStreamRedirector.getBuffer(StandardStreamRedirector.StreamFileDescriptor.Output);

                    if (buffer.Length > 0)
                        onRedirectionPerformed(buffer);
                }
            }
        }
    }
}
