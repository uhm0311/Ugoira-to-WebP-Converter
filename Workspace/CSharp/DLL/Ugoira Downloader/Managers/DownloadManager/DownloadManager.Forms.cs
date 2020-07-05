using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Pixiv.Utilities.Ugoira.Download.Managers
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class DownloadManager
    {
        private delegate void FormCloseMethodCallback();
        private int ugoiraIndex = 0;

        private Thread loginTimeoutDectector = null;
        private int loginTimeout = int.MaxValue;

        private bool loginScriptInvoked = false;

        private object loginMutex = new object();
        private bool isLoginPerformed = true;
        private bool isLoginFailed = true;

        private Form form = null;
        private ChromiumWebBrowser browser = null;

        [STAThread]
        private void initForm()
        {
            form = new Form() { /*WindowState = FormWindowState.Minimized, ShowInTaskbar = false*/ };
            form.Load += new EventHandler(formLoaded);

            browser = new ChromiumWebBrowser("") { Dock = DockStyle.Fill };
            browser.AddressChanged += new EventHandler<AddressChangedEventArgs>(addressChanged);

            form.Controls.Add(browser);
            Application.Run(form);
        }

        private void closeForm()
        {
            if (form != null)
            {
                if (form.InvokeRequired)
                    form.Invoke(new FormCloseMethodCallback(closeForm));
                else form.Close();
            }
        }

        private void formLoaded(object sender, EventArgs e)
        {
            navigateNextUgoira();
        }

        private void addressChanged(object sender, AddressChangedEventArgs e)
        {
            if (e.Address.ToLower().Contains("pixiv"))
            {
                if (isPixivAlive())
                {
                    string currentLink = ugoiraLinks[ugoiraIndex - 1];
                    StringBuilder script = new StringBuilder();

                    if (!e.Address.Equals(currentLink))
                    {
                        Tuple<string, string> idPw = onPixivLoginNeeded();

                        script.AppendLine(Properties.Resources.pixiv_login);
                        script.Append(loginFunction).Append("(\"").Append(idPw.Item1).Append("\", \"").Append(idPw.Item2).AppendLine("\");");

                        browser.EvaluateScriptAsync(script.ToString()).ContinueWith(response =>
                        {
                            onPixivLoginStarted();

                            loginTimeoutDectector = new Thread(detectLoginTimeout) { IsBackground = true };
                            loginTimeoutDectector.Start();

                            loginScriptInvoked = true;
                        });
                    }
                    else
                    {
                        lock (loginMutex)
                        {
                            if (isLoginPerformed)
                            {
                                isLoginFailed = false;

                                if (loginScriptInvoked)
                                {
                                    onPixivLoginPerformed();
                                    loginScriptInvoked = false;
                                }

                                script.AppendLine(Properties.Resources.jszip_min);
                                script.AppendLine(Properties.Resources.ugoira_data);
                                script.Append(ugoiraDataFunction).AppendLine("();");

                                browser.EvaluateScriptAsync(script.ToString()).ContinueWith(response =>
                                {
                                    JToken ugoiraData = JObject.Parse(response.Result.Result.ToString());
                                    string fileName = ugoiraData[ugoiraZipFileName].ToString();

                                    if (!ugoiraSrcMap.ContainsKey(fileName))
                                    {
                                        ugoiraZipFileNamesQueue.Enqueue(fileName);
                                        ugoiraSrcMap.Add(fileName, new Tuple<string, string, string>(ugoiraLinks[ugoiraIndex - 1], ugoiraData[ugoiraSrcName].ToString(), ugoiraData[ugoiraInfo].ToString()));

                                        onUgoiraNavigationPerformed(ugoiraIndex, ugoiraLinks.Count);
                                        navigateNextUgoira();
                                    }
                                });
                            }
                        }
                    }
                }
            }
        }

        private void detectLoginTimeout()
        {
            Thread.Sleep(loginTimeout);

            lock (loginMutex)
            {
                if (isLoginFailed)
                {
                    isLoginPerformed = false;

                    onPixivLoginFailed();
                    closeForm();
                }
            }
        }

        public void navigateNextUgoira()
        {
            if (ugoiraIndex < ugoiraLinks.Count)
            {
                onUgoiraNavigationStarted(ugoiraIndex + 1, ugoiraLinks.Count);
                browser.Load(createRedirectedLoginLink(ugoiraLinks[ugoiraIndex++]));
            }
            else closeForm();
        }
    }
}
