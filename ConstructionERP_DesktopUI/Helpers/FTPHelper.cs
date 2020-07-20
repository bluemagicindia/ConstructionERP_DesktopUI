using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace ConstructionERP_DesktopUI.Helpers
{
    public class FTPHelper
    {

        #region Upload Logic

        public static void UploadFile(string filePath, string uploadPath)
        {
            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["FTPUsername"], ConfigurationManager.AppSettings["FTPPassword"]);
                client.UploadFile(uploadPath, WebRequestMethods.Ftp.UploadFile, filePath);
            }
        }

        #endregion

        #region Download Logic

        public static void DownloadFile(string downloadFromPath)
        {
            var saveSheetPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Downloads");
            if (Directory.Exists(saveSheetPath))
            {
                string fileName = downloadFromPath.Substring(downloadFromPath.LastIndexOf("/") + 1, downloadFromPath.Length - downloadFromPath.LastIndexOf("/") - 1);
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["FTPUsername"], ConfigurationManager.AppSettings["FTPPassword"]);
                    client.DownloadFileAsync(new Uri(downloadFromPath), saveSheetPath + "/" + fileName);
                }
            }

        }

        #endregion

        #region Delete Login

        public static void DeletFile(string fileUri)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fileUri);
            request.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["FTPUsername"], ConfigurationManager.AppSettings["FTPPassword"]);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            response.Close();
        }

        #endregion
    }
}
