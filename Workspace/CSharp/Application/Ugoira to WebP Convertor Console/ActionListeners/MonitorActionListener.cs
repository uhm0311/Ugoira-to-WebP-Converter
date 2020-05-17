using Pixiv.Utilities.Ugoira.Convert.WebP.Console;
using System.Collections.Generic;

namespace Pixiv.Utilities.Ugoira.ActionListeners
{
    public class MonitorActionListener : Utility.StandardOutputMonitor.ActionListener
    {
        private IEnumerable<string> blacklist = null;

        public MonitorActionListener(IEnumerable<string> blacklist)
        {
            this.blacklist = blacklist;
        }

        public void onRedirectionStarted()
        {

        }

        public void onRedirectionPerformed(string redirectedContent)
        {
            foreach (string s in blacklist)
                redirectedContent = redirectedContent.Replace("\r", string.Empty).Replace(s, string.Empty);

            if (redirectedContent.Length > 0)
                Utility.StandardOutputMonitor.writeStandardOutput(redirectedContent);
        }

        public void onRedirectionStoped()
        {

        }
    }
}
