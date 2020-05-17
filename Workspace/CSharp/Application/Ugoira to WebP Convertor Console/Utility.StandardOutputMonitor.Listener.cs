
namespace Pixiv.Utilities.Ugoira.Convert.WebP.Console
{
    public static partial class Utility
    {
        public static partial class StandardOutputMonitor
        {
            private static ActionListener listener = null;

            public interface ActionListener
            {
                void onRedirectionStarted();
                void onRedirectionPerformed(string redirectedContent);
                void onRedirectionStoped();
            }

            private static void onRedirectionStarted()
            {
                if (listener != null)
                    listener.onRedirectionStarted();
            }

            private static void onRedirectionPerformed(string redirectedContent)
            {
                if (listener != null)
                    listener.onRedirectionPerformed(redirectedContent);
            }

            private static void onRedirectionStoped()
            {
                if (listener != null)
                    listener.onRedirectionStoped();
            }
        }
    }
}
