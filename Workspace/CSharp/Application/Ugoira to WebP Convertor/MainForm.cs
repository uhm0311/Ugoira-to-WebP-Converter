using Pixiv.Utilities.Ugoira.Assembly;
using Pixiv.Utilities.Ugoira.Assembly.Managers;
using Pixiv.Utilities.Ugoira.Download.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pixiv.Utilities.Ugoira.Convert.WebP
{
    public partial class MainForm : Form
    {
        private LoginDialog loginDialog = new LoginDialog();

        private List<ProgressBar> progressBars = new List<ProgressBar>();
        private List<Label> progressLabels = new List<Label>();

        private List<string> downloadedFileNames = new List<string>();
        private List<string> downloadFailedFileNames = new List<string>();

        private DownloadManager manager;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            loginDialog.hidden = true;
            loginDialog.Show();

            startDownload();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            int width = Math.Max(this.Size.Width - 40, 1);
            int height = Math.Max(this.Size.Height - 87, 1);

            progressPanel.Size = new Size(width, height);
        }

        private void startDownload()
        {
            List<string> ugoiraLinks = getUgoiraLinks();

            if (ugoiraLinks.Count > 0)
            {
                createProgresses(ugoiraLinks.Count);

                manager = new DownloadManager(ugoiraLinks, this, DownloadManager.maxThreadCount);
                manager.startDownload();
            }
            else startAssembly();
        }

        private void startAssembly()
        {
            string[] ugoiraFileNames = getUgoiraFileNames();
            createProgresses(ugoiraFileNames.Length);

            if (ugoiraFileNames.Length > 0)
            {
                createProgressLabels(ugoiraFileNames.Length);

                Thread tempThead = new Thread(() =>
                {
                    using (UgoiraAssembler assembler = new UgoiraAssembler(this, this, UgoiraAssembler.maxThreadCount))
                    {
                        assembler.startAssembly().join();
                    }
                });

                tempThead.IsBackground = true;
                tempThead.Start();
            }
        }

        private void createProgresses(int count)
        {
            if (count > 0)
            {
                progressPanel.Controls.Clear();

                progressBars.Clear();
                progressLabels.Clear();

                createProgressBars(count);
                createProgressLabels(count);
            }
        }

        private void createProgressBars(int count)
        {
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    ProgressBar progressBar = new ProgressBar();
                    progressBar.Location = new Point(3, 3 + i * 29);

                    progressPanel.Controls.Add(progressBar);
                    progressBars.Add(progressBar);
                }
            }
        }

        private void createProgressLabels(int count)
        {
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Label label = new Label() { AutoSize = true, AutoEllipsis = true };
                    label.MaximumSize = new Size(progressPanel.Size.Width - 6, label.Size.Height);
                    label.Location = new Point(109, 8 + i * 29);
                    label.Text = "Wating...";

                    progressPanel.Controls.Add(label);
                    progressLabels.Add(label);
                }
            }
        }

        private void setMessageLabelText(string message)
        {
            if (!messageLabel.InvokeRequired)
                messageLabel.Text = message;
            else messageLabel.Invoke(new Action<string>(setMessageLabelText), message);
        }

        private void setProgressBarValue(int index, int value, int max)
        {
            if (!progressBars[index].InvokeRequired)
            {
                progressBars[index].Maximum = max;
                progressBars[index].Value = value;
            }
            else progressBars[index].Invoke(new Action<int, int, int>(setProgressBarValue), index, value, max);
        }

        private string getProgressLabelText(int index)
        {
            if (!progressLabels[index].InvokeRequired)
                return progressLabels[index].Text;
            else return progressLabels[index].Invoke(new Func<int, string>(getProgressLabelText), index).ToString();
        }

        private void setProgressLabelText(int index, string message)
        {
            if (!progressLabels[index].InvokeRequired)
                progressLabels[index].Text = message;
            else progressLabels[index].Invoke(new Action<int, string>(setProgressLabelText), index, message);
        }

        private void showErrorMessage(string message)
        {
            if (!this.InvokeRequired)
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else this.Invoke(new Action<string>(showErrorMessage), message);
        }

        private List<string> getUgoiraLinks()
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

        private string[] getUgoiraFileNames()
        {
            string[] result = null;

            if (Directory.Exists(FileManager.inputPath))
            {
                result = Directory.GetFiles(FileManager.inputPath, "*.zip.*");
            }
            else result = new string[0];

            return result;
        }

        private string progressMessage(int currentPosition, int count)
        {
            return string.Format("({0}/{1})", currentPosition, count);
        }
    }
}
