using Pixiv.Utilities.Ugoira.Assembly.Managers;
using Pixiv.Utilities.Ugoira.Download.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pixiv.Utilities.Ugoira.Convert.WebP
{
    partial class MainForm : DownloadManager.ActionListener
    {
        public void onPixivConnectionFailed()
        {
            if (!loginDialog.InvokeRequired)
            {
                showErrorMessage("Pixiv connection failed. Check your internet state.");
                this.Close();
            }
            else loginDialog.Invoke(new Action(onPixivConnectionFailed));
        }

        private delegate Tuple<string, string> OnPixivLoginNeededCallback();
        public Tuple<string, string> onPixivLoginNeeded()
        {
            if (!loginDialog.InvokeRequired)
            {
                if (!this.InvokeRequired)
                {
                    string id = string.Empty;
                    string pw = string.Empty;

                    loginDialog.hidden = false;
                    loginDialog.Visible = false;

                    if (loginDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        id = loginDialog.getID();
                        pw = loginDialog.getPassword();

                        loginDialog.clearLoginInfo();
                    }
                    else onPixivLoginFailed();

                    return new Tuple<string, string>(id, pw);
                }
                else
                {
                    OnPixivLoginNeededCallback callback = new OnPixivLoginNeededCallback(onPixivLoginNeeded);
                    return (Tuple<string, string>)this.Invoke(callback);
                }
            }
            else
            {
                OnPixivLoginNeededCallback callback = new OnPixivLoginNeededCallback(onPixivLoginNeeded);
                return (Tuple<string, string>)loginDialog.Invoke(callback);
            }
        }

        public void onPixivLoginStarted()
        {
            setMessageLabelText("Login...");
        }

        public void onPixivLoginPerformed()
        {
            setMessageLabelText("Login... Done.");
        }

        public void onPixivLoginFailed()
        {
            if (!loginDialog.InvokeRequired)
            {
                loginDialog.Close();
                setMessageLabelText("Login... Failed.");

                showErrorMessage("Pixiv login failed. Check your ID or password.");
            }
            else loginDialog.Invoke(new Action(onPixivLoginFailed));
        }

        public void onUgoiraNavigationStarted(int currentPosition, int ugoiraCount)
        {
            setProgressLabelText(currentPosition - 1, "Navigating Ugoira... " + progressMessage(currentPosition, ugoiraCount));
        }

        public void onUgoiraNavigationPerformed(int currentPosition, int ugoiraCount)
        {
            setProgressLabelText(currentPosition - 1, getProgressLabelText(currentPosition - 1) + " Done.");
        }

        public void onUgoiraDownloadStarted(string ugoiraZipFileName, int currentPosition, int ugoiraCount)
        {
            setProgressLabelText(currentPosition - 1, "Downloading \"" + ugoiraZipFileName + "\"... " + progressMessage(currentPosition, ugoiraCount));
        }

        public void onUgoiraDownloadProcessing(string ugoiraZipFileName, int ugoiraPosition, int ugoiraCount, long downloadedBytes, long totalBytes)
        {
            setProgressBarValue(ugoiraPosition - 1, System.Convert.ToInt32(downloadedBytes), System.Convert.ToInt32(totalBytes));
        }

        public void onUgoiraDownloadPerformed(string ugoiraZipFileName, string base64UgoiraZip, int currentPosition, int ugoiraCount)
        {
            File.WriteAllBytes(Path.Combine(FileManager.inputPath, ugoiraZipFileName), System.Convert.FromBase64String(base64UgoiraZip));

            downloadedFileNames.Add(ugoiraZipFileName);
            setProgressLabelText(currentPosition - 1, getProgressLabelText(currentPosition - 1) + " Done.");

            if (downloadedFileNames.Count == ugoiraCount)
            {
                if (this.InvokeRequired)
                    this.Invoke(new Action(startAssembly));
                else startAssembly();
            }
        }

        public void onUgoiraDownloadFailed(string ugoiraZipFileName, int currentPosition, int ugoiraCount)
        {
            setProgressLabelText(currentPosition - 1, getProgressLabelText(currentPosition - 1) + " Failed.");
            downloadFailedFileNames.Add(ugoiraZipFileName);
        }
    }
}
