using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace Pixiv.Utilities.Ugoira.Download.Managers
{
    public partial class DownloadManager
    {
        private bool isPixivAlive()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pixiv);
            HttpWebResponse response;

            try
            {
                bool result = false;
                HttpStatusCode code = (response = (HttpWebResponse)request.GetResponse()).StatusCode;

                if ((int)code < 400)
                    result = true;

                response.Close();
                return result;
            }
            catch
            {
                onPixivConnectionFailed();
                closeForm();

                return false;
            }
        }

        private string createRedirectedLoginLink(string ugoiraLink)
        {
            StringBuilder builder = new StringBuilder("https://accounts.pixiv.net/login?return_to=");
            builder.Append(HttpUtility.UrlEncode(ugoiraLink)).Append("&source=pc&view_type=page");
            
            return builder.ToString();
        }

        private void downloadUgoira(string ugoiraZipFileName, int ugoiraIndex, int ugoiraCount)
        {
            try
            {
                onUgoiraDownloadStarted(ugoiraZipFileName, ugoiraIndex + 1, ugoiraCount);

                string ugoiraLink = ugoiraSrcMap[ugoiraZipFileName].Item1;
                string ugoiraSrc = ugoiraSrcMap[ugoiraZipFileName].Item2;

                EventWaitHandle waiter = new EventWaitHandle(false, EventResetMode.AutoReset);
                Connection.CookieAwareWebClient client = new Connection.CookieAwareWebClient(GetUriCookieContainer(new Uri(pixiv)));                
                client.BaseAddress = ugoiraLink;
                client.Headers.Add("Referer", ugoiraLink);

                client.DownloadProgressChanged += (sender, e) => onUgoiraDownloadProcessing(ugoiraZipFileName, ugoiraIndex + 1, ugoiraCount, e.BytesReceived, e.TotalBytesToReceive);
                client.DownloadDataCompleted += (sender, e) => 
                {
                    byte[] ugoira = e.Result;
                    onUgoiraDownloadProcessing(ugoiraZipFileName, ugoiraIndex + 1, ugoiraCount, ugoira.Length, ugoira.Length);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        stream.Write(ugoira, 0, ugoira.Length);

                        using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Update))
                        {
                            ZipArchiveEntry readmeEntry = archive.CreateEntry(ugoiraInfo);
                            using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                            {
                                writer.WriteLine(ugoiraSrcMap[ugoiraZipFileName].Item3);
                            }
                        }
                        onUgoiraDownloadPerformed(ugoiraZipFileName, ugoiraSrcMap[ugoiraZipFileName].Item4, Convert.ToBase64String(stream.ToArray()), ugoiraIndex + 1, ugoiraCount);
                    }
                    waiter.Set();
                };

                client.DownloadDataAsync(new Uri(ugoiraSrc));
                waiter.WaitOne();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                Console.Error.WriteLine();

                onUgoiraDownloadFailed(ugoiraZipFileName, ugoiraIndex + 1, ugoiraCount);
            }
        }
    }
}
